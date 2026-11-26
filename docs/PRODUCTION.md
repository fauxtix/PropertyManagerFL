# Produção — deploy IIS, segredos e verificação

> **TL;DR (EN):** Production uses GitHub Secrets (never committed). Deploy with `gh workflow run cd.yml --repo fauxtix/PropertyManagerFL --ref master`; it needs a self-hosted runner registered in **this repo** with label `iis-propertymanager`. The script injects secrets into the published `web.config` as environment variables (Production does not load user-secrets), except the connection string, which comes from `appsettings.Production.json`. Two Syncfusion secrets are required: `PM_SYNCFUSION_LICENSE` (UI) and `PM_SYNCFUSION_LICENSE_API` (API). Pools `PropertyManagerApi` and `PropertyManagerFL` already have SQL logins/`db_owner`. **The CD publishes the apps only: it never creates, drops or migrates a database.**

> **Aviso:** a base de dados de Produção **nunca** é criada nem atualizada automaticamente pelos scripts ou pelo CD. “Responsável da infraestrutura/DBA” significa apenas quem administra o SQL Server desse ambiente — se fizer deploy no seu PC, é você. Com autorização e backup prévio, aplicar em `PropertyManagerDB_Production` apenas a migração idempotente `PropertyManagerFL.Infrastructure/Database/EndLeaseAndArchiving.sql`; nunca os scripts completos nem o snapshot. A ordem correta é: **BD criada/atualizada → CD → verificação**.

## 0. Base de dados antes do deploy

- As duas aplicações leem `PropertyManagerDB_Production` em `appsettings.Production.json`. Nenhum script do repositório contém `CREATE DATABASE`/`DROP DATABASE`; a BD tem de ser criada pelo administrador do SQL Server (num deploy local, pelo próprio utilizador).
- **BD nova:** criar vazia e instalar com **um** dos caminhos de [DATABASE.md](DATABASE.md) — scripts modulares ou snapshot gerado. Não executar ambos.
- **BD já existente:** fazer backup e aplicar uma vez `PropertyManagerFL.Infrastructure/Database/EndLeaseAndArchiving.sql` antes de publicar o código novo. Confirmar:

  ```sql
  SELECT name FROM sys.procedures
  WHERE name IN ('usp_Arrendamentos_EndLease','usp_Inquilinos_Archive','usp_Fiadores_Archive');
  SELECT name FROM sys.tables WHERE name = 'HistoricoArrendamentos';
  ```

- Só depois disto disparar o CD. Publicar o código novo antes da migração faz as novas endpoints chamarem procedures inexistentes.

## 1. Como disparar o deploy

O deploy é manual (`workflow_dispatch`) e tem de ser lançado a partir de `master`:

```powershell
gh workflow run cd.yml --repo fauxtix/PropertyManagerFL --ref master
```

Acompanhar com:

```powershell
gh run list --repo fauxtix/PropertyManagerFL --workflow cd.yml --limit 5
gh run view <id> --repo fauxtix/PropertyManagerFL
```

O workflow `.github/workflows/cd.yml`:

1. faz checkout, instala o SDK a partir de `global.json`;
2. `restore` → `build -c Release` → `test`;
3. publica API e UI (`publish` **com build** — o projeto vazio `PropertyManagerFL.Logging` não gera DLL, pelo que `--no-build` falha com MSB3030);
4. executa `.github/scripts/deploy-iis.ps1`.

### Runner

- `runs-on: [self-hosted, Windows, iis-propertymanager]`.
- O runner tem de estar registado **neste repositório** — runners são *repo-scoped*; um runner registado noutro projeto não serve.
- O serviço do runner corre como `SYSTEM`, pelo que consegue arrancar o IIS e gerir certificados em `LocalMachine\My`.
- O runner usa **Windows PowerShell** (`shell: powershell`), não `pwsh`.

## 2. GitHub Secrets

Os segredos não são visíveis após serem criados e **não são copiados por um `git clone`**. Defini-los em *Settings → Secrets and variables → Actions* ou com `gh secret set`.

| Segredo | Destino |
|---|---|
| `PM_API_KEY` | API e UI — mesmo valor; é o cabeçalho `ApiKey` verificado pelo middleware |
| `PM_SYNCFUSION_LICENSE` | Licença Syncfusion da **UI** |
| `PM_SYNCFUSION_LICENSE_API` | Licença Syncfusion da **API** — **usar o mesmo valor** de `PM_SYNCFUSION_LICENSE` (chave válida 35.2.x que cubra UI + Document SDK) |
| `PM_PMCONNECTION` | Usado **apenas pelo script de deploy** para ler os caminhos de backup em `ApplicationSettings` e conceder ACLs. **Não** é injetado nas aplicações: `appsettings.Production.json` já aponta para `PropertyManagerDB_Production`. |
| `PM_JWT_SECRET`, `PM_JWT_ISSUER`, `PM_JWT_AUDIENCE`, `PM_JWT_EXPIRA_EM_MINUTOS` | API — o `JWT:Secret` é validado no arranque (32+ caracteres) |
| `PM_EMAIL_USERNAME`, `PM_EMAIL_SMTPSERVER`, `PM_EMAIL_PORT`, `PM_EMAIL_PASSWORD`, `PM_EMAIL_FROM` | UI — a secção `EmailConfiguration` é obrigatória no arranque |
| `PM_UI_CERT_THUMBPRINT` | Opcional: thumbprint de certificado existente em `LocalMachine\My` para a binding HTTPS 4310 |

Se os dois segredos Syncfusion puderem ser o mesmo valor (chave Enterprise/Community que cobre todas as edições), ver [SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md). Neste servidor os dois segredos passaram a conter a mesma chave 35.2.x em 2026-11-24; mantêm-se dois segredos separados apenas para permitir chaves diferentes no futuro.

### Como os valores chegam à aplicação

Em Production os user-secrets não são carregados. O `deploy-iis.ps1` escreve cada valor no `web.config` publicado, dentro de `<system.webServer><aspNetCore><environmentVariables>`:

```xml
<environmentVariable name="SyncfusionLicenseKey" value="..." />
```

- O elemento correto é `environmentVariable` (não `<add>`); usar `<add>` provoca o erro ANCM *"Unable to get required configuration section 'system.webServer/aspNetCore'"*.
- Se faltar algum valor, o deploy falha com `Missing deployment value for <chave>` — não fica uma aplicação meio configurada.
- A injeção é idempotente: repetir o deploy não cria entradas duplicadas.
- `ConnectionStrings__PMConnection` **não** é injetado; as duas aplicações leem `appsettings.Production.json`, que aponta para `PropertyManagerDB_Production`. O segredo `PM_PMCONNECTION` serve apenas para o script de deploy descobrir as pastas de backup e aplicar as ACLs. Se o nome da base mudar, alterar esse ficheiro na API e na UI e voltar a deployar — não criar um segredo novo.
- Os restantes valores em `appsettings*.json` são apenas placeholders.

### Rodar uma chave

```powershell
gh secret set PM_SYNCFUSION_LICENSE --repo fauxtix/PropertyManagerFL --body "<nova-chave-ui>"
gh secret set PM_SYNCFUSION_LICENSE_API --repo fauxtix/PropertyManagerFL --body "<nova-chave-api>"
gh workflow run cd.yml --repo fauxtix/PropertyManagerFL --ref master
```

O mesmo padrão serve para qualquer outro segredo. **Nunca** imprimir os valores em logs, issues, `backup.md` ou ficheiros versionados.

## 3. IIS

| Item | Valor |
|---|---|
| Sites | `PropertyManagerFL.Api`, `PropertyManagerFL.UI` |
| Pools | `PropertyManagerApi` (API), `PropertyManagerFL` (UI) |
| Pastas | `C:\inetpub\wwwroot\PropertyManagerFL\Api` e `\UI` |
| Bindings | HTTP `*:4300` (API), HTTPS `*:4310` (UI, **non-SNI**) |
| Certificado | `PM_UI_CERT_THUMBPRINT` se definido; senão self-signed `CN=localhost` |

> Estas portas são **exclusivas do IIS/Produção**. Em Development (Kestrel) a API usa `https://localhost:5001` e a UI `https://localhost:7181` — ver [LOCAL-SETUP.md](LOCAL-SETUP.md). Assim os dois ambientes podem correr ao mesmo tempo sem conflito de bind.

Notas importantes:

- **Reutilizar os pools existentes.** As identidades `IIS APPPOOL\PropertyManagerApi` (também referida como `PropertyManagerAPI` nos scripts SQL) e `IIS APPPOOL\PropertyManagerFL` já têm login SQL e `db_owner` em `PropertyManagerDB`. Pools novos não conseguem abrir a base de dados com `Integrated Security` e o login falha com 500.
- Se criar pools novos noutro servidor, executar `Login_App.sql`, `Login_Api.sql` e `UsersAndSchemas.sql` e dar `db_owner` — ver [DATABASE.md](DATABASE.md).
- A binding 4310 é criada sem SNI por ser uma porta dedicada; assim o IIS não mostra o aviso *"Não foi criado um site SSL predefinido"*.
- O script para os pools antes de copiar os ficheiros (incluindo os antigos `PropertyManagerFL.Api` / `PropertyManagerFL.UI`, que podem reter DLLs em `w3wp.exe`) e volta a arrancá-los no `finally`, mesmo se o deploy falhar.
- A cópia é por sobrescrita: **nunca apaga** a pasta de destino, para preservar ficheiros carregados pelos utilizadores.
- Os 8 modelos `.dotx` são publicados com a API (`CopyToOutputDirectory` no `PropertyManagerFL.Api.csproj`). O CI verifica a sua presença no output de publish e o CD verifica tanto o output como a pasta `Reports\Docs` do IIS, falhando o deploy se faltarem. **Não copiar ficheiros manualmente para o IIS**: corrigir a regra de publicação e voltar a deployar.
- O CD concede `Modify`, de forma idempotente, a **todas as pastas onde a app escreve em runtime**:
  - `Api\Reports\Docs` → pools `PropertyManagerFL` e `PropertyManagerApi` (modelos `.dotx`, subpastas e cartas geradas);
  - `UI\wwwroot\uploads` → pool `PropertyManagerFL` (fotos de imóveis, documentos de inquilinos, imagens de frações);
  - `UI\Data\Files` → pool `PropertyManagerFL` (FileManager da Syncfusion);
  - `Api\uploads` e `Api\wwwroot\uploads` → pool `PropertyManagerApi` (`ImagesController`);
  - `C:\inetpub\wwwroot\PropertyManagerFL\Backups` → ambos os pools;
  - os caminhos `BackupBaseDados` e `BackupOutrosFicheiros` lidos de `ApplicationSettings` no deploy → respetivamente API e UI; a conta do serviço SQL também recebe `Modify` no caminho de backup da base de dados.
- A UI é Blazor Server: o código corre no servidor e grava caminhos absolutos devolvidos pela API. Um `Access to the path ... is denied` significa que a ACL não foi aplicada (ou que o CD não foi repetido), **não** que o caminho seja do PC do utilizador.

## 4. Verificação após o deploy

```powershell
# API: 401 sem chave, 200 com chave
curl.exe -k -s -o NUL -w '%{http_code}' http://localhost:4300/api/Notifications/alerts
curl.exe -k -s -o NUL -w '%{http_code}' -H 'ApiKey: <valor>' http://localhost:4300/api/Notifications/alerts
curl.exe -k -s -o NUL -w '%{http_code}' -H 'ApiKey: <valor>' -H 'X-User-Id: <id-utilizador>' http://localhost:4300/api/Notifications/unread-count
curl.exe -k -s -o NUL -w '%{http_code}' -H 'ApiKey: <valor>' -H 'X-User-Id: <id-utilizador>' 'http://localhost:4300/api/Notifications?take=1'

# Modelos de carta: tem de devolver nomes de ficheiros .dotx (nunca uma lista vazia)
curl.exe -k -s -H 'ApiKey: <valor>' http://localhost:4300/api/Templates/GetTemplatesFilenamesFromServer/pt-PT

# UI: 302 para /Identity/Account/Login
curl.exe -k -s -o NUL -w '%{http_code}' https://localhost:4310/

# Bindings registadas
netsh http show servicestate
```

Esperado: `401`, `200`, `200`, `200`, `302` e as entradas `HTTP://*:4300/` e `HTTPS://*:4310/`. `/alerts` não precisa de `X-User-Id`; a caixa de entrada e o contador devolvem `400` sem esse header. O endpoint de modelos deve listar os `.dotx` (5 para `pt-PT`); lista vazia significa que a publicação não levou a pasta `Reports\Docs`.

Depois da migração e do CD, confirmar também na BD de Produção (os resultados da secção 0 não podem estar vazios):

```sql
SELECT name FROM sys.procedures
WHERE name IN ('usp_Arrendamentos_EndLease','usp_Inquilinos_Archive','usp_Fiadores_Archive')
ORDER BY name;
SELECT name FROM sys.tables WHERE name = 'HistoricoArrendamentos';
SELECT COL_LENGTH('dbo.Recebimentos','ID_Arrendamento')     AS RecebimentosID,
       COL_LENGTH('dbo.RecebimentosTemp','ID_Arrendamento') AS RecebimentosTempID;
```

Na UI, fazer apenas um smoke não destrutivo: abrir **Terminar contrato** num contrato real e cancelar (o aviso de revogação é informativo), e abrir/cancelar a confirmação **Arquivar** num inquilino com contrato ativo (deve estar bloqueado). Para testar a operação real, criar um registo descartável com autorização e restore prévio.

Para confirmar que o CD concedeu escrita em todas as pastas de runtime:

```powershell
$root = 'C:\inetpub\wwwroot\PropertyManagerFL'
$targets = @(
  "$root\Api\Reports\Docs",
  "$root\UI\wwwroot\uploads",
  "$root\UI\Data\Files",
  "$root\Api\uploads",
  "$root\Api\wwwroot\uploads",
  "$root\Backups"
)
foreach ($path in $targets) {
  if (Test-Path $path) {
    $aces = @(icacls $path | Select-String -Pattern 'IIS APPPOOL\\(PropertyManagerFL|PropertyManagerApi)')
    "{0} -> {1}" -f $path, $(if ($aces.Count) { 'OK' } else { 'SEM GRANT' })
  } else {
    "$path -> AINDA NÃO EXISTE"
  }
}
```

Se alguma pasta mostrar `SEM GRANT`, repetir o CD (não corrigir com `icacls` manual: o próximo deploy sobrescreve o estado).

Para confirmar que a chave Syncfusion da UI foi injetada **sem revelar valores** (substituir `<valor-local>` pela chave já existente no seu ambiente):

```powershell
$valorLocal = '<valor-local>'
$webConfig  = Get-Content 'C:\inetpub\wwwroot\PropertyManagerFL\UI\web.config' -Raw
$match      = [regex]::Match($webConfig, '<environmentVariable name="SyncfusionLicenseKey" value="([^"]*)"')
"injetada=$($match.Success) correspondeAoValorLocal=$($match.Groups[1].Value -ceq $valorLocal) comprimento=$($match.Groups[1].Value.Length)"
```

Repetir para a API com `C:\inetpub\wwwroot\PropertyManagerFL\Api\web.config` e a chave da API.

## 5. Resolução de problemas

| Sintoma | Causa provável | Correção |
|---|---|---|
| Deploy não arranca | Runner ausente ou com etiqueta errada | Registar runner neste repo com `iis-propertymanager` |
| `Missing deployment value for ...` | Falta um GitHub Secret | Criar o segredo indicado e repetir o CD |
| ANCM erro 1034 / config `aspNetCore` | Elemento errado no `web.config` | Usar `environmentVariable` (já implementado no script) |
| API/UI `503` | Pool parado | O `finally` do script arranca os pools; confirmar com `Get-WebAppPoolState` |
| `POST /Identity/Account/Login` → 500 | `Login failed for user 'IIS APPPOOL\...'` | Dar login SQL e `db_owner` ao pool (secção 3) |
| Cópia falha com DLL bloqueada | `w3wp.exe` ainda a correr | O script já espera pelo fim do processo e repete a cópia |
| Aviso de certificado no browser | Certificado self-signed | Instalar um certificado de confiança e definir `PM_UI_CERT_THUMBPRINT` |
| Mensagem da Syncfusion em produção | Segredo errado/expirado ou injetado no site errado | Ver [SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md) |
| `Access to the path ... Reports\Docs\...dotx is denied` ao gravar modelo | Pool da UI sem escrita na pasta de modelos da API | O CD já concede `Modify` aos dois pools; repetir o CD. Conferir com `icacls C:\inetpub\wwwroot\PropertyManagerFL\Api\Reports\Docs` |
| `Access to the path ... wwwroot\uploads\...` ao enviar/remover ficheiro | Pool da UI sem escrita em `UI\wwwroot\uploads` | Repetir o CD; verificar com `icacls C:\inetpub\wwwroot\PropertyManagerFL\UI\wwwroot\uploads` |
| `Access to the path ...` em `BackupDatabase` ou `BackupStaticFiles` | Caminho alterado em `ApplicationSettings` ou conta do SQL sem escrita | O CD lê `BackupBaseDados`/`BackupOutrosFicheiros` da BD e concede aos pools e ao serviço SQL; repetir o CD após alterar a configuração |

> Não alterar diretamente os `web.config` publicados para corrigir configuração: o próximo deploy sobrescreve-os. Alterar sempre o GitHub Secret correspondente e repetir o CD.
