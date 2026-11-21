# AGENTS.md

## Repo shape
- `PropertyManagerFL.sln` is the build boundary: 6 projects (Core, Application, Infrastructure, Api, UI, EmailService). Five other `.csproj` files sit outside it (`Client.Infrastrucure` [sic], `Shared`, `Sql`, `Logging`, `Application/Shared/CommonLayer`); `Logging` is empty but still referenced by Api.
- Two web apps must run together: `PropertyManagerFL.slnLaunch.user` starts API, then UI. UI is Blazor Server; it calls the API via `ApiWrappers/*` HttpClient wrappers **and** references Infrastructure directly (repositories registered in `UI/Configuration/DependencyInjectionConfig.cs`).
- API/UI DI, AutoMapper and FluentValidation setup lives in parallel `Configuration/*.cs` files and is near-duplicated; changes often need both. Entry points are `Api/Program.cs` and `UI/Program.cs`.

## Commands / verification
- Build: `dotnet build PropertyManagerFL.sln`
- Run locally in two terminals: set `$env:ASPNETCORE_ENVIRONMENT = 'Development'` first, then `dotnet run --project PropertyManagerFL.Api` and `dotnet run --project PropertyManagerFL.UI`. The shell may already have `ASPNETCORE_ENVIRONMENT=Production`; in Production the user-secrets are not loaded (API returns 401) and UI uses the stale Production `BaseUrl`.
- Dev URLs: API `https://localhost:5001` / `http://localhost:5000`; UI `https://localhost:7181` / `http://localhost:5152`.
- No test projects, CI workflows, lint scripts, or codegen tasks exist. Verification = build + manual smoke test (login, properties/tenants CRUD, payment, letter/PDF, upload).
- SDK pinned by `global.json` to 10.0.404 (`rollForward: latestPatch`); `dotnet --version` in this repo returns 10.0.x even though SDK 11 is installed machine-wide. No `Directory.Build.props`, lockfiles or NuGet config. Projects in the solution now target `net10.0`; the 5 orphan `.csproj` outside it still target `net6.0`.

## Config / environment gotchas
- Both web projects set `UserSecretsId`; supply real values via user-secrets/env. Checked-in `PMConnection` strings point at `DESKTOP-SLIMNUD\SQLEXPRESS` and must be overridden locally.
- `BaseUrl` exists only in UI `appsettings.Development/Production`, not the base file. Dev must be `https://localhost:5001/api`; the Production value (`http://localhost:4300/api`) is stale.
- `ApiKey` is the real API gate: global `ApiKeyMiddleware` checks one header on every request, added by UI via `Services/ClientApi/HttpClientConfigurationService.cs`. JWT is mostly unused/misconfigured (`Issuer` vs `ValidIssuer`); do not assume `[Authorize]` protects endpoints.
- `SyncfusionLicenseKey` is a placeholder in appsettings but registered in both `Program.cs` files; a valid key is required for the Syncfusion UI.
- DB setup is manual: `Infrastructure/Database/*.sql` (WorkingTables, LookupTablesWithData, StoredProcedures, Views, UsersAndSchemas) or `SqlData_Script/PropertyManagerDB.sql`. EF migrations are commented out in `Api/Configuration/DataBaseConfig.cs`; local `dotnet-ef` tool manifests exist but are unused.
- Serilog `appsettings` sections are ignored; logging is configured in code to the MSSQL `PMLogs` sink. API also calls `XmlConfigurator.Configure("log4net.config")` although no such file exists.
- Treat checked-in config values and generated documents under `Api/Reports/Docs/` as sensitive/placeholder; never commit real tenant data.

## Code traps
- Compile-time exclusions mean edits have no effect unless the csproj changes: `Api/Controllers/LoginAccessController.cs`, `Api/Filter/AuthorizationFilterAttribute.cs`, `Infrastructure/Services/AppManagerServices/**`, `UI/Pages/Admin/ViewLogins.razor`, `UI/wwwroot/EstateAgency/**`.
- Don't hand-edit committed generated files: `Core/Datasets/TableAdapters.Designer.cs`, `UI/Resources/*Designer.cs`, `UI/Data/Scheduler/ScheduleData.cs`.
- Add new AutoMapper profiles to both `Api/Configuration/AutoMapperConfig.cs` and `UI/Configuration/AutoMapperConfig.cs`; the UI registers only a subset.
- `LookupTablesController` passes raw table/column names into `LookupTableRepository`; changing that shape affects UI wrappers too.
- ~188 `NotImplementedException` throws exist across wrappers/repositories; check whether a missing behavior is a stub before debugging callers.

## Implementation handoff (`backup.md`)
- For any multi-step implementation, create `backup.md` in the repo root before starting. It is transient local state (gitignored), not documentation.
- Update it after every meaningful step, before risky edits, and at the end of every session. Append timestamped entries; keep a short `Resume here` block at the top. Never put secrets, credentials or tenant PII in it.
- Record: goal/scope, current phase and exact next action, completed steps with commands and results, files changed, verbatim errors, verification done, open decisions/blockers, and any "do not do X" warning discovered.
- On resume: read `backup.md` first, then run `git status --short` and verify the recorded last-good build/smoke state before continuing. Do not rely on conversation memory.
- Delete `backup.md` only after the implementation is verified and the user confirms success; create a fresh one at the start of the next multi-step task.
