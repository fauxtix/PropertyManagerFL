# Base de dados — instalação (SQL Server)

> **TL;DR (EN):** Install **SQL Server Developer** (dev/test) or **Community** (free, with limits), plus SSMS or `sqlcmd`. Create an empty `PropertyManagerDB`, create the `[Identity]` schema, then run the scripts in the order below. The scripts are **not idempotent**: run them once, on an empty database. The full script `SqlData_Script/PropertyManagerDB.sql` is an alternative to the individual scripts, not an extra step: it is generated from the cleaned reference database, creates the `[Identity]` schema itself and contains **no operational/sample data**. These scripts are for **fresh Dev/test installs only** — never use them to create or update the Production database. Existing Dev/test databases need only the small `SuggestedAction` upgrade below. See the [local setup](LOCAL-SETUP.md) and [Syncfusion licences](SYNCFUSION-LICENSES.md) guides.

## 1. Pré-requisitos

| Ferramenta | Para que serve | Notas |
|---|---|---|
| **SQL Server Developer** | Base de dados local de desenvolvimento/teste | Gratuito; não usar em produção |
| **SQL Server Community** | Alternativa gratuita para produção pequena | Gratuita, com limites de licença; ver a página oficial |
| **SSMS** (SQL Server Management Studio) | Criar a BD e executar os scripts | Gratuito |
| `sqlcmd` | Executar scripts por linha de comando | Vem com o SSMS ou com as *Microsoft Command Line Utilities* |

Downloads: [SQL Server (Developer/Community/Express)](https://www.microsoft.com/pt-pt/sql-server/sql-server-downloads) · [SSMS](https://aka.ms/ssmsfullsetup).

O projeto foi desenvolvido com uma instância `SQLEXPRESS`, mas serve qualquer instância local. As *connection strings* checked-in apontam para `DESKTOP-SLIMNUD\SQLEXPRESS` — **têm de ser sobrepostas** com a sua instância (ver [LOCAL-SETUP.md](LOCAL-SETUP.md)).

> As migrações EF estão desativadas (`Api/Configuration/DataBaseConfig.cs`). A criação da base de dados é **manual**.

## 2. Ficheiros disponíveis

### `PropertyManagerFL.Infrastructure/Database/`

| Ficheiro | Conteúdo |
|---|---|
| `WorkingTables.sql` | Tabelas, incluindo os seus índices, e as tabelas do schema `[Identity]` usadas pelo ASP.NET Identity |
| `LookupTablesWithData.sql` | Tabelas de *lookup* e dados iniciais (redigidos em português — ver a secção 6) |
| `UserDefinedFunctions.sql` | Funções SQL |
| `StoredProcedures.sql` | *Stored procedures* |
| `Views.sql` | Vistas |
| `UsersAndSchemas.sql` | Schema `[Identity]`, users `IIS APPPOOL\*`, permissão `db_owner` |
| `Login_App.sql` | Login `IIS APPPOOL\PropertyManagerFL` (executado em `master`) |
| `Login_Api.sql` | Login `IIS APPPOOL\PropertyManagerAPI` (executado em `master`) |
| `23012024_160350.Bak` | Backup antigo — alternativa aos scripts |

### `SqlData_Script/PropertyManagerDB.sql`

Script completo **gerado** da base de dados de referência já limpa (`PropertyManagerDB_Production`, 2026-11-25). Cria o schema `[Identity]`, as 78 tabelas, 2 funções, 6 vistas e 294 procedures, e carrega **apenas** os dados de referência/configuração: distritos, concelhos, população, tabelas de *lookup*, roles, a conta `admin@propertymanager.pt` e a linha de `ApplicationSettings`.

**Não contém dados operacionais nem dados de exemplo** — imóveis, fracções, inquilinos, arrendamentos, recibos, documentos, mensagens, notificações e logs ficam vazios. Criar dados de teste é da responsabilidade de quem faz o clone, com nomes fictícios; nunca reutilizar dados reais de inquilinos.

Valores sanitizados no `INSERT` de `ApplicationSettings`: `ApiKey` e os dois caminhos de backup são *placeholders* (`REPLACE_WITH_...`) e os campos de identidade SMTP ficam `NULL`. Substituir pelos valores reais antes de usar a aplicação. A conta `admin` é genérica: alterar a palavra-passe após a instalação.

**Não é um passo adicional**: usar este script *substitui* a execução dos ficheiros individuais. Não executar ambos. A pasta `SqlData_Script` contém apenas este ficheiro — os antigos `Sps_Arrendamentos.sql` e `RentsDB.db.sql` foram removidos. Não editar este ficheiro à mão — regenerá-lo a partir de uma BD limpa com SMO.

> **Não usar em Produção.** A base de dados de Produção (`PropertyManagerDB_Production`) é gerida pelo responsável da infraestrutura (backups e manutenção). Estes scripts servem apenas para instalações novas em Development/teste.

## 3. Instalação passo a passo (scripts individuais)

1. No SSMS, criar uma base de dados vazia com o nome **`PropertyManagerDB`**.
2. Criar o schema `[Identity]`, necessário antes de `WorkingTables.sql`:

   ```sql
   USE [PropertyManagerDB];
   GO
   IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = N'Identity')
       EXEC(N'CREATE SCHEMA [Identity]');
   GO
   ```

3. **Apenas se for usar IIS**: executar `Login_App.sql` e `Login_Api.sql` com a base de dados `master` selecionada. Estes scripts criam os logins Windows dos app pools. O nome `PropertyManagerAPI` (maiúsculas) é equivalente ao pool `PropertyManagerApi` do deploy — os nomes Windows não são sensíveis a maiúsculas/minúsculas.
4. Executar `UsersAndSchemas.sql` na `PropertyManagerDB` (cria os users, dá `db_owner` e cria o schema `[Identity]`). Se o passo 2 já criou o schema, o `CREATE SCHEMA [Identity]` deste script vai falhar — nesse caso, remover apenas essa linha ou ignorar o erro. Em desenvolvimento **sem IIS**, os `CREATE USER [IIS APPPOOL\...]` também falham se as contas virtuais dos pools não existirem: ignorar essas linhas; a aplicação liga-se com a sua conta Windows através de `Integrated Security`.
5. Executar, nesta ordem, na `PropertyManagerDB`:

   | # | Script |
   |---|---|
   | 1 | `WorkingTables.sql` |
   | 2 | `LookupTablesWithData.sql` |
   | 3 | `UserDefinedFunctions.sql` |
   | 4 | `StoredProcedures.sql` |
   | 5 | `Views.sql` |

   Por linha de comando (exemplo, ajustar a instância):

   ```powershell
   $scripts = @(
     'PropertyManagerFL.Infrastructure/Database/WorkingTables.sql',
     'PropertyManagerFL.Infrastructure/Database/LookupTablesWithData.sql',
     'PropertyManagerFL.Infrastructure/Database/UserDefinedFunctions.sql',
     'PropertyManagerFL.Infrastructure/Database/StoredProcedures.sql',
     'PropertyManagerFL.Infrastructure/Database/Views.sql'
   )
   foreach ($s in $scripts) { sqlcmd -S .\SQLEXPRESS -E -d PropertyManagerDB -i $s -b }
   ```

   `-b` faz o `sqlcmd` terminar com erro se o script falhar.

### Alternativa A — script completo

```powershell
sqlcmd -S .\SQLEXPRESS -E -i SqlData_Script\PropertyManagerDB.sql -b
```

O script começa com `USE [PropertyManagerDB]`, pelo que a base de dados já tem de existir. O schema `[Identity]` é criado pelo próprio script — não executar o passo 2 da secção 3 neste caminho.

### Alternativa B — restaurar o backup

No SSMS: botão direito em **Databases → Restore Database… → Device → Add…** e escolher `PropertyManagerFL.Infrastructure/Database/23012024_160350.Bak`. O backup é antigo: confirme depois se contém os objetos da secção 4 e atualize-os com os scripts atuais se necessário. **Não use dados reais de inquilinos** num ambiente partilhado.

## 4. Atenção: os scripts não são idempotentes

`PropertyManagerDB.sql` contém dezenas de `CREATE TABLE` e `CREATE PROCEDURE` sem verificação de existência. Voltá-los a executar numa base de dados já populada provoca erros de "objeto já existe" e pode deixar a BD num estado intermédio.

- Usar **sempre numa base de dados nova/vazia**.
- Para repetir a instalação, largar e recriar a base de dados.
- Não executar em conjunto os scripts individuais e o script completo.

## 5. Verificação

Depois da instalação, executar na `PropertyManagerDB`:

```sql
SELECT (SELECT COUNT(*) FROM sys.tables)                         AS Tabelas,
       (SELECT COUNT(*) FROM sys.views)                          AS Vistas,
       (SELECT COUNT(*) FROM sys.procedures)                     AS Procedures,
       (SELECT COUNT(*) FROM sys.schemas WHERE name = 'Identity') AS SchemaIdentity;

SELECT name FROM sys.procedures
WHERE name LIKE 'usp_Notifications_%' ORDER BY name;   -- esperadas: 8

SELECT COUNT(*) AS Notificacoes FROM dbo.Notifications; -- 0 numa BD nova
```

Resultado esperado para as procedures de notificações:

`usp_Notifications_Insert`, `usp_Notifications_GetForUser`, `usp_Notifications_GetUnreadCount`, `usp_Notifications_MarkRead`, `usp_Notifications_MarkAllRead`, `usp_Notifications_MarkSeen`, `usp_Notifications_PurgeOld`, `usp_Notifications_ExistsRecent`.

### O que muda para quem usa a aplicação

O sino e a página `/notifications` mostram, além do texto do aviso, uma **sugestão de ação** (`SuggestedAction`): por exemplo, abrir a ficha do inquilino, registar um pagamento ou gerar a carta correspondente. Os alertas operacionais agora incluem:

- **rendas em atraso** com valor em falta;
- **falta de carta de aviso** para rendas em atraso;
- **renda a vencer** nos próximos dias;
- **carta de revogação sem resposta** após o prazo configurado;
- **contrato marcado como emitido sem documento gerado**.

Estas regras são calculadas no servidor; não é preciso configurar nada para as ver.

### Atualização de uma BD já criada (coluna `SuggestedAction`)

As notificações passam a guardar também uma **sugestão de ação** para o utilizador. Bases criadas com o script completo já a têm; em bases criadas antes desta alteração, executar apenas:

```sql
IF COL_LENGTH(N'dbo.Notifications', N'SuggestedAction') IS NULL
    ALTER TABLE [dbo].[Notifications] ADD [SuggestedAction] nvarchar(1024) NULL;
GO
```

Depois voltar a executar **apenas** `usp_Notifications_Insert` e `usp_Notifications_GetForUser` (estão em `StoredProcedures.sql` e no script completo). Não é necessário recriar a base de dados.

Se usar IIS, confirmar também:

```sql
SELECT name FROM sys.server_principals WHERE name LIKE 'IIS APPPOOL\%';
SELECT name FROM sys.database_principals WHERE name LIKE 'IIS APPPOOL\%';
```

## 6. Dados iniciais, idiomas e modelos de cartas

- As tabelas de *lookup* vêm preenchidas em português. Para usar outra língua, adaptar os dados (`LookupTablesWithData.sql` ou diretamente na BD) — ver a secção de localização no README.
- O script completo não inclui dados operacionais nem de demonstração. Cada utilizador deve criar os seus próprios dados de teste (imóveis, inquilinos, rendas, etc.) com nomes fictícios. Nunca copiar dados reais de inquilinos para um ambiente partilhado.
- Os modelos de cartas (`.dotx`) estão em `PropertyManagerFL.Api/Reports/Docs`. Cada país pode ter regras/modelos diferentes; traduzir o texto com Winword, preservando os *placeholders* dos campos variáveis.
- Não alterar ficheiros gerados (`*.Designer.cs`, `TableAdapters.Designer.cs`, `ScheduleData.cs`).

## 7. Próximo passo

Com a base de dados criada, seguir [LOCAL-SETUP.md](LOCAL-SETUP.md) para configurar as *connection strings*, os segredos locais e arrancar a API e a UI.
