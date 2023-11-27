# PropertyManagerFL

Application designed to assist landlords in managing their properties and rentals. With a user-friendly interface and a range of helpful features, PropertManagerFL (PMFL) serves as a centralized platform for landlords, to streamline their property management tasks.

# Key Features

- **Centralized Property and Tenant Information**: PMFL allows landlords to store all property and tenant details in one convenient location. From property specifications to tenant records and contact information, everything is easily accessible whenever you need it.

- **Rent Lease Creation**: PMFL simplifies the process of creating rent leases; new and existing ones.

- **Rent Payment Tracking**: PMFL provides a comprehensive system for tracking rent payments, helping landlords stay updated on transactions and promptly address any overdue payments.
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

# Localization / globalization
The application supports Portuguese, English, French and Spanish languages (nearing completion).

Some of the tables used in the application (mainly **lookup** ones) will require user intervention, since Portuguese has been used as the native language for filling in/configuring them.

The same goes for the wording of the various letters sent to tenants, as each country may have different templates/rules to apply, which will have to be adapted accordingly.
It's a simple process to implement:
- use Winword to open each template document used in the application (templates / dotx);
- copy its text, and use a translator (Google, DeepL, ...) to adapt it to your needs;
- copy the translated text and replace the text in the letter;
- finish the process by saving the updated documents (overwriting the existing ones).

# Database
For the database structure (tables, functions, stored procedures,...), you can access the scripts at https://github.com/fauxtix/PropertyManagerFL/tree/master/SqlData_Script
#

## 🌟 Contributing

Contributions to this project are welcome! If you encounter any issues or have suggestions for improvement, please open an issue on the GitHub repository: https://github.com/fauxtix/PropertyManagerFL/Issues 

Fork the project (https://github.com/fauxtix/PropertyManagerFL/fork)

Create a branch for your modification (git checkout -b fauxtix/PropertyManagerFL)

Commit (git commit -am 'added a new feature - some files changed')

Push_ (git push origin fauxtix/PropertyManagerFL)

Create a new Pull Request

More info: https://www.digitalocean.com/community/tutorials/how-to-create-a-pull-request-on-github

When contributing code, please follow the existing code style and submit a pull request with your changes.

## ⚖ License

The Property Manager FL project is licensed under the MIT License. You can find more information in the [LICENSE](https://github.com/fauxtix/PropertyManagerFL/blob/main/LICENSE) file.

## 📞 Contact

If you have any questions or need further assistance, you can reach out to the project maintainer:

- 👨‍💻 Maintainer: Fausto Luís
- ✉ Email: fauxtix.luix@hotmail.com

Feel free to contact me with any feedback or inquiries.

Thank you for using PropertyManagerFL!

#

# Screenshots

- **Main screen**
  #
![Main](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/61fe28f7-9703-4a8b-922a-9b948084db15)

- **Properties**
 #
![Properties](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/d1cf7d8f-900d-49c3-9065-82a86a92803a)
![Properties_edit_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/13edd0bd-3925-4a16-8011-fb515ffdd239)
![Properties_edit_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/8dbe7cfa-bea7-4260-96e6-75390c6fcfdc)
![Properties_edit_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/135498e8-f373-41d8-bf8f-35d3f6fb5a20)

- **Tenants**
  #
![Tenants](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/40d50281-d7ae-4acb-8b75-f40087801743)
![Tenants_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6edc7a70-b197-4e3b-b9e6-f540811d74ba)
![Tenants_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/11dce827-b339-4ac2-a8b8-b72bab92b956)
![Tenants_documents_create](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/ac2ab8e7-fc90-4088-8a82-1adf9c930094)
![Tenants_documents_create_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/3966d0c8-a3ad-4236-a918-671cbf54b938)
![Tenants_documents_create_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/9fb35174-0c4a-4ecd-9105-4f75f91cf465)
![Tenants_guarantor](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/d8742713-ecd6-48eb-b800-8eaed7c1cf3f)

- **Units**
 #
![Unit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/54a0caae-2265-49e6-8b87-e500d43ab15d)
![Unit_Images](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/77df0a32-2d8a-4699-ba78-2b66a8d00279)

- **Monthly payments**
 #
![MonthlyRentPayment_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/67874f25-5fb6-4e66-8cfd-1783c5eb1c29)
![MonthlyRentPayment_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6169ae59-9c2d-4db2-b407-f8e2d7bc6e58)

- **Payments**
  #
![Payments_Dashboard_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/d4fe17c6-6977-4c60-b2a2-c5f3e22d7cb3)
![Payments_Dashboard_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/9961bd55-7314-45d0-bb26-f035b3800627)
![Payments](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/cdcc4ce8-ed48-48e9-8241-fe3a4da2770e)
![Payments_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/f904752f-d38a-4cab-ac5a-bad459f6fac0)
![Payments_new_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/48d94d8e-6d17-4ecb-a89d-abc3a73815a7)
![Payments_new_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/0cfa598b-43ff-4398-9d9c-0523cfdaa363)
![Payments_new_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/aaf6cf89-7dca-46e2-a54f-1cf9984b56b4)

- **Leases**
 #
![Leases](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/b573364b-939b-4f70-be09-acc9634093f5)
![Leases_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/85d07a7a-8c79-4fba-a252-359cf0a1e593)

- **Expenses**
 #
![Expenses_maintenance](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/b164b028-23b8-4b6c-b057-e70d2f7ba095)
![Expenses_maintenance_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6dda062b-02e5-410c-a5ae-bc1bf961677c)
![Expenses_maintenance_subCategories_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/c47278d5-e37e-44d0-9132-1f1cf8ec1d39)
![Expenses_maintenance_subCategories_new](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/7c443597-3d3e-41f3-8251-f84374f5a481)

![Expenses_dashboard_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/66b25d6a-5928-44cd-a489-5d48cd5cde5f)
![Expenses_dashboard_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6e7a0cd7-39d2-4690-9dca-1f7de3c34b5d)
![Expenses_dashboard_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/37b05f59-b63b-47c9-9fbc-4552d6e27fa3)

- **Scheduler**
  #
![Scheduler_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/8419c317-ec2a-472e-8d4a-cb174b62a6f7)

- **Communicating with tenants**
  #
![Messages](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/33ed9de2-d80e-418f-bb00-df5d2a15ad32)

- **Contacts**
  #
![Contacts](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/c32585b3-b6b7-4042-bda8-296e79ff76c9)

