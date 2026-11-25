# User Manual — Property Manager

> Practical guide to the **Property Manager** application: properties, units, tenants, lease agreements, rents, payments, expenses, documents, messages, calendar and administration.
>
> Written for end users and administrators. No programming knowledge is required.
>
> 💡 This manual is available **inside the application**: click the `?` button in the **top bar** to open it in a dialog without leaving the page you are on.

---

## Contents

- [1. Getting started](#primeiros-passos)
- [2. Overview and navigation](#navegacao)
- [3. Dashboard](#dashboard)
- [4. Properties](#imoveis)
- [5. Units](#fracoes)
- [6. Tenants](#inquilinos)
- [7. Leases](#arrendamentos)
- [8. Rents and payments](#rendas-pagamentos)
- [9. Expenses](#despesas)
- [10. Contacts, owner and documents](#contactos-documentos)
- [11. Messages, notifications and calendar](#mensagens)
- [12. Letter templates](#modelos-cartas)
- [13. Calculators and maps](#calculadoras)
- [14. Administration](#administracao)
- [15. Troubleshooting](#resolucao)
- [16. Frequently asked questions](#faq)
- [Appendix A — Main routes](#rotas)

---

## 1. Getting started {#primeiros-passos}

### 1.1 Signing in

1. Open the application address in your browser.
2. Enter your **email** and **password**.
3. Click **Sign in**.
4. If you do not have an account yet, use the **register** link on the sign-in screen (in some installations accounts are created by the administrator).

### 1.2 Choosing a language

1. In the top bar, use the **language selector**.
2. Choose **Portuguese**, **English**, **Spanish** or **French**.
3. The choice is saved in the application settings and reloads the interface.

> 💡 Content you enter (names, addresses, descriptions) is **not translated** — only the interface. You can have the interface in English and your data in Portuguese.

### 1.3 Creating your first property

1. In the sidebar, open **Properties** (`/imoveis`).
2. Click **New** (the green `+` button).
3. Fill in the identification details and the **address** of the property.
4. Click **Save**.

### 1.4 Creating your first unit

1. Open **Properties** and select the property.
2. Add a **unit** — for example an apartment or a shop.
3. Fill in the unit identification, the **rent**, the **type** and the **status**.
4. You can upload **images** of the unit in the image manager (`/fracoes/imagesmanager`).

### 1.5 Creating your first tenant

1. In the sidebar, open **Tenants** (`/inquilinos`).
2. Click **New** and fill in the personal and contact details.
3. If applicable, associate a **guarantor**.
4. Save.

### 1.6 Creating your first lease

1. Open **Leases** (`/arrendamentos`).
2. Click **New**.
3. Associate the **unit** and the **tenant**, then enter the start date, term and rent amount.
4. Save. From here you can generate the contract and related letters.

### 1.7 «I want to do X → I go to Y» table

| I want to… | Go to… | Route |
|---|---|---|
| See the general summary | Home | `/` |
| Manage properties | Properties | `/imoveis` |
| Manage units and images | Properties → unit | `/fracoes/viewunits` |
| Manage tenants | Tenants | `/inquilinos` |
| Manage leases | Leases | `/arrendamentos` |
| Process monthly rents | Payments → Process rents | `/monthlyrentprocessing` |
| Record payments | Payments → Maintenance | `/recebimentos/manutencao` |
| Manage expenses | Expenses → Maintenance | `/despesas/manutencao` |
| Edit letters | Administration → Letter templates | `/LetterTemplates` |
| See notifications | Bell in the top bar | `/notifications` |
| Open the calendar | Calendar | `/scheduler` |
| Settings and backups | Maintenance | `/admin/appsettings` |

---

## 2. Overview and navigation {#navegacao}

### 2.1 Top bar

It contains:

- The **sidebar toggle** button.
- The application name.
- The **help** button (`?`) — opens this manual without leaving the page.
- The **notifications bell**.
- The **language selector**.
- The **username** (link to manage the account).
- The **light/dark theme** button.
- The **sign out** option.

### 2.2 Sidebar

The menu is organised into groups:

- **General** — Home, Properties, Tenants, Leases, Expenses, Payments, Calculators, Contacts, Documents, Messages and Calendar.
- **Administration / maintenance** — Logs, Settings, Database backup and Files backup.

Groups with children expand/collapse when clicked. The active item is highlighted.

### 2.3 Grids

Almost all listings use a grid with a toolbar. The most common operations are:

| Action | How |
|---|---|
| Search | Search box in the grid toolbar |
| Sort | Click a column header |
| Export | `PdfExport` or `ExcelExport` in the toolbar |
| Expand/collapse rows | Expand/collapse buttons in the toolbar |
| Create | **New** button |
| Edit / delete | Buttons on the row or the context menu |

> 💡 In some grids, right-clicking opens a **context menu** with quick actions (late-payment letters, rent updates, etc.).

---

## 3. Dashboard {#dashboard}

The home page (`/`) shows the general indicators: a summary of properties, tenants, leases, rents and expenses, plus alerts and recent activity.

- Use the cards to jump to the corresponding area.
- If a figure is out of date, refresh the page (F5).
- The dashboard **alerts** are the same ones shown by the notifications bell — they are not different warnings per page.

---

## 4. Properties {#imoveis}

Page: **Properties** (`/imoveis`). An alternative view is available at `/properties`.

### 4.1 What is a property

A **property** is the top level: a building, a house or a set of units. **Units** live inside it.

### 4.2 Create and edit

1. Click **New**.
2. Fill in the identification and address.
3. Save.
4. To change it, use the edit button on the grid row.

### 4.3 Export and search

The grid allows searching, sorting and exporting the listing to PDF.

---

## 5. Units {#fracoes}

Pages: **Units** (`/fracoes/viewunits`) and **Image manager** (`/fracoes/imagesmanager`).

### 5.1 What is a unit

A **unit** is a division inside a property — for example an apartment, a shop or a garage. The **lease** and the **rent** are associated with the unit.

### 5.2 Main fields

- Identification and description.
- **Unit type** and **status** (for example available or rented).
- **Rent** and **IMI** (property tax) value.
- Associated **insurance**.
- **Active / inactive**.

### 5.3 Images

1. Open the image manager (`/fracoes/imagesmanager`).
2. Select the unit.
3. Upload one or more images.
4. Confirm the upload; the images become available in the unit view.

> 💡 If the upload fails with `Access to the path ... is denied`, the server upload folder is not writable — ask the administrator to fix the IIS permissions.

---

## 6. Tenants {#inquilinos}

Page: **Tenants** (`/inquilinos`).

### 6.1 Creating a tenant

1. Click **New**.
2. Fill in the **name**, **contacts** and identification details.
3. If applicable, associate a **guarantor**.
4. Save.

### 6.2 The tenant record

The record brings together:

- Personal and contact details.
- Uploaded **documents** (`DocumentosInquilino`).
- **Current balance** (`SaldoCorrenteInquilino`) — what is owed or in credit.
- Associated leases.

### 6.3 Updating the rent amount

There are two ways:

- **Manually**: in the tenant context menu, choose **Update rent amount** (`/updaterentvalue/{id}`).
- **By letter**: via **Leases → Rent update** (`/updaterentsletter`).

### 6.4 Quick letters

The tenant context menu can generate directly:

- **Late-payment letter**.
- **Cancellation / contract opposition letter**.
- **Rent update letter**.

> ⚠️ Before generating a letter, check that the lease and tenant details are correct — the letter uses the stored values.

---

## 7. Leases {#arrendamentos}

Page: **Leases** (`/arrendamentos`).

### 7.1 What is a lease

A lease links a **tenant** to a **unit**, with a start date, term and rent amount. The application warns when a contract is nearing its end or when rents are overdue.

### 7.2 Creating a contract

1. Click **New**.
2. Select the **unit** and the **tenant**.
3. Enter the **start date**, **term** and **rent amount**.
4. Save.

### 7.3 Grid actions

The toolbar and context menu allow you to:

- **View the contract** (`View contract`).
- **Extend the contract term**.
- **Update rents** and generate the corresponding letter.
- Check the tenant's **current balance** (`/saldocorrenteinquilino`).

### 7.4 Update coefficients

In **Leases → Coefficients** (`/updatecoefficients`) you can manage the coefficients used to calculate annual rent updates.

> 💡 Depending on the installation settings, the *Leases* menu may appear with submenus (Maintenance and Rent update) instead of a single item.

---

## 8. Rents and payments {#rendas-pagamentos}

Pages: **Payments → Maintenance** (`/recebimentos/manutencao`), **Process rents** (`/monthlyrentprocessing`) and **Payments dashboard** (`/payments/dashboard`).

### 8.1 Recording a payment

1. Open **Payments → Maintenance**.
2. Click **New** (or equivalent).
3. Select the **tenant**, the **concept**, the **amount** and the **date**.
4. Save.

The grid allows searching, sorting and exporting to Excel.

### 8.2 Monthly rent processing

1. Open **Payments → Process rents** (`/monthlyrentprocessing`).
2. Confirm the **month/year** and the criteria.
3. Review the preview of the rents to be processed.
4. Confirm the processing.

> ⚠️ Batch processing creates several records at once. Always review the list before confirming.

### 8.3 Payments dashboard

The dashboard (`/payments/dashboard`) summarises payments by period and helps you track what has been paid and what is outstanding.

---

## 9. Expenses {#despesas}

Pages: **Expenses → Maintenance** (`/despesas/manutencao`), **Expense types** (`/despesas/tipodespesas`) and **Expenses dashboard** (`/expenses/dashboard`).

### 9.1 Recording an expense

1. Open **Expenses → Maintenance**.
2. Click **New**.
3. Fill in the **description**, **amount**, **date** and **expense type**.
4. Save.

### 9.2 Expense types

In **Expense types** you can create, edit and organise the categories used when recording expenses.

### 9.3 Charts

The dashboard shows expenses by type and over time (line and pie charts). Hover over a point to see the exact value.

---

## 10. Contacts, owner and documents {#contactos-documentos}

### 10.1 Contacts

Page: `/contactos`. Stores useful contacts (administrators, condominiums, suppliers, etc.).

### 10.2 Owner

Pages: `/Proprietario` and `/ProprietarioPage`. Contain the property owner's details.

### 10.3 Documents

Page: **Documents** (`/documentos`). Lets you register and upload general application documents. For tenant documents, use the tenant record.

---

## 11. Messages, notifications and calendar {#mensagens}

### 11.1 Notifications

The **bell** in the top bar opens the notification centre. You can also open the full page at `/notifications`.

- Notifications include operational alerts (appointments, rents, letters, contracts).
- Each alert can include a **suggested action** — for example, open the tenant record, record a payment or generate the corresponding letter.
- Alerts include: overdue rent, a missing overdue-rent letter, rent due soon, a revocation letter with no reply, and a lease marked as issued without a generated document.
- Unread messages are highlighted.
- Opening a notification marks it as read.

### 11.2 Messages

Page: **Messages** (`/messages-modern`; the legacy version is at `/messages`). Lets you send and read messages.

### 11.3 Calendar

Page: **Calendar** (`/scheduler`). Lets you record appointments and view them in a calendar, with event search (`/scheduler/search-events`).

---

## 12. Letter templates {#modelos-cartas}

Page: **Letter templates** (`/LetterTemplates`).

### 12.1 What templates are

They are `.dotx` files used to automatically generate letters and contracts (late notices, rent updates, cancellations, lease agreements, etc.).

### 12.2 Editing a template

1. Open **Letter templates**.
2. Select the template from the list.
3. Edit the document in the text editor.
4. To see the result with sample values, use **Preview**.
5. **Save** only when you are satisfied.

### 12.3 Merge fields

Templates contain **merge fields** (for example `«NomeInquilino»`) that are filled with the lease/tenant data when the letter is generated. Do not delete these fields unless necessary.

> ⚠️ Deleting a template is permanent. Always confirm before deleting.

---

## 13. Calculators and maps {#calculadoras}

### 13.1 Calculators

The **Calculators** menu includes:

- **IMI** (`/CalculadoraIMI`) — municipal property tax calculation.
- **VPT** (`/CalculadoraVPT`) — taxable property value calculation.
- **IMI coefficient management** (`/ManutencaoCoeficientesIMI`).

### 13.2 Maps

The pages `/maps` and `/gmap` show the location of properties on a map.

---

## 14. Administration {#administracao}

### 14.1 Settings

Page: **Settings** (`/admin/appsettings`). Configures general parameters such as the default language and automatic rent adjustment.

### 14.2 Backups

- **Database** (`/BackupDatabase`) — creates a copy of the database.
- **Files / uploads** (`/backupuploads/1`) — creates copies of the uploaded file folders.

> 💡 Back up regularly and store copies somewhere other than the server.

### 14.3 Logs

- **Application logs** (`/admin/viewapplogs`) — application errors, warnings and events.
- **Login information** (`/admin/logininfo`) — records of sign-ins.

### 14.4 Tables and dashboards

- **Administration panel** (`/admin/adminpanel`) and **table configuration** (`/admin/configapptables`).
- **Summary dashboard** (`/summarydashboard`).

---

## 15. Troubleshooting {#resolucao}

| Symptom | Likely cause | What to do |
|---|---|---|
| «Access to the path ... is denied» when uploading files | The server upload folder is not writable | Ask the administrator to fix the IIS permissions |
| Grid does not load | API unavailable or database connection failed | Check the logs (`/admin/viewapplogs`) and try again |
| Manual does not open | `docs/Help*.md` file missing on the server | Make sure the `docs` folder was published with the application |
| Data «disappeared» | Active grid filter or a different user | Clear the filters and confirm which account you signed in with |
| Buttons not responding | Page loaded before the Blazor connection was ready | Wait a second and try again; refresh if it persists |

---

## 16. Frequently asked questions {#faq}

**Do I need to know how to program to use the application?**
No. Everything is done through forms and grids.

**Can I use the application in another language?**
Yes. Portuguese, English, Spanish and French. Choose the language selector in the top bar.

**Is my data deleted when I change the language?**
No. Only the interface changes.

**What happens if I delete a tenant with contracts?**
The application warns or prevents the deletion, depending on the configured rules. Resolve the associated contracts first.

**How do I know whether a rent has been paid?**
Check the tenant's **current balance** and the **payments dashboard**.

**Is the manual updated without restarting the application?**
Yes. The file is read from disk each time you open the dialog.

---

## Appendix A — Main routes {#rotas}

| Area | Route |
|---|---|
| Home | `/` |
| Properties | `/imoveis`, `/properties` |
| Units | `/fracoes/viewunits`, `/fracoes/imagesmanager` |
| Tenants | `/inquilinos`, `/Inquilinos/Detalhe` |
| Leases | `/arrendamentos`, `/updaterentsletter`, `/updatecoefficients` |
| Rents | `/monthlyrentprocessing`, `/saldocorrenteinquilino` |
| Payments | `/recebimentos/manutencao`, `/payments/dashboard` |
| Expenses | `/despesas/manutencao`, `/despesas/tipodespesas`, `/expenses/dashboard` |
| Contacts | `/contactos` |
| Documents | `/documentos` |
| Messages | `/messages-modern`, `/messages` |
| Notifications | `/notifications` |
| Calendar | `/scheduler` |
| Letter templates | `/LetterTemplates` |
| Calculators | `/CalculadoraIMI`, `/CalculadoraVPT`, `/ManutencaoCoeficientesIMI` |
| Maps | `/maps`, `/gmap` |
| Settings | `/admin/appsettings` |
| Backups | `/BackupDatabase`, `/backupuploads/1` |
| Logs | `/admin/viewapplogs`, `/admin/logininfo` |
