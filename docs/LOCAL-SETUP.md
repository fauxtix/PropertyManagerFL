# Configuração local (.NET 10)

> **TL;DR (EN):** Prerequisites: .NET 10 SDK (`global.json`), SQL Server Developer/Community with `PropertyManagerDB` created ([DATABASE.md](DATABASE.md)), SSMS/`sqlcmd`, your own Syncfusion keys ([SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md)) and SMTP settings. Set secrets **per project** with `dotnet user-secrets set --project ...`, set `ASPNETCORE_ENVIRONMENT=Development`, then run the API (`https://localhost:5001`) and the UI (`https://localhost:7181`) in two terminals. These are the original **Kestrel** ports and are **different from the IIS/Production ports (4300/4310)**, so both can run on the same machine. GitHub Secrets are **not** copied by a clone.

## 1. Pré-requisitos

- [ ] **.NET 10 SDK** — a versão está fixada em `global.json` (`10.0.404`, `rollForward: latestPatch`). Verificar com `dotnet --version`.
- [ ] **SQL Server Developer ou Community** com a base de dados `PropertyManagerDB` criada — ver [DATABASE.md](DATABASE.md).
- [ ] **SSMS** ou `sqlcmd` para gerir a base de dados.
- [ ] **Certificado HTTPS de desenvolvimento**: `dotnet dev-certs https --trust`.
- [ ] **Conta Syncfusion e chaves próprias** para a UI e para a API — ver [SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md).
- [ ] **Credenciais SMTP** (a UI exige a secção `EmailConfiguration` no arranque; valores de exemplo servem se não for testar envio de correio).
- [ ] **Git** e, opcionalmente, **GitHub CLI (`gh`)**.

> Um `git clone` **não copia** os GitHub Secrets do repositório nem os user-secrets do mantenedor. Cada pessoa precisa da sua BD, das suas credenciais e das suas licenças.

## 2. Portas e configuração incluída

| Serviço | URL | Configuração |
|---|---|---|
| API (Development, Kestrel) | `https://localhost:5001;http://localhost:5000` | perfil `PM.WebApi` em `Properties/launchSettings.json` |
| UI (Development, Kestrel) | `https://localhost:7181;http://localhost:5152` | perfil da UI em `Properties/launchSettings.json` |
| API chamada pela UI (Development) | `https://localhost:5001/api` | `BaseUrl` em `appsettings.Development.json` |
| API (Production/IIS) | `http://localhost:4300` | binding HTTP do site `PropertyManagerFL.Api` |
| UI (Production/IIS) | `https://localhost:4310` | binding HTTPS do site `PropertyManagerFL.UI` |
| API chamada pela UI (Production) | `http://localhost:4300/api` | `BaseUrl` em `appsettings.Production.json` |

Os perfis **IIS Express** (`5010`/`44301` na API, `31859`/`44350` na UI) ficam em `iisSettings` e só são usados se selecionar "IIS Express" no Visual Studio. `dotnet run` e o arranque da solução usam os perfis Kestrel acima.

O `BaseUrl` já vem nos ficheiros de configuração e é usado pelos `ApiWrappers/*`, pelo cliente SignalR (`WrapperNotifications.HubUrl`) e pelo upload de ficheiros (`FileUploadComponent.razor`). Não precisa de o alterar, a menos que mude as portas.

> **Não use 4300/4310 em Development.** Essas portas pertencem ao IIS em Produção. Se o arranque falhar com `Failed to bind to address http://localhost:4300`, o perfil de launch ou o `BaseUrl` ficou desatualizado — verifique os ficheiros acima. A mensagem vem do Kestrel; o `4300` aparece porque a config de Dev estava com a porta do IIS, não porque o ambiente seja Production.

**A API e a UI têm de estar a correr ao mesmo tempo.**

## 3. Segredos locais (user-secrets)

Os valores em `appsettings*.json` são placeholders. Cada projeto tem um `UserSecretsId` diferente, por isso **tem de definir os valores por projeto**:

```powershell
dotnet user-secrets set --project PropertyManagerFL.Api "<chave>" "<valor>"
dotnet user-secrets set --project PropertyManagerFL.UI  "<chave>" "<valor>"
```

### API — `PropertyManagerFL.Api`

| Chave | Exemplo / nota |
|---|---|
| `ConnectionStrings:PMConnection` | `Data Source=.\SQLEXPRESS;Initial Catalog=PropertyManagerDB;Integrated Security=True;TrustServerCertificate=True;Encrypt=False` — usada pelo EF **e** pelo Serilog; só necessário se a sua instância diferir do valor em `appsettings.Development.json` |
| `ApiKey` | valor local à sua escolha; tem de ser **igual** ao da UI |
| `SyncfusionLicenseKey` | a sua chave Syncfusion da API — ver [SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md) |
| `JWT:Secret` | mínimo 32 caracteres, não pode começar por `<`; a API valida no arranque |
| `JWT:Issuer` | por exemplo `https://localhost:5001/` |
| `JWT:Audience` | por exemplo `https://localhost:5001/` |
| `JWT:ExpiraEmMinutos` | por exemplo `120` |

> `appsettings.Development.json` contém valores de exemplo no `JWT:Issuer`/`JWT:Audience` e um nome de máquina antigo na connection string. Os user-secrets sobrepõem-nos. Defina-os sempre.

Exemplo (substituir os valores entre `<...>`):

```powershell
dotnet user-secrets set --project PropertyManagerFL.Api "ConnectionStrings:PMConnection" "<connection-string-da-aplicacao>"
dotnet user-secrets set --project PropertyManagerFL.Api "ApiKey" "<api-key-local>"
dotnet user-secrets set --project PropertyManagerFL.Api "SyncfusionLicenseKey" "<chave-syncfusion-da-api>"
dotnet user-secrets set --project PropertyManagerFL.Api "JWT:Secret" "<segredo-com-32-caracteres-ou-mais>"
dotnet user-secrets set --project PropertyManagerFL.Api "JWT:Issuer" "https://localhost:5001/"
dotnet user-secrets set --project PropertyManagerFL.Api "JWT:Audience" "https://localhost:5001/"
dotnet user-secrets set --project PropertyManagerFL.Api "JWT:ExpiraEmMinutos" "120"
```

### UI — `PropertyManagerFL.UI`

| Chave | Exemplo / nota |
|---|---|
| `ApiKey` | **mesmo valor** da API; é o cabeçalho `ApiKey` que o `ApiKeyMiddleware` exige |
| `SyncfusionLicenseKey` | a sua chave Syncfusion da UI — pode ser igual à da API se a chave cobrir todas as edições |
| `EmailConfiguration:Username` | utilizador SMTP |
| `EmailConfiguration:SmtpServer` | servidor SMTP |
| `EmailConfiguration:Port` | porta SMTP, por exemplo `587` |
| `EmailConfiguration:Password` | palavra-passe SMTP / app password |
| `EmailConfiguration:From` | remetente |
| `ConnectionStrings:PMConnection` | opcional; só se a instância SQL diferir do `appsettings.Development.json` |

```powershell
dotnet user-secrets set --project PropertyManagerFL.UI "ApiKey" "<mesma-api-key-local>"
dotnet user-secrets set --project PropertyManagerFL.UI "SyncfusionLicenseKey" "<chave-syncfusion-da-ui>"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:Username" "<utilizador-smtp>"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:SmtpServer" "<servidor-smtp>"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:Port" "<porta-smtp>"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:Password" "<palavra-passe-smtp>"
dotnet user-secrets set --project PropertyManagerFL.UI "EmailConfiguration:From" "<remetente>"
```

> O Serilog está configurado em **código** para gravar na tabela `dbo.PMLogs` da **`PropertyManagerDB`**, usando `ConnectionStrings:PMConnection` (tanto na API como na UI). As secções `Serilog` dos `appsettings`/user-secrets são ignoradas; a tabela é criada automaticamente (`AutoCreateSqlTable = true`). A conta que executa a aplicação precisa de permissões de escrita na BD.

## 4. Arrancar a aplicação

Em **dois terminais**, a partir da raiz do repositório:

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project PropertyManagerFL.Api
```

```powershell
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project PropertyManagerFL.UI
```

Em `Production` os user-secrets **não** são carregados — ver [PRODUCTION.md](PRODUCTION.md).

### Verificação rápida

```powershell
# API protegida pelo cabeçalho ApiKey (não é X-ApiKey)
curl.exe -k -H "ApiKey: <api-key-local>" https://localhost:5001/api/Imoveis/TableHasData
curl.exe -k -H "ApiKey: <api-key-local>" https://localhost:5001/api/Notifications/alerts

# Modelos de carta (devem devolver ficheiros .dotx, não uma lista vazia)
curl.exe -k -H "ApiKey: <api-key-local>" https://localhost:5001/api/Templates/GetTemplatesFilenamesFromServer/pt-PT

# UI deve redirecionar para a página de login
curl.exe -k -I https://localhost:7181/
```

Esperado: API `200` (com `true` no primeiro endpoint) e UI `302` para `Identity/Account/Login`. O endpoint de modelos deve devolver nomes de ficheiros `.dotx`; uma lista vazia significa que a API não encontrou `Reports\Docs` no content root. Depois de autenticar, o sino de notificações aparece no layout e a caixa de entrada em `/notifications`: cada aviso pode incluir uma sugestão de ação e os alertas cobrem rendas em atraso, cartas em falta, rendas a vencer, cartas sem resposta e contratos sem documento.

Se a base de dados foi criada antes desta alteração, aplicar primeiro o pequeno `ALTER` da coluna `SuggestedAction` indicado em [DATABASE.md](DATABASE.md).

## 5. Idiomas, temas e documentos

- **Idiomas:** a UI suporta `en-US` (culture por omissão), `pt-PT`, `es` e `fr` (`Program.cs`, `UseRequestLocalization`). `AppSettings:Language` em `appsettings.Development.json` é `pt-PT`. As traduções da interface estão em `PropertyManagerFL.UI/Resources/App.*.resx` e as dos componentes Syncfusion em `SfResources.*.resx`.
- **Dados de lookup:** as tabelas criadas por `LookupTablesWithData.sql` estão em português. Para outro idioma, adaptar os dados na BD.
- **Temas:** existe um interruptor claro/escuro (`Components/ThemeToggle.razor`, `Services/ThemeStateService.cs`, `wwwroot/js/themeToggle.js`). O **dark** é o tema por omissão; a escolha fica em `localStorage`. Os estilos usam `wwwroot/css/theme-tokens.css` e os temas Syncfusion `bootstrap5.css` / `bootstrap5-dark.css` em `Pages/_Layout.cshtml`.
- **Cartas/contratos:** os modelos `.dotx` estão em `PropertyManagerFL.Api/Reports/Docs` e são essenciais em **Development e Production**. A API lê-os de `ContentRootPath/reports/docs`: em Dev são os ficheiros do projeto; em Prod são copiados para a publicação pela regra `CopyToOutputDirectory` no `PropertyManagerFL.Api.csproj` e verificados pelo CI/CD. Ao traduzir, preservar os *placeholders* dos campos variáveis (por exemplo `«NomeInquilino»`).

## 6. Resolução de problemas

| Sintoma | Causa provável | O que fazer |
|---|---|---|
| API devolve `401` | Falta o cabeçalho `ApiKey` ou o valor difere da UI | Usar `ApiKey: <valor>`; confirmar o segredo nos dois projetos |
| API não arranca: `JWT secret must be set...` | Falta `JWT:Secret` ou começa por `<` | Definir um segredo com 32+ caracteres |
| UI não arranca: `Email configuration is missing or invalid` | Falta a secção `EmailConfiguration` | Definir as cinco chaves `EmailConfiguration:*` |
| Mensagem da Syncfusion | Chave ausente/inválida/errada para a versão | Ver [SYNCFUSION-LICENSES.md](SYNCFUSION-LICENSES.md) |
| Erro `Login failed for user ...` | A conta Windows/app pool não tem acesso ao SQL | Ver a secção de IIS em [DATABASE.md](DATABASE.md) e [PRODUCTION.md](PRODUCTION.md) |
| `dotnet dev-certs` / aviso de certificado | Certificado de desenvolvimento não confiável | `dotnet dev-certs https --clean` e depois `--trust` |
| `Failed to bind to address http://localhost:4300` em Dev | O `applicationUrl` do perfil Kestrel ficou com a porta do IIS | Confirmar `5001`/`5000` na API e `7181`/`5152` na UI, além do `BaseUrl` de Development |
| Porta ocupada | Outra aplicação está a usar 5001/5000/7181/5152 | Verificar com `netstat -ano | findstr :5001` |
| Editor de cartas mostra "sem modelos" | A API não encontrou `Reports\Docs` no content root | Confirmar os `.dotx` na pasta da API e o endpoint `GetTemplatesFilenamesFromServer` |

## 7. Testes e deploy

- Testes: `dotnet test PropertyManagerFL.Tests/PropertyManagerFL.Tests.csproj`.
- CI: `.github/workflows/ci.yml` (restore/build/test, sem BD).
- Deploy IIS: ver [PRODUCTION.md](PRODUCTION.md).
