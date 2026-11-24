# Licenças Syncfusion — qual chave escolher e onde a colocar

> **TL;DR (EN):** Choose a key for the **same version as the NuGet packages (currently 35.2.12 → select 35.2.x)** and for the **edition(s)** used by this repo (Community/Enterprise: select all five editions). In Development set `SyncfusionLicenseKey` **per project** with `dotnet user-secrets`. In Production use the two GitHub Secrets `PM_SYNCFUSION_LICENSE` (UI) and `PM_SYNCFUSION_LICENSE_API` (API). Keys are version-specific: regenerate them when you upgrade Syncfusion. Never commit a key.

## Resumo em 4 pontos

1. **Versão:** a chave tem de ser da mesma *versão* dos pacotes Syncfusion instalados — hoje **35.2.12**. Escolher **35.2.x** no site da Syncfusion (não "a mais recente", nem 24.x/30.x).
2. **Edição:** a partir da v31.1.17 as chaves são por **edição**, não por plataforma. Neste projeto selecione **todas as edições/SDKs disponíveis** (equivalente a *Enterprise*): UI Component Suite, Document SDK, DOCX Editor SDK, PDF Viewer SDK e Spreadsheet Editor SDK.
3. **Onde:** em Development, `SyncfusionLicenseKey` em **cada projeto** (a UI e a API registam a chave no arranque); em Production, os segredos `PM_SYNCFUSION_LICENSE` (UI) e `PM_SYNCFUSION_LICENSE_API` (API).
4. **Quem faz clone:** precisa da **sua própria licença**. Os GitHub Secrets e os user-secrets do mantenedor não são copiados pelo clone.

## 1. Porque é que há duas chaves neste repositório?

A UI e a API registam a licença separadamente no arranque:

- `PropertyManagerFL.UI/Program.cs` → `SyncfusionLicenseProvider.RegisterLicense(...)` com `Configuration["SyncfusionLicenseKey"]`;
- `PropertyManagerFL.Api/Program.cs` → o mesmo registo.

Cada projeto tem o seu próprio `UserSecretsId`, pelo que os valores são independentes. Neste repositório, historicamente, a UI e a API ficaram com **chaves diferentes**; em 2026-11-24 a chave da API estava desatualizada (24.x) e foi alinhada com a chave válida da UI (35.2.x). O CD continua a usar dois segredos, mas **ambos devem conter o mesmo valor**:

| Ambiente | UI | API |
|---|---|---|
| Development | user-secrets do projeto `PropertyManagerFL.UI` | user-secrets do projeto `PropertyManagerFL.Api` |
| Production | GitHub Secret `PM_SYNCFUSION_LICENSE` | GitHub Secret `PM_SYNCFUSION_LICENSE_API` |

**Não é obrigatório ter duas chaves diferentes — e neste repositório as duas devem ser iguais.** Se você gerar uma única chave válida que cubra todas as edições usadas pelo projeto, use o mesmo valor na UI e na API (e nos dois segredos de produção). O que não pode acontecer é a API receber uma chave antiga/errada: uma chave 24.x não serve para assemblies 35.2.12 e provoca a marca de água `Created with a trial version of Syncfusion PDF library...` ao gerar PDFs. O inverso (UI receber a chave da API) provocou a mensagem `Claim your FREE account and get a key in less than a minute` em produção.

Regra prática: se o aviso aparece **só na UI**, corrigir a chave da UI; se aparece **só na API/geração de PDFs**, corrigir a chave da API.

## 2. Que versão escolher

A Syncfusion declara que as chaves são **version-specific**: uma chave de outro volume release não é válida para assemblies diferentes (por exemplo, uma chave 24.x não serve para assemblies 35.x). Os service packs dentro do mesmo volume (35.2.1 → 35.2.12) usam a mesma chave.

Confirmar a versão instalada antes de gerar a chave:

```powershell
dotnet list PropertyManagerFL.UI\PropertyManagerFL.UI.csproj package | findstr Syncfusion
dotnet list PropertyManagerFL.Api\PropertyManagerFL.Api.csproj package | findstr Syncfusion
```

Todos os projetos (`UI`, `Api` e `Infrastructure`) referenciam atualmente **35.2.12**. Portanto:

- escolher **35.2.x** (por exemplo 35.2.12, se o seletor permitir a versão exata);
- **não** escolher a versão mais recente do site se ainda não atualizou os pacotes NuGet;
- se atualizar os pacotes para um novo volume release (36.x, 37.x, …), **gerar nova chave para essa versão** e atualizar Development + Production antes de considerar a atualização concluída;
- manter todos os pacotes Syncfusion na **mesma versão** nos três `.csproj`.

## 3. Que edição / tipo de licença escolher

Desde a versão 31.1.17, a Syncfusion passou de chaves *por plataforma* (ASP.NET Core, Blazor, …) para chaves **por edição/SDK**:

| Edição/SDK | Porque é usada neste repo |
|---|---|
| **UI Component Suite** | Componentes Blazor da UI: Grid, Schedule, Charts, FileManager, DropDowns, Navigations, Notifications, WordProcessor, etc.; também `Syncfusion.Blazor.Data` na Infrastructure |
| **Document SDK** | `Syncfusion.DocIO.Net.Core`, `Syncfusion.DocIORenderer.Net.Core` (cartas/contratos em `.docx`) |
| **DOCX Editor SDK** | `Syncfusion.Blazor.WordProcessor` na UI |
| **PDF Viewer SDK** | `Syncfusion.Blazor.SfPdfViewer`, `Syncfusion.Pdf.Net.Core` |
| **Spreadsheet Editor SDK** | Incluído na licença Enterprise/Community; selecione-o para obter uma chave equivalente a *Enterprise* |

Na prática: **selecione as cinco opções** (ou a opção *Enterprise*), para que a mesma chave cubra todos os pacotes referenciados. Se a sua conta só permitir uma edição, gere uma chave por projeto com as edições que esse projeto usa (UI: todas; API: Document SDK + PDF Viewer SDK).

### Licença gratuita (Community)

Quem faz clone pode pedir a **Community License**, gratuita e válida enquanto os critérios se mantiverem. Segundo a Syncfusion (condições podem mudar — confirmar na página oficial):

- receita bruta anual inferior a **1 000 000 USD**;
- **5 ou menos programadores** e **10 ou menos funcionários** no total;
- não ter recebido mais de **3 000 000 USD** de capital externo;
- entidades governamentais financiadas por fundos públicos não são elegíveis.

Pedido: <https://www.syncfusion.com/products/communitylicense> → **Claim Free License**. Depois de aprovado:

1. Abrir <https://www.syncfusion.com/account/downloads>.
2. Clicar em **Get License Key**.
3. Escolher a versão **35.2.x** e as edições indicadas acima (todas).
4. Copiar a chave gerada — é uma string longa, codificada em Base64.

Referências oficiais: [Which version License Key should I use?](https://support.syncfusion.com/kb/article/7865/which-version-license-key-should-i-use-in-my-application) · [Licensing errors](https://help.syncfusion.com/common/essential-studio/licensing/licensing-errors).

## 4. Development — onde configurar

Os valores em `appsettings*.json` são **placeholders**; a configuração real vem dos user-secrets. Substituir `<...>` pela sua chave (nunca a commitar):

```powershell
dotnet user-secrets set --project PropertyManagerFL.UI  "SyncfusionLicenseKey" "<chave-da-UI>"
dotnet user-secrets set --project PropertyManagerFL.Api "SyncfusionLicenseKey" "<chave-da-API>"
```

Se optar por uma única chave Enterprise/Community para ambos, usar o mesmo valor nos dois comandos.

Para confirmar que os valores estão definidos **sem os mostrar**, verificar a existência da entrada (a saída de `dotnet user-secrets list` mostra valores — não a cole em issues ou logs):

```powershell
dotnet user-secrets list --project PropertyManagerFL.UI  | Select-String SyncfusionLicenseKey
dotnet user-secrets list --project PropertyManagerFL.Api | Select-String SyncfusionLicenseKey
```

A aplicação lê estes valores no arranque; reiniciar API e UI depois de alterar a chave.

## 5. Production — onde configurar

Em Production os user-secrets **não são carregados**. O workflow `.github/workflows/cd.yml` lê os GitHub Secrets e o `.github/scripts/deploy-iis.ps1` escreve-os no `web.config` publicado como `environmentVariable` (elemento `environmentVariable`, não `<add>`):

```powershell
gh secret set PM_SYNCFUSION_LICENSE    --repo fauxtix/PropertyManagerFL --body "<chave-da-UI>"
gh secret set PM_SYNCFUSION_LICENSE_API --repo fauxtix/PropertyManagerFL --body "<chave-da-API>"
gh workflow run cd.yml --repo fauxtix/PropertyManagerFL --ref master
```

Se uma chave expirar ou for inválida, basta atualizar o segredo correspondente e repetir o deploy. Não é preciso mexer em `appsettings`, nem alterar código.

Os restantes segredos e o funcionamento do CD estão descritos em [PRODUCTION.md](PRODUCTION.md).

## 6. Resolução de problemas

| Sintoma | Causa provável | O que fazer |
|---|---|---|
| `Claim your FREE account and get a key in less than a minute` | Não há chave registada, valor vazio/placeholder, ou a variável não chegou àquela app | Confirmar a origem de configuração do ambiente (user-secrets em Dev, `web.config` em Prod), reiniciar a app |
| `The included Syncfusion® license key is invalid` | Chave de outra versão, edição ou plataforma | Gerar chave para 35.2.x com todas as edições |
| `The included Syncfusion® license ({versão}) is invalid for version 35.2.12` | Chave de volume release diferente | Gerar chave para a versão dos pacotes instalados |
| Aviso aparece só na UI | Chave da UI errada | Corrigir `SyncfusionLicenseKey` da UI / `PM_SYNCFUSION_LICENSE` |
| Aviso aparece só ao gerar documentos/PDF | Chave da API errada | Corrigir `SyncfusionLicenseKey` da API / `PM_SYNCFUSION_LICENSE_API` |
| Apareceu depois de atualizar pacotes NuGet | Subiu de volume release | Gerar novas chaves para a nova versão e atualizar os dois ambientes |
| Continua a aparecer após o deploy | Cache do browser ou pools antigos | `Ctrl+F5`; confirmar que o CD parou e reiniciou os app pools |

> **Nunca** publicar o valor de uma chave em issues, logs, `backup.md` ou ficheiros versionados. Em caso de dúvida, comparar apenas o comprimento/prefixo do valor injetado com o valor local, sem o imprimir.
