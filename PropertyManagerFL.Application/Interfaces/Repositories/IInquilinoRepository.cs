﻿using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.Recebimentos;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IInquilinoRepository
    {
        Task<int> InsereInquilino(NovoInquilino inquilino);
        Task<Inquilino> AtualizaInquilino(AlteraInquilino inquilino);
        Task ApagaInquilino(int id);
        Task<string> GetNomeInquilino(int id);
        int GetFirstId_Inquilino();
        int GetFirstIdInquilino();
        Task AtualizaSaldo(int Id, decimal SaldoCorrente);
        Task<IEnumerable<LookupTableVM>> GetInquilinosDisponiveis();
        Task<int> GetInquilinoFracao(int ID_Fracao);
        string GetNomeFracao(int IdInquilino, bool bTitular);
        string GetUltimoMesPago_Inquilino(int ID_Inquilino);
        Task<IEnumerable<InquilinoVM>> GetAll();
        Task<IEnumerable<RecebimentoVM>> GetTenantPayments(int tenantId);

        Task<InquilinoVM> GetInquilino_ById(int id);
        Task<bool> CanInquilinoBeDeleted(int id);

        Task<int> CriaDocumentoInquilino(NovoDocumentoInquilino documento);
        Task<DocumentoInquilinoVM> GetDocumentoById(int Id);
        Task<IEnumerable<DocumentoInquilinoVM>> GetDocumentos();
        Task<IEnumerable<DocumentoInquilinoVM>> GetDocumentosInquilino(int id);
        Task<bool> AtualizaDocumentoInquilino(AlteraDocumentoInquilino document);
        Task ApagaDocumentoInquilino(int id);
        Task<IEnumerable<FiadorVM>> GeFiadortInquilino_ById(int idInquilino);
        Task<bool> TableHasData();
        Task<IEnumerable<CC_Inquilino>> GetTenantPaymentsHistory(int id);
        Task<IEnumerable<LookupTableVM>> GetInquilinosAsLookup();
        Task<IEnumerable<LookupTableVM>> GetInquilinos_SemContrato();
        Task AtualizaRendaInquilino(int unitId, DateTime leaseStart, decimal currentRentValue);
        Task<(DateTime, int)> GetLeaseData_ByTenantId(int tenantId);
        Task AtualizaRendaInquilino_Manual(int unitId, DateTime leaseStart, decimal oldValue, decimal newValue);
        Task<decimal> GetTenantRent(int Id);
        Task<bool> PriorRentUpdates_ThisYear(int unitId);
        Task<IEnumerable<HistoricoAtualizacaoRendasVM>> GetAllRentUpdates();
        Task<IEnumerable<HistoricoAtualizacaoRendasVM>> GetRentUpdates_ByTenantId(int tenantId);
        Task<IEnumerable<RentAdjustmentsVM>> GetRentAdjustments();
        Task<IEnumerable<LatePaymentLettersVM>> GetLatePaymentLetters();
        Task<bool> CriaCartaAtualizacaoInquilinoDocumentosInquilino(int tenantId, string docGerado);
        Task<IEnumerable<LatePaymentLettersVM>> GetRentUpdateLetters();
    }
}
