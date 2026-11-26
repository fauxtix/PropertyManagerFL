# Base de dados — instalação (SQL Server)

> **TL;DR (EN):** Install **SQL Server Developer** (dev/test) or **Community** (free, with limits), plus SSMS or `sqlcmd`. **Create an empty `PropertyManagerDB` first** — none of the scripts contains `CREATE DATABASE` or `DROP DATABASE`. Then choose **one** fresh-install path: run the individual scripts in the order below, or run the generated all-in-one `SqlData_Script/PropertyManagerDB.sql`. The full script was regenerated on 2026-11-26 and is in sync with the modular scripts; it creates the `[Identity]` schema itself and contains **no operational/sample data**. Never run both paths. Existing Dev/test databases need the idempotent `EndLeaseAndArchiving.sql` migration described in section 4 (and possibly the one-time `SuggestedAction` upgrade below). Production is created/updated outside CD by whoever administers SQL Server — for a local deploy, that is you — always with a backup. The CD publishes the apps only; it never runs SQL. See the [local setup](LOCAL-SETUP.md) and [Syncfusion licences](SYNCFUSION-LICENSES.md) guides.

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

Resumo para quem faz `clone`:

| Situação | O que fazer |
|---|---|
| BD nova para testar a app localmente | Criar a BD vazia, depois escolher **um** dos dois caminhos de instalação da secção 3 |
| BD Dev/test já existente | Aplicar a migração idempotente da secção 4 (com backup) |
| Deploy IIS/Produção | O CD publica só a aplicação; a BD é criada/atualizada pelo administrador do SQL Server, fora do CD |

Nos dois caminhos de instalação fresca a BD **já tem de existir**: os ficheiros começam com `USE [PropertyManagerDB]` (ou `USE [master]` nos logins IIS) e não criam nem largam bases de dados.

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
| `EndLeaseAndArchiving.sql` | **Migração idempotente** para bases de dados Dev/test já existentes: arquivamento de inquilino/fiador, `ID_Arrendamento` nos movimentos de renda, `HistoricoArrendamentos`, `usp_Arrendamentos_EndLease`, `usp_Inquilinos_Archive` e `usp_Fiadores_Archive` |
| `23012024_160350.Bak` | Backup antigo — alternativa aos scripts |

### `SqlData_Script/PropertyManagerDB.sql`

Script completo **gerado** do snapshot limpo de 2026-11-25 mais a migração idempotente de 2026-11-26. Cria o schema `[Identity]`, 79 tabelas, 2 funções, 6 vistas e 297 procedures de utilizador, e carrega **apenas** os dados de referência/configuração: distritos, concelhos, população, tabelas de *lookup*, roles, a conta `admin@propertymanager.pt` e a linha de `ApplicationSettings`.

**Não contém dados operacionais nem dados de exemplo** — imóveis, fracções, inquilinos, arrendamentos, recibos, documentos, mensagens, notificações e logs ficam vazios. Criar dados de teste é da responsabilidade de quem faz o clone, com nomes fictícios; nunca reutilizar dados reais de inquilinos.

Valores sanitizados no `INSERT` de `ApplicationSettings`: `ApiKey` e os dois caminhos de backup são *placeholders* (`REPLACE_WITH_...`) e os campos de identidade SMTP ficam `NULL`. Substituir pelos valores reais antes de usar a aplicação. A conta `admin` é genérica: alterar a palavra-passe após a instalação.

**Não é um passo adicional**: usar este script *substitui* a execução dos ficheiros individuais. Não executar ambos. A pasta `SqlData_Script` contém apenas este ficheiro — os antigos `Sps_Arrendamentos.sql` e `RentsDB.db.sql` foram removidos. O snapshot foi regenerado a 2026-11-26 e está sincronizado com os scripts modulares, incluindo `HistoricoArrendamentos`, `ID_Arrendamento`, `usp_Arrendamentos_EndLease`, `usp_Inquilinos_Archive` e `usp_Fiadores_Archive`. Não editar à mão — regenerar a partir de uma BD limpa com SMO sempre que o esquema/procedures mudarem.

> **Não usar para criar ou atualizar Produção.** Aqui, “responsável da infraestrutura/DBA” significa simplesmente quem administra o SQL Server desse ambiente — num deploy no teu PC, és tu. O `cd.yml` publica API/UI e **não corre SQL**. Em `PropertyManagerDB_Production`, fazer backup e aplicar apenas a migração da secção 4, com autorização; nunca os scripts completos.

## 3. Instalação passo a passo (scripts individuais)

1. No SSMS (ou `sqlcmd`), criar uma base de dados vazia com o nome **`PropertyManagerDB`**. Nenhum dos scripts cria ou larga a BD:

   ```sql
   CREATE DATABASE [PropertyManagerDB];
   GO
   ```
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

> ℹ️ `SqlData_Script/PropertyManagerDB.sql` é um *snapshot* gerado e **não deve ser editado à mão**. Foi regenerado a 2026-11-26 a partir do snapshot limpo anterior + `EndLeaseAndArchiving.sql` e verificado num BD vazio (contagens de objetos e todas as 79 tabelas). Após futuras alterações de esquema/procedures, tem de ser regenerado novamente. Numa BD já existente, use a migração idempotente da secção 4 — nunca o snapshot.

### Alternativa B — restaurar o backup

No SSMS: botão direito em **Databases → Restore Database… → Device → Add…** e escolher `PropertyManagerFL.Infrastructure/Database/23012024_160350.Bak`. O backup é antigo: confirme depois se contém os objetos da secção 4 e atualize-os com os scripts atuais se necessário. **Não use dados reais de inquilinos** num ambiente partilhado.

## 4. Atenção: os scripts não são idempotentes

`PropertyManagerDB.sql` contém dezenas de `CREATE TABLE` e `CREATE PROCEDURE` sem verificação de existência. Voltá-los a executar numa base de dados já populada provoca erros de "objeto já existe" e pode deixar a BD num estado intermédio.

- Usar **sempre numa base de dados nova/vazia**.
- Para repetir a instalação, largar e recriar a base de dados.
- Não executar em conjunto os scripts individuais e o script completo.

### Atualizar uma base de dados Dev/test já existente (fim de contrato)

`EndLeaseAndArchiving.sql` é **idempotente** e deve ser executado uma vez em cada base de dados Dev/test antes de usar a página de arrendamentos com a nova opção “Terminar contrato”:

```powershell
sqlcmd -S .\SQLEXPRESS -E -d PropertyManagerDB `
  -i PropertyManagerFL.Infrastructure\Database\EndLeaseAndArchiving.sql -b
```

O script:
- cria/repara os defaults de `Inquilinos.Ativo` e `Fiadores.Ativo` (`1`) e preenche valores `NULL`;
- adiciona `ID_Arrendamento` a `Recebimentos` e `RecebimentosTemp`, com *backfill* pelo contrato mais recente da mesma fração/inquilino;
- cria `HistoricoArrendamentos`;
- substitui as procedures de inserção de renda, inserção/alteração de inquilino e fiador, listas de disponíveis e verificação de fração livre;
- cria `usp_Arrendamentos_EndLease`, que termina o contrato, liberta a fração, arquiva inquilino/fiador (apenas se não tiver outro contrato ativo), grava o histórico e calcula a dívida — tudo numa transação com `TRY/CATCH`/`ROLLBACK`;
- cria `usp_Inquilinos_Archive` e `usp_Fiadores_Archive` para o botão **Arquivar**: `Ativo=0`, nota com a data da decisão e bloqueio quando existe contrato ativo. Nunca apagam registos.

Instalações frescas já incluem estas alterações nos scripts modulares e no snapshot regenerado; **não executar a migração após uma instalação fresca**. Em `PropertyManagerDB_Production`, só executar com autorização e backup prévio: o “responsável” é quem administra o SQL Server (num deploy local, o próprio utilizador). O CD não aplica esta migração.

## 5. Verificação

Depois da instalação, executar na `PropertyManagerDB`:

```sql
SELECT (SELECT COUNT(*) FROM sys.tables  WHERE is_ms_shipped=0) AS Tabelas,      -- 79
       (SELECT COUNT(*) FROM sys.views   WHERE is_ms_shipped=0) AS Vistas,       -- 6
       (SELECT COUNT(*) FROM sys.objects WHERE type IN ('FN','IF','TF') AND is_ms_shipped=0) AS Funcoes, -- 2
       (SELECT COUNT(*) FROM sys.procedures WHERE is_ms_shipped=0) AS Procedures, -- 297
       (SELECT COUNT(*) FROM sys.schemas WHERE name = 'Identity') AS SchemaIdentity;

SELECT name FROM sys.procedures
WHERE name LIKE 'usp_Notifications_%' ORDER BY name;   -- esperadas: 8

SELECT name FROM sys.procedures
WHERE name IN ('usp_Arrendamentos_EndLease','usp_Inquilinos_Archive','usp_Fiadores_Archive')
ORDER BY name;                                        -- esperadas: 3

SELECT name FROM sys.tables WHERE name = 'HistoricoArrendamentos';
SELECT COL_LENGTH('dbo.Recebimentos','ID_Arrendamento')     AS RecebimentosID,
       COL_LENGTH('dbo.RecebimentosTemp','ID_Arrendamento') AS RecebimentosTempID; -- 4 e 4

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

### Alertas como tarefas e povoamento inicial

Cada aviso é uma tarefa: `ReadAt` passa a significar **concluído em** (`IsCompleted`), não "lido". Abrir o aviso não altera esse estado; o botão **Concluir/Reabrir** é que o altera. Um aviso concluído não é recriado pelos refreshes seguintes.

Ao arrancar, a API executa `SeedAlertsForAllUsersAsync` para todos os utilizadores em `dbo.AspNetUsers`, pelo que uma base nova fica com os alertas pendentes desde o primeiro minuto. A operação é idempotente (não duplica avisos). Também está disponível `POST /api/Notifications/alerts/refresh-all` (protegido pela ApiKey).

Ao atualizar uma BD que já continha notificações com o significado antigo de "lida", essas linhas **não devem** ser interpretadas como concluídas. Faça primeiro um backup e limpe a caixa de entrada uma única vez, deixando o arranque da API gerar os alertas pendentes:

```sql
-- apenas após backup e com a aplicação parada
DELETE FROM dbo.Notifications;
```

Não é necessária nenhuma alteração de *stored procedures* nem de colunas para esta mudança.

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
