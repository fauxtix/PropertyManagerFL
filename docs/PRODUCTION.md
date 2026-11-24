# Produção — deploy IIS, segredos e verificação

> **TL;DR (EN):** Production uses GitHub Secrets (never committed). Deploy with `gh workflow run cd.yml --repo fauxtix/PropertyManagerFL --ref master`; it needs a self-hosted runner registered in **this repo** with label `iis-propertymanager`. The script injects secrets into the published `web.config` as environment variables (Production does not load user-secrets). Two Syncfusion secrets are required: `PM_SYNCFUSION_LICENSE` (UI) and `PM_SYNCFUSION_LICENSE_API` (API). Pools `PropertyManagerApi` and `PropertyManagerFL` already have SQL logins/`db_owner`.

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
| `PM_PMCONNECTION` | Connection string SQL (`ConnectionStrings__PMConnection`) |
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
- Os valores em `appsettings*.json` são apenas placeholders.

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

## 4. Verificação após o deploy

```powershell
# API: 401 sem chave, 200 com chave
curl.exe -k -s -o NUL -w '%{http_code}' http://localhost:4300/api/Notifications/alerts
curl.exe -k -s -o NUL -w '%{http_code}' -H 'ApiKey: <valor>' http://localhost:4300/api/Notifications/alerts
curl.exe -k -s -o NUL -w '%{http_code}' -H 'ApiKey: <valor>' http://localhost:4300/api/Notifications/unread-count

# Modelos de carta: tem de devolver nomes de ficheiros .dotx (nunca uma lista vazia)
curl.exe -k -s -H 'ApiKey: <valor>' http://localhost:4300/api/Templates/GetTemplatesFilenamesFromServer/pt-PT

# UI: 302 para /Identity/Account/Login
curl.exe -k -s -o NUL -w '%{http_code}' https://localhost:4310/

# Bindings registadas
netsh http show servicestate
```

Esperado: `401`, `200`, `200`, `302` e as entradas `HTTP://*:4300/` e `HTTPS://*:4310/`. O endpoint de modelos deve listar os `.dotx` (5 para `pt-PT`); lista vazia significa que a publicação não levou a pasta `Reports\Docs`.

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

> Não alterar diretamente os `web.config` publicados para corrigir configuração: o próximo deploy sobrescreve-os. Alterar sempre o GitHub Secret correspondente e repetir o CD.
