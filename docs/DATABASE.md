# Base de dados — instalação (SQL Server)

> **TL;DR (EN):** Install **SQL Server Developer** (dev/test) or **Community** (free, with limits), plus SSMS or `sqlcmd`. Create an empty `PropertyManagerDB`, create the `[Identity]` schema, then run the scripts in the order below. The scripts are **not idempotent**: run them once, on an empty database. The full script `SqlData_Script/PropertyManagerDB.sql` is an alternative to the individual scripts, not an extra step. See the [local setup](LOCAL-SETUP.md) and [Syncfusion licences](SYNCFUSION-LICENSES.md) guides.

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
| `WorkingTables.sql` | Tabelas, incluindo `dbo.Notifications` e os seus índices, e as tabelas do schema `[Identity]` usadas pelo ASP.NET Identity |
| `LookupTablesWithData.sql` | Tabelas de *lookup* e dados iniciais (redigidos em português — ver a secção 6) |
| `UserDefinedFunctions.sql` | Funções SQL |
| `StoredProcedures.sql` | *Stored procedures*, incluindo as 8 `usp_Notifications_*` do centro de notificações |
| `Views.sql` | Vistas |
| `UsersAndSchemas.sql` | Schema `[Identity]`, users `IIS APPPOOL\*`, permissão `db_owner` |
| `Login_App.sql` | Login `IIS APPPOOL\PropertyManagerFL` (executado em `master`) |
| `Login_Api.sql` | Login `IIS APPPOOL\PropertyManagerAPI` (executado em `master`) |
| `23012024_160350.Bak` | Backup antigo — alternativa aos scripts |

### `SqlData_Script/PropertyManagerDB.sql`

Script completo (tabelas + dados + funções + procedures + vistas) para criar a base de dados de uma só vez. **Não é um passo adicional**: usar este script *substitui* a execução dos ficheiros individuais. Não executar ambos.

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

O script começa com `USE [PropertyManagerDB]`, pelo que a base de dados já tem de existir. Crie também o schema `[Identity]` (secção 3, passo 2), porque o script completo usa tabelas `[Identity].*` mas não cria o schema.

### Alternativa B — restaurar o backup

No SSMS: botão direito em **Databases → Restore Database… → Device → Add…** e escolher `PropertyManagerFL.Infrastructure/Database/23012024_160350.Bak`. O backup é antigo: confirme depois se contém os objetos da secção 4 e atualize-os com os scripts atuais se necessário. **Não use dados reais de inquilinos** num ambiente partilhado.

## 4. Atenção: os scripts não são idempotentes

`PropertyManagerDB.sql` contém dezenas de `CREATE TABLE` e `CREATE PROCEDURE` sem verificação de existência. Voltá-los a executar numa base de dados já populada provoca erros de "objeto já existe" e pode deixar a BD num estado intermédio.

- Usar **sempre numa base de dados nova/vazia**.
- Para repetir a instalação, largar e recriar a base de dados.
- Não executar em conjunto os scripts individuais e o script completo.
- O centro de notificações não precisa de nenhum script à parte: `dbo.Notifications`, os índices e as procedures `usp_Notifications_*` já estão incluídos nos ficheiros acima.

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

Se usar IIS, confirmar também:

```sql
SELECT name FROM sys.server_principals WHERE name LIKE 'IIS APPPOOL\%';
SELECT name FROM sys.database_principals WHERE name LIKE 'IIS APPPOOL\%';
```

## 6. Dados iniciais, idiomas e modelos de cartas

- As tabelas de *lookup* vêm preenchidas em português. Para usar outra língua, adaptar os dados (`LookupTablesWithData.sql` ou diretamente na BD) — ver a secção de localização no README.
- Os modelos de cartas (`.dotx`) estão em `PropertyManagerFL.Api/Reports/Docs`. Cada país pode ter regras/modelos diferentes; traduzir o texto com Winword, preservando os *placeholders* dos campos variáveis.
- Não alterar ficheiros gerados (`*.Designer.cs`, `TableAdapters.Designer.cs`, `ScheduleData.cs`).

## 7. Próximo passo

Com a base de dados criada, seguir [LOCAL-SETUP.md](LOCAL-SETUP.md) para configurar as *connection strings*, os segredos locais e arrancar a API e a UI.
