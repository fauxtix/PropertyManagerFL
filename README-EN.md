# PropertyManagerFL

[Português](https://github.com/fauxtix/PropertyManagerFL/blob/master/README.md)

Application designed to assist landlords in managing their properties and rentals. With a user-friendly interface and a range of helpful features, PropertyManagerFL (PMFL) serves as a centralized platform for landlords, to streamline their property management tasks.

## Summary (start here)

- **What it is:** a localhost, single-user application with an **ASP.NET Core API** and a **Blazor Server UI** that must run **at the same time**. It manages properties, tenants, leases, rents, expenses, letters, messages and a **notification centre** (bell + inbox, SignalR with a 60-second polling fallback). Each alert can include a **suggested action** (open the tenant record, record a payment, generate a letter, etc.).
- **Stack:** C#, .NET 10 LTS, Blazor, **Syncfusion 35.2.12**, Dapper, AutoMapper, FluentValidation, Serilog, SQL Server.
- **Prerequisites:** .NET 10 SDK (`global.json`), **SQL Server Developer or Community** (both free) with the database created, SSMS/`sqlcmd`, a development HTTPS certificate, SMTP credentials and **your own Syncfusion licences**.
- **Database:** manual setup — EF migrations are disabled. The scripts (`PropertyManagerFL.Infrastructure/Database/*.sql` or `SqlData_Script/PropertyManagerDB.sql`) are **not idempotent** and must run **on an empty database**, in the order described in [DATABASE.md](docs/DATABASE.md). The full script is generated from the cleaned reference database and contains only configuration/reference data; **test data is the user's responsibility**.
- **Keys:** the UI and the API each register their own Syncfusion licence. In Development use `dotnet user-secrets set --project ...`; in Production use GitHub Secrets. **Choose a key for version 35.2.x with all editions** — see [SYNCFUSION-LICENSES.md](docs/SYNCFUSION-LICENSES.md).
- **Run locally (Development, Kestrel):** API at `https://localhost:5001`, UI at `https://localhost:7181` — the original project ports, different from IIS (Production: 4300/4310), so both can run at the same time. Full commands in [LOCAL-SETUP.md](docs/LOCAL-SETUP.md).
- **If you cloned this repo:** GitHub Secrets are **not copied by a clone**. Everyone needs their own database, credentials and licences. Never reuse the maintainer's keys or commit any key.
- **Production:** manual GitHub Actions deploy to IIS with secrets injected into `web.config` — **except the connection string**, which comes from `appsettings.Production.json` and points to `PropertyManagerDB_Production` — see [PRODUCTION.md](docs/PRODUCTION.md).
- **Tests:** `dotnet test PropertyManagerFL.Tests/PropertyManagerFL.Tests.csproj`.

### Guides

| Guide | What it covers |
|---|---|
| [DATABASE.md](docs/DATABASE.md) | Install SQL Server, create `PropertyManagerDB`, script order, verification |
| [LOCAL-SETUP.md](docs/LOCAL-SETUP.md) | Ports, per-project user-secrets, running the apps, themes/languages, troubleshooting |
| [SYNCFUSION-LICENSES.md](docs/SYNCFUSION-LICENSES.md) | **Which key to choose (version/edition)**, where to set it, how to fix licence warnings |
| [PRODUCTION.md](docs/PRODUCTION.md) | GitHub Secrets, IIS runner, app pools, deploy, key rotation |

> The detailed guides are written in Portuguese; each one starts with a short English TL;DR.

## Key Features

- **Centralized Property and Tenant Information**: PMFL allows landlords to store all property and tenant details in one convenient location. From property specifications to tenant records and contact information, everything is easily accessible whenever you need it.

- **Rent Lease Creation**: PMFL simplifies the process of creating rent leases; new and existing ones.

- **Rent Payment Tracking**: PMFL provides a comprehensive system for tracking rent payments, helping landlords stay updated on transactions and promptly address any overdue payments.

- **Notification centre**: a bell and an inbox with operational alerts and a **suggested action** on each alert. Alerts include overdue rent, a missing overdue-rent letter, rent due soon, a revocation letter with no reply, and a lease marked as issued without a generated document. Updates arrive via SignalR, with a 60-second polling fallback.

- **Keeping track of rental property expenses**: as a landlord, you’ll be lost (even with an accountant) if you aren’t tracking your rental property expenses correctly.
  Keeping detailed expense records will not only help you feel more organized but it will also make filing your taxes easier, allow you to see more opportunities for deductions, and understand the return on each of your rental investments.

- **Tenant letters management**:

  >Invitation to Renew - This gives the tenant some time to decide whether to renew their lease or move out.

  >Termination Letter - This signals the end of the rental agreement. It might be due to the tenant’s plan to move out or the landlord's refusal to renew the lease

  >Increase in Rent - an invitation to renew so the tenant can decide whether to stay

  >Change in Payment Information - To avoid confusion and continue to receive rent payments on time, landlords should let tenants know of payment changes

  >Overdue Rent Notice - to put rent reminders in writing. A landlord might need proof that a tenant was chronically late in paying as a basis for terminating their lease

  >Pay or Quit Letter - warning about unpaid rent. It demands payment of current and back rent by a certain date or eviction proceedings will begin

  >Rent increases - process is automatic for the following year, from the start date of the contract, or through a manual procedure for each tenant.
    Each of these situations presupposes a letter alerting them to the change

  >Communicating with tenants - messages sent or received to/from tenants, through the use of e-mail

## Localization, themes and documents

The application supports Portuguese, English, French and Spanish (`en-US` is the default culture; `AppSettings:Language` is `pt-PT`). There is a light/dark theme toggle: **dark** is the default and the choice is persisted in the browser.

Some of the tables used in the application (mainly **lookup** ones) will require user intervention, since Portuguese has been used as the native language for filling in/configuring them.

The same goes for the wording of the various letters sent to tenants, as each country may have different templates/rules to apply, which will have to be adapted accordingly.
It's a simple process to carry out:
1. Use Winword to open each template document used in the application (templates / dotx), stored in the [documents folder](https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Api/Reports/Docs);
2. Copy its text, and use a translator (Google, DeepL, ...) to adapt it to your needs;
3. Copy the translated text and replace the text in the letter (safeguarding the placeholders of variable fields);
4. Finish the process by saving the updated documents (overwriting the existing ones).

More details in [LOCAL-SETUP.md](docs/LOCAL-SETUP.md).

## Database

The structure (tables, stored procedures, functions, views) is in the [database scripts folder](https://github.com/fauxtix/PropertyManagerFL/tree/master/PropertyManagerFL.Infrastructure/Database) and in the full script `SqlData_Script/PropertyManagerDB.sql`, generated from the already cleaned reference database. The notification centre and its procedures are part of those scripts — there is no separate script; the suggested-action column is already included. The full script contains only reference/configuration data (`ApiKey` and backup paths are placeholders); it has **no properties, tenants or other sample data** — each user creates their own with fictional names. EF migrations are disabled: database creation is manual and the scripts must only be run on an empty database. **Never use these scripts to create or update the Production database** — it is managed by the infrastructure owner. The execution order and the upgrade path for older databases are in [DATABASE.md](docs/DATABASE.md).

## Tests and deployment

- Tests: `dotnet test PropertyManagerFL.Tests/PropertyManagerFL.Tests.csproj`.
- CI: `.github/workflows/ci.yml` (restore/build/test on .NET 10, no database).
- CD: `.github/workflows/cd.yml`, manual (`workflow_dispatch`) from `master`, on a self-hosted runner with the `iis-propertymanager` label. See [PRODUCTION.md](docs/PRODUCTION.md).

## 🌟 Contributing

Contributions to this project are welcome! If you encounter any issues or have suggestions for improvement, please open an issue on the GitHub repository: https://github.com/fauxtix/PropertyManagerFL/Issues

Fork the project (https://github.com/fauxtix/PropertyManagerFL/fork)

Create a branch for your modification (git checkout -b fauxtix/PropertyManagerFL)

Commit (git commit -am 'added a new feature - some files changed')

Push_ (git push origin fauxtix/PropertyManagerFL)

Create a new Pull Request

More info: https://www.digitalocean.com/community/tutorials/how-to-create-a-pull-request-on-github

When contributing code, please follow the existing code style and submit a pull request with your changes. Before running the app, follow the [setup guides](#guides) and never commit keys, connection strings or tenant data.

## ⚖ License

The source code for the site is licensed under the MIT license, which you can find in the [[MIT-LICENSE.txt](https://github.com/fauxtix/PropertyManagerFL/blob/master/MIT-LICENSE.txt)] file.

## 📞 Contact

If you have any questions or need further assistance, you can reach out to the project maintainer:

- 👨‍💻 Maintainer: Fausto Luís
- ✉ Email: fauxtix.luix@hotmail.com

Feel free to contact me with any feedback or inquiries.

Thank you for using PropertyManagerFL!
