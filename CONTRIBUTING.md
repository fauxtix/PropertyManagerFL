## 🌟 Contributing

Contributions to this project are welcome! If you encounter any issues or have suggestions for improvement, please open an issue on the GitHub repository: https://github.com/fauxtix/PropertyManagerFL/Issues 

Fork the project (https://github.com/fauxtix/PropertyManagerFL/fork)

Create a branch for your modification (git checkout -b fauxtix/PropertyManagerFL)

Commit (git commit -am 'added a new feature - some files changed')

Push_ (git push origin fauxtix/PropertyManagerFL)

Create a new Pull Request

More info: https://www.digitalocean.com/community/tutorials/how-to-create-a-pull-request-on-github

When contributing code, please follow the existing code style and submit a pull request with your changes.

## Local configuration

Before running the solution, read the **Summary** and the **Guides** table in [README.md](README.md) (or [README-EN.md](README-EN.md)):

1. [DATABASE.md](docs/DATABASE.md) — install SQL Server Developer/Community and create `PropertyManagerDB` with the scripts in the documented order.
2. [LOCAL-SETUP.md](docs/LOCAL-SETUP.md) — ports (API 4300 / UI 4310), per-project user-secrets, startup and troubleshooting.
3. [SYNCFUSION-LICENSES.md](docs/SYNCFUSION-LICENSES.md) — which Syncfusion key/version/edition to choose. The repo currently uses **35.2.12**; a key must match the version of the NuGet packages.
4. [PRODUCTION.md](docs/PRODUCTION.md) — GitHub Secrets and IIS deployment (maintainers only).

GitHub Secrets are **not copied by a clone**: use your own `dotnet user-secrets`, your own database, SMTP credentials and your own Syncfusion licences for the API and the UI. Never commit keys, connection strings or tenant data.
