# PropertyManagerFL

Application designed to assist landlords in managing their properties and rentals. With a user-friendly interface and a range of helpful features, PropertManagerFL (PMFL) serves as a centralized platform for landlords to streamline their property management tasks.

# Key Features

- Centralized Property and Tenant Information: PMFL allows landlords to store all property and tenant details in one convenient location. From property specifications to tenant records and contact information, everything is easily accessible whenever you need it.

- Rent Lease Creation: PMFL simplifies the process of creating rent leases.

- Rent Payment Tracking: PMFL provides a comprehensive system for tracking rent payments, helping landlords stay updated on transactions and promptly address any overdue payments.

# Sample Code

Examples:

Tenant's service (interface for dependency injection)

```cs:   
﻿using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    // 'Inquilino' translates to 'Tenant'
    public interface IInquilinoService
    {
        Task<string> GetNomeInquilino(int id);
        int GetFirstId_Inquilino();
        int GetFirstIdInquilino();
        Task AtualizaSaldo(int Id, decimal SaldoCorrente);
        Task<IEnumerable<LookupTableVM>> GetInquilinosDisponiveis();
        Task<IEnumerable<LookupTableVM>> GetInquilinosAsLookup();
        Task<int> GetInquilinoFracao(int ID_Fracao);
        string GetNomeFracao(int IdInquilino, bool bTitular);
        Task<string?> GetUltimoMesPago_Inquilino(int ID_Inquilino);
        Task<IEnumerable<LookupTableVM>> GetInquilinos();
        Task<IEnumerable<InquilinoVM>> GetAll();
        Task<InquilinoVM> GetInquilino_ById(int id);
        Task<bool> InsereInquilino(InquilinoVM inquilino);
        Task<bool> AtualizaInquilino(int id, InquilinoVM inquilino);
        Task<bool> ApagaInquilino(int id);
        Task<int> CriaDocumentoInquilino(DocumentoInquilinoVM documento);
        Task<bool> AtualizaDocumentoInquilino(int id, DocumentoInquilinoVM document);
        Task<DocumentoInquilinoVM> GetDocumentoById(int Id);
        Task<IEnumerable<DocumentoInquilinoVM>> GetDocumentos();
        Task<IEnumerable<DocumentoInquilinoVM>> GetDocumentosInquilino(int id);
        Task<bool> ApagaDocumentoInquilino(int id);
        Task<string> GetPdfFromServer(string foldername, string fileName);
        Task<string> GetServerPdfFileName(string foldername, string filename);
        Task<IEnumerable< FiadorVM>> GeFiadorInquilino_ById(int idInquilino);
        Task<IEnumerable<CC_InquilinoVM>> GetTenantPaymentsHistory(int id);
        Task<IEnumerable<LookupTableVM>> GetInquilinos_SemContrato();
        Task<string> AtualizaRendaInquilino(int Id);
        Task<string> AtualizaRendaInquilino_Manual(int Id, string oldValue, string newValue);
        Task<decimal> GetTenantRent(int Id);
        Task<bool> PriorRentUpdates_ThisYear(int unitId);
        Task<IEnumerable<HistoricoAtualizacaoRendasVM>> GetAllRentUpdates();
        Task<IEnumerable<HistoricoAtualizacaoRendasVM>> GetRentUpdates_ByTenantId(int tenantId);
        Task<IEnumerable<RentAdjustmentsVM>?> GetRentAdjustments();
        Task<IEnumerable<LatePaymentLettersVM>> GetLatePaymentLetters();
        Task<CartaAtualizacao> GetDadosCartaAtualizacaoInquilino(ArrendamentoVM DadosArrendamento);
        Task<string> EmiteCartaAtualizacaoInquilino(CartaAtualizacao DadosAtualizacao);
        Task<bool> CriaCartaAtualizacaoInquilinoDocumentosInquilino(int tenantId, string docGerado);
    }
}
```

Lease service
```cs:
﻿using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.Inquilinos;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IArrendamentoRepository
    {
        Task<bool> RequirementsMet();
        Task<int> InsertArrendamento(NovoArrendamento arrendamento);
        Task<bool> UpdateArrendamento(AlteraArrendamento arrendamento);
        Task DeleteArrendamento(int id);
        Task<List<ArrendamentoVM>> GetResumedData();
        Task<bool> ContratoEmitido(int Id);
        Task<bool> CartaAtualizacaoRendasEmitida(int ano);
        void MarcaContratoComoEmitido(int Id, string docGerado);
        void MarcaContratoComoNaoEmitido(int Id);
        string GetDocumentoGerado(int Id);
        bool RenovacaoAutomatica(int Id);
        decimal TotalRendas();
        void CriaRegistoHistorico(Arrendamento arrendamento); // incluído no módulo de libertação de fração
        Task GeraMovimentos(Arrendamento arrendamento, int IdFracao);
        Task<bool> ChildrenExists(int IdFracao);
        Task<string> GetNomeInquilino(int Id);
        Task<int> GetIdInquilino(int tenantId);
        void CheckNewRents();
        bool ArrendamentoExiste(int IdFracao);
        Task<ArrendamentoVM> GetArrendamento_ById(int id);
        Task<IEnumerable<ArrendamentoVM>> GetAll();
        Task<int> GetLastId();
        Task UpdateLastPaymentDate(int id, DateTime date);
        Task<DateTime> GetLastPaymentDate(int unitId);

        Task<float> GetCurrentRentCoefficient(string? ano);
        Task<IEnumerable<CoeficienteAtualizacaoRendas>> GetRentUpdatingCoefficients();
        Task<CoeficienteAtualizacaoRendas> GetRentUpdatingCoefficient_ById(int id);
        Task<int> InsertRentCoefficient(CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas);
        Task<bool> UpdateRentCoefficient(CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas);

        Task<bool> RegistaProcessamentoAtualizacaoRendas();
        Task<bool> MarcaCartaAtualizacaoComoEmitida(int Id, string docGerado);
        Task<bool> RegistaCartaRevogacao(int id, string docGerado);
        Task<bool> VerificaSeExisteCartaRevogacao(int id);
        Task<bool> VerificaSeExisteRespostaCartaRevogacao(int id);

        Task<bool> VerificaEnvioCartaAtrasoEfetuado(int id);
        Task<bool> RegistaCartaAtraso(int id, DateTime? referralDate, string tentativa, string docGerado);
        Task<bool> MarcaCartaAtrasoRendaComoEmitida(int id, string docGerado);
        Task<IEnumerable<LookupTableVM>> GetApplicableLaws();
        Task<CoeficienteAtualizacaoRendas> GetCoefficient_ByYear(int year);
        Task ExtendLeaseTerm(int Id);
    }
}
``

Implementing 'New lease'

```cs:


``


# Screenshots

![Main](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/61fe28f7-9703-4a8b-922a-9b948084db15)

![Properties](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/d1cf7d8f-900d-49c3-9065-82a86a92803a)
![Properties_edit_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/13edd0bd-3925-4a16-8011-fb515ffdd239)
![Properties_edit_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/8dbe7cfa-bea7-4260-96e6-75390c6fcfdc)
![Properties_edit_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/135498e8-f373-41d8-bf8f-35d3f6fb5a20)

![Tenants](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/40d50281-d7ae-4acb-8b75-f40087801743)
![Tenants_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6edc7a70-b197-4e3b-b9e6-f540811d74ba)
![Tenants_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/11dce827-b339-4ac2-a8b8-b72bab92b956)
![Tenants_documents_create](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/ac2ab8e7-fc90-4088-8a82-1adf9c930094)
![Tenants_documents_create_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/3966d0c8-a3ad-4236-a918-671cbf54b938)
![Tenants_documents_create_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/9fb35174-0c4a-4ecd-9105-4f75f91cf465)
![Tenants_guarantor](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/d8742713-ecd6-48eb-b800-8eaed7c1cf3f)

![Unit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/54a0caae-2265-49e6-8b87-e500d43ab15d)
![Unit_Images](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/77df0a32-2d8a-4699-ba78-2b66a8d00279)

![MonthlyRentPayment_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/67874f25-5fb6-4e66-8cfd-1783c5eb1c29)
![MonthlyRentPayment_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6169ae59-9c2d-4db2-b407-f8e2d7bc6e58)

![Payments](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/cdcc4ce8-ed48-48e9-8241-fe3a4da2770e)
![Payments_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/f904752f-d38a-4cab-ac5a-bad459f6fac0)
![Payments_new_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/48d94d8e-6d17-4ecb-a89d-abc3a73815a7)
![Payments_new_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/0cfa598b-43ff-4398-9d9c-0523cfdaa363)
![Payments_new_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/aaf6cf89-7dca-46e2-a54f-1cf9984b56b4)

![Leases](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/b573364b-939b-4f70-be09-acc9634093f5)
![Leases_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/85d07a7a-8c79-4fba-a252-359cf0a1e593)

![Expenses_maintenance](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/b164b028-23b8-4b6c-b057-e70d2f7ba095)
![Expenses_maintenance_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6dda062b-02e5-410c-a5ae-bc1bf961677c)
![Expenses_maintenance_subCategories_edit](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/c47278d5-e37e-44d0-9132-1f1cf8ec1d39)
![Expenses_maintenance_subCategories_new](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/7c443597-3d3e-41f3-8251-f84374f5a481)

![Expenses_dashboard_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/66b25d6a-5928-44cd-a489-5d48cd5cde5f)
![Expenses_dashboard_2](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/6e7a0cd7-39d2-4690-9dca-1f7de3c34b5d)
![Expenses_dashboard_3](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/37b05f59-b63b-47c9-9fbc-4552d6e27fa3)


![Scheduler_1](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/8419c317-ec2a-472e-8d4a-cb174b62a6f7)

![Messages](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/33ed9de2-d80e-418f-bb00-df5d2a15ad32)

![Contacts](https://github.com/fauxtix/PropertyManagerFL/assets/49880538/c32585b3-b6b7-4042-bda8-296e79ff76c9)
