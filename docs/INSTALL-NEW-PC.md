# Instalar noutro PC para testar (guia único)

> **TL;DR (EN):** Clone the repo, install the .NET 10 SDK (`global.json`), SQL Server Developer/Express + SSMS/`sqlcmd`, trust the dev HTTPS certificate and get **your own Syncfusion 35.2.x key**. Create an empty `PropertyManagerDB` first, then install **one** schema path (the generated `SqlData_Script/PropertyManagerDB.sql` is recommended for a clean PC). Set user-secrets **per project** (API and UI), point the UI's `EmailConfiguration` at a local [smtp4dev](https://github.com/rnwood/smtp4dev) so registration works, register your own user at `/Identity/Account/Register` and confirm it via the link shown on the confirmation page. Set `ASPNETCORE_ENVIRONMENT=Development`, run the API (`https://localhost:5001`) **and** the UI (`https://localhost:7181`) in two terminals. IIS/CD is optional and needs IIS + the .NET Hosting Bundle + a self-hosted runner registered **in your repo** with label `iis-propertymanager`; the CD publishes apps only and never touches SQL. GitHub Secrets, user-secrets, licences and databases are **not** copied by a clone.

Este é o guia de entrada para **instalar a aplicação num PC limpo e testá-la**. Está escrito em ordem de execução; os guias mais detalhados estão no fim de cada secção. O caminho recomendado é **Development/Kestrel** (§1–§6). O IIS/CD é opcional (§7).

## 0. O que o `git clone` não traz

| Item | Por quem é criado |
|---|---|
| GitHub Secrets (`PM_API_KEY`, Syncfusion, JWT, e-mail, …) | Cada pessoa, nas *Settings* do seu repo/fork |
| user-secrets da API e da UI | Cada pessoa, com `dotnet user-secrets set --project ...` |
| Base de dados e os seus dados | Cada pessoa, manualmente (não há EF migrations ativas) |
| Certificado HTTPS de desenvolvimento | Cada PC, com `dotnet dev-certs https --trust` |
| Licenças Syncfusion | Cada pessoa, com a sua própria conta (Community/Enterprise) |
| IIS, app pools, logins SQL e segredos de deploy | Cada servidor; o CD publica ficheiros, não instala nem configura o IIS |

> O `ApiKey` na linha de `ApplicationSettings` da BD é um valor sanitizado **genérico**. O cabeçalho `ApiKey` exigido pelo `ApiKeyMiddleware` vem dos **user-secrets/Secrets** e tem de ser igual na API e na UI — ver §4 e §3.

## 1. Pré-requisitos

| Software | Development (recomendado) | IIS/CD (opcional) |
|---|---|---|
| **.NET 10 SDK** | Obrigatório — `global.json` fixa `10.0.404` (`rollForward: latestPatch`). `dotnet --version` | O runner instala o SDK a partir do `global.json` |
| **SQL Server Developer, Community ou Express** | Obrigatório; instância local, qualquer nome. O projeto foi desenvolvido com `SQLEXPRESS` | Opcional; a BD de Produção pode estar noutra máquina |
| **SSMS** ou `sqlcmd` | Obrigatório para criar a BD e correr os scripts | `sqlcmd` no servidor, se a migração for manual |
| **Certificado HTTPS dev** | `dotnet dev-certs https --trust` | Não aplicável (o CD usa certificado no `LocalMachine\My`) |
| **Conta Syncfusion própria** | Chave **35.2.x com todas as edições**, para UI **e** API — [SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md) | Dois GitHub Secrets, idealmente com o mesmo valor |
| **SMTP de teste** | [smtp4dev](https://github.com/rnwood/smtp4dev) (gratuito) ou credenciais reais — necessário para registar o primeiro utilizador (§5) | `PM_EMAIL_*` nos Secrets |
| **Git** / **GitHub CLI (`gh`)** | Obrigatório / recomendado | `gh` para disparar o CD |
| **IIS + .NET Hosting Bundle (ANCM)** | Não necessário | Obrigatório **antes** do primeiro deploy: o `deploy-iis.ps1` cria sites/pools, mas **não instala** o IIS nem o Hosting Bundle. O script atual não usa URL Rewrite. |
| **Runner self-hosted Windows** | Não necessário | Registado **neste repo/fork** com o label `iis-propertymanager`; `shell: powershell` (não `pwsh`) |

Notas:

- Em SQL Server **Express**, `BACKUP DATABASE ... WITH COMPRESSION` falha com `Msg 1844` — usar backup sem compactação.
- As portas de Development são `5001/5000` (API) e `7181/5152` (UI). As portas `4300` (API) e `4310` (UI) pertencem ao IIS; não as usar em Development. Assim os dois ambientes podem coexistir no mesmo PC.
- Se o PC tiver o Visual Studio com as DLLs bloqueadas, um `dotnet build` pode falhar com MSB3027; não é problema de código — fechar o VS ou usar outro `OutputPath`.

## 2. Clonar o repositório

```powershell
git clone https://github.com/fauxtix/PropertyManagerFL.git
cd PropertyManagerFL
dotnet --version          # tem de ser 10.0.x
dotnet restore PropertyManagerFL.sln
```

Se for trabalhar num **fork**, ajustar todos os comandos `gh --repo fauxtix/PropertyManagerFL` para `--repo <o-seu-utilizador>/PropertyManagerFL` e registar o runner e os Secrets **no fork**. O `BaseUrl` de Development já aponta para `https://localhost:5001/api`; só o alterar se mudar as portas.

## 3. Base de dados

### 3.1 Criar a BD vazia

**Nenhum** script do repositório contém `CREATE DATABASE` ou `DROP DATABASE` — criar primeiro, no SSMS ou com `sqlcmd`:

```sql
CREATE DATABASE [PropertyManagerDB];
GO
```

### 3.2 Instalar o esquema — escolher **um** caminho

**Recomendado num PC novo: snapshot gerado.** É o mesmo esquema dos scripts modulares, já inclui a migração de fim de contrato/arquivamento e as notificações, e não traz dados operacionais:

```powershell
sqlcmd -S .\SQLEXPRESS -E -i SqlData_Script\PropertyManagerDB.sql -b
```

Alternativa: executar os scripts modulares pela ordem indicada em [DATABASE.md](DATABASE.md) (`WorkingTables.sql`, `LookupTablesWithData.sql`, `UserDefinedFunctions.sql`, `StoredProcedures.sql`, `Views.sql`). **Nunca executar ambos os caminhos na mesma BD.** O backup antigo `23012024_160350.Bak` **não é recomendado** para máquina nova: é antigo, pode conter dados reais e não garante os objetos atuais.

### 3.3 Verificar

```sql
SELECT (SELECT COUNT(*) FROM sys.tables  WHERE is_ms_shipped=0) AS Tabelas,      -- 79
       (SELECT COUNT(*) FROM sys.views   WHERE is_ms_shipped=0) AS Vistas,       -- 6
       (SELECT COUNT(*) FROM sys.objects WHERE type IN ('FN','IF','TF') AND is_ms_shipped=0) AS Funcoes, -- 2
       (SELECT COUNT(*) FROM sys.procedures WHERE is_ms_shipped=0) AS Procedures; -- 297
```

### 3.4 Valores sanitizados do snapshot

A linha de `dbo.ApplicationSettings` traz *placeholders*. Ajustar apenas o necessário para o teste local (substituir os caminhos pelos seus; não usar estes valores em Produção):

```sql
UPDATE dbo.ApplicationSettings
SET ApiKey = N'<valor-local-a-escolher>',
    BackupBaseDados = N'C:\Backups\PropertyManagerFL\Backups\BaseDados',
    BackupOutrosFicheiros = N'C:\Backups\PropertyManagerFL\Backups\Ficheiros'
WHERE Id = 1;
```

- O `ApiKey` acima é um campo de configuração da aplicação; **não** é a chave do middleware. A chave do cabeçalho `ApiKey` é a dos user-secrets (§4) e tem de ser igual na API e na UI.
- Os campos SMTP da BD ficam `NULL`; a UI lê `EmailConfiguration` da configuração/segredos (§4), não dessa linha.
- O snapshot inclui a conta `admin@propertymanager.pt`, mas a palavra-passe não é documentada e não deve ser reutilizada. Criar o seu próprio utilizador pelo UI (§5).
- O snapshot **não** contém imóveis, inquilinos, contratos nem notificações. Criar dados de teste com nomes fictícios.
- Se a BD já existir de uma versão anterior, **não** executar o snapshot: fazer backup e aplicar a migração idempotente `EndLeaseAndArchiving.sql` (ver [DATABASE.md](DATABASE.md), §4).

## 4. Segredos de Development

Os valores em `appsettings*.json` são placeholders. Cada projeto tem um `UserSecretsId` diferente, por isso **definir por projeto**:

### API

```powershell
dotnet user-secrets set --project PropertyManagerFL.Api "ConnectionStrings:PMConnection" "Data Source=.\SQLEXPRESS;Initial Catalog=PropertyManagerDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=False"
dotnet user-secrets set --project PropertyManagerFL.Api "ApiKey" "<api-key-local>"
dotnet user-secrets set --project PropertyManagerFL.Api "SyncfusionLicenseKey" "<chave-syncfusion>"
dotnet user-secrets set --project PropertyManagerFL.Api "JWT:Secret" "<segredo-com-32-caracteres-ou-mais>"
dotnet user-secrets set --project PropertyManagerFL.Api "JWT:Issuer" "https://localhost:5001/"
dotnet user-secrets set --project PropertyManagerFL.Api "JWT:Audience" "https://localhost:5001/"
dotnet user-secrets set --project PropertyManagerFL.Api "JWT:ExpiraEmMinutos" "120"
```

`JWT:Secret` é validado no arranque (32+ caracteres, não pode começar por `<`). A connection string só é necessária se a sua instância diferir do `appsettings.Development.json`.

### UI

```powershell
dotnet user-secrets set --project PropertyManagerFL.UI "ApiKey" "<mesma-api-key-local>"
dotnet user-secrets set --project PropertyManagerFL.UI "SyncfusionLicenseKey" "<chave-syncfusion>"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:SmtpServer" "localhost"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:Port" "2525"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:Username" "dev"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:Password" "dev"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:From" "dev@localhost"
dotnet user-secrets set --project PropertyManagerFL.UI "ConnectionStrings:PMConnection" "Data Source=.\SQLEXPRESS;Initial Catalog=PropertyManagerDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=False"
```

`ApiKey` da UI tem de ser **exatamente igual** ao da API. A secção `EmailConfiguration` é obrigatória no arranque; os valores acima apontam para o smtp4dev (§5). A Syncfusion deve ser a **mesma chave válida 35.2.x** nos dois projetos, com todas as edições.

> Em `Production` os user-secrets **não** são carregados — ver §7 e [PRODUCTION.md](PRODUCTION.md).

## 5. Primeiro utilizador e primeiro login

A UI exige confirmação de e-mail (`RequireConfirmedAccount = true`). O registo chama `EmailSender.SendEmailAsync` **antes** de mostrar a página de confirmação, por isso é preciso um SMTP que funcione — mesmo que a página mostre o link de confirmação. Para testar sem credenciais reais, usar o smtp4dev.

### 5.1 SMTP local (recomendado)

```powershell
winget install Rnwood.Smtp4dev        # ou: dotnet tool install -g Rnwood.Smtp4dev
$env:ServerOptions__Urls = 'http://localhost:5002'   # 5000 está ocupado pela API
$env:ServerOptions__Port = '2525'
$env:ServerOptions__AuthenticationRequired = 'true'
$env:ServerOptions__SmtpAllowAnyCredentials = 'true'
smtp4dev
```

Deixar esta janela aberta. O interface web do smtp4dev fica em `http://localhost:5002`. As variáveis `AuthenticationRequired`/`SmtpAllowAnyCredentials` fazem o servidor anunciar AUTH e aceitar as credenciais fictícias `dev`/`dev` usadas na §4 (o MailKit da aplicação autentica-se sempre).

### 5.2 Registar e confirmar

1. Abrir `https://localhost:7181/` → **Registo** (ou `https://localhost:7181/Identity/Account/Register?returnUrl=%2F`).
2. Introduzir um e-mail e palavra-passe de teste (mínimo 6 caracteres) e submeter.
3. A página **Register confirmation** mostra o link **Click here to confirm your account** (o mesmo link fica também na caixa do smtp4dev). Clicar nesse link.
4. Fazer login com o e-mail e a palavra-passe do passo 2.

Se o registo devolver `500`, o SMTP não está acessível ou não anuncia AUTH — verificar o smtp4dev e as cinco chaves `EmailConfiguration:*`. Se o link expirar, atualizar a página de confirmação: um novo link é gerado. **Não** editar a BD para marcar `EmailConfirmed=1`: isso não evita o erro no passo 2 e deixa o utilizador sem e-mail de confirmação coerente. A conta `admin@propertymanager.pt` do snapshot serve apenas como referência; a palavra-passe não é conhecida — criar a sua.

Para enviar mensagens/cartas pela UI depois do login, manter o smtp4dev a correr ou substituir os segredos por um SMTP real que aceite login.

## 6. Arranque e smoke de Development

Em **dois terminais**, a partir da raiz do repositório:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project PropertyManagerFL.Api
```

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project PropertyManagerFL.UI
```

A API e a UI **têm de estar a correr ao mesmo tempo**. Verificação rápida:

```powershell
curl.exe -k -s -o NUL -w '%{http_code}' -H "ApiKey: <api-key-local>" https://localhost:5001/api/Imoveis/TableHasData
curl.exe -k -s -o NUL -w '%{http_code}' -H "ApiKey: <api-key-local>" https://localhost:5001/api/Notifications/alerts
curl.exe -k -s -H "ApiKey: <api-key-local>" https://localhost:5001/api/Templates/GetTemplatesFilenamesFromServer/pt-PT
curl.exe -k -s -o NUL -w '%{http_code}' -I https://localhost:7181/
```

Esperado: `200`, `200`, lista de ficheiros `.dotx` e `302` para `Identity/Account/Login`.

A caixa de entrada e o contador de notificações exigem, além do `ApiKey`, o cabeçalho `X-User-Id` com o `Id` de um utilizador em `dbo.AspNetUsers` (a UI envia-o automaticamente; só é necessário em chamadas `curl`):

```powershell
curl.exe -k -s -o NUL -w '%{http_code}' -H "ApiKey: <api-key-local>" -H "X-User-Id: <id-do-utilizador>" https://localhost:5001/api/Notifications/unread-count
curl.exe -k -s -H "ApiKey: <api-key-local>" -H "X-User-Id: <id-do-utilizador>" "https://localhost:5001/api/Notifications?take=1"
```

Para obter o id na sua BD de teste: `SELECT Id, Email FROM dbo.AspNetUsers;`. Esperado: `200` nos dois endpoints; sem `X-User-Id` devolvem `400`.

Depois do login, fazer um smoke não destrutivo: sino e caixa de entrada, criar um imóvel/fração/inquilino fictício, registar uma renda, gerar uma carta/PDF e carregar um ficheiro. Para testar **Terminar contrato** e **Arquivar**, criar registos descartáveis: o contrato ativo bloqueia o arquivamento e a dívida não bloqueia o fim do contrato (gera apenas alerta). Ao arrancar, a API cria os alertas pendentes de todos os utilizadores — não é preciso povoar notificações à mão.

Mais detalhes: [LOCAL-SETUP.md](LOCAL-SETUP.md) e [DATABASE.md](DATABASE.md).

## 7. Opcional: IIS/CD no PC novo

1. **Instalar antes do deploy:** IIS, **.NET Hosting Bundle/ANCM** (o `web.config` publicado usa `system.webServer/aspNetCore`) e o SDK .NET 10 no runner. O `deploy-iis.ps1` cria sites/pools se faltarem, mas não instala IIS nem o hosting bundle e não usa URL Rewrite.
2. **Registar o runner** no mesmo repo/fork com `[self-hosted, Windows, iis-propertymanager]`. O serviço corre como `SYSTEM` (ou equivalente com permissão para IIS e certificados em `LocalMachine\My`). Usa Windows PowerShell.
3. **Configurar os GitHub Secrets** listados em [PRODUCTION.md](PRODUCTION.md): `PM_API_KEY`, `PM_SYNCFUSION_LICENSE`, `PM_SYNCFUSION_LICENSE_API`, `PM_PMCONNECTION`, `PM_JWT_*`, `PM_EMAIL_*` e, opcionalmente, `PM_UI_CERT_THUMBPRINT`. `PM_PMCONNECTION` serve só ao script de deploy para ACLs; a connection string das apps vem de `appsettings.Production.json` → `PropertyManagerDB_Production`.
4. **Preparar a BD antes do código:** criar `PropertyManagerDB_Production` vazia e instalar o esquema pela §3, **ou** fazer backup e aplicar apenas `EndLeaseAndArchiving.sql` se a BD já existir. Pools novos não têm login SQL: executar `Login_App.sql`, `Login_Api.sql` e `UsersAndSchemas.sql` e dar `db_owner`, ou usar os nomes `PropertyManagerApi` / `PropertyManagerFL` já configurados.
5. **Disparar o CD** (num fork, trocar `--repo`):

   ```powershell
   gh workflow run cd.yml --repo fauxtix/PropertyManagerFL --ref master
   ```

   A ordem é sempre **BD → CD → verificação**. O CD nunca cria, larga nem migra bases de dados.
6. **Verificar** com os comandos da secção 4 de [PRODUCTION.md](PRODUCTION.md): API `401` sem chave e `200` com chave, `unread-count`/inbox `200` com `X-User-Id`, `302` na UI, 5 modelos `.dotx`, bindings `HTTP://*:4300/` e `HTTPS://*:4310/`. Não executar fim de contrato/arquivamento em dados reais sem autorização e backup.

> Primeiro login em IIS/Produção: o mesmo fluxo da §5, mas com o SMTP real configurado nos Secrets `PM_EMAIL_*` (ou um smtp4dev acessível ao servidor). A UI de Produção também exige e-mail confirmado.

## 8. Checklist / problemas comuns

- [ ] `dotnet --version` → 10.0.x; `dotnet dev-certs https --trust`
- [ ] BD `PropertyManagerDB` criada **antes** dos scripts; contagens 79 / 6 / 2 / 297
- [ ] `ApiKey` igual na API e na UI; `JWT:Secret` com 32+ caracteres
- [ ] Chave Syncfusion 35.2.x válida nos dois projetos
- [ ] smtp4dev a correr na porta 2525 (web em 5002) durante registo/envio de mensagens
- [ ] Utilizador registado **e** confirmado; login feito com sucesso
- [ ] API + UI a correr em simultâneo nas portas 5001 / 7181
- [ ] Smoke com `ApiKey` e `X-User-Id` concluído

| Sintoma | Causa provável | Correção |
|---|---|---|
| `401` na API | Falta o cabeçalho `ApiKey` ou valores diferentes | Confirmar os dois user-secrets; o header é `ApiKey`, não `X-ApiKey` |
| API não arranca: `JWT secret must be set...` | `JWT:Secret` ausente/curto/começa por `<` | Definir 32+ caracteres |
| UI não arranca: `Email configuration is missing or invalid` | Faltam chaves `EmailConfiguration:*` | Definir as cinco chaves (§4) |
| Registo devolve `500` | SMTP inacessível ou sem AUTH | Iniciar smtp4dev com as variáveis da §5.1; confirmar `Port=2525` na UI |
| Página de confirmação sem link | Página errada ou link expirado | Voltar a `/Identity/Account/Register` ou recarregar a confirmação |
| `/Notifications/unread-count` → `400` | Falta `X-User-Id` | Usar o `Id` de `dbo.AspNetUsers` (§6) |
| Aviso/ marca de água Syncfusion | Chave inválida, de outra versão ou só num projeto | [SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md) |
| `Failed to bind ... 4300` em Dev | Perfil Kestrel/BaseUrl com porta do IIS | Verificar 5001/5000 e 7181/5152 e `BaseUrl` de Development |
| `Login failed for user 'IIS APPPOOL\...'` | Pool novo sem login SQL | Executar `Login_*.sql` + `UsersAndSchemas.sql` (§7) |
| Deploy não arranca / não há runner | Runner não registado neste repo ou label errada | Registar com `iis-propertymanager` no repo/fork correto |

## Referências

- [DATABASE.md](DATABASE.md) — esquema, ordem dos scripts, migração de BDs existentes
- [LOCAL-SETUP.md](LOCAL-SETUP.md) — portas, segredos, temas/idiomas, resolução de problemas
- [SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md) — versão/edição da chave
- [PRODUCTION.md](PRODUCTION.md) — IIS, GitHub Secrets, verificação pós-deploy
- [smtp4dev](https://github.com/rnwood/smtp4dev) — servidor SMTP de teste
