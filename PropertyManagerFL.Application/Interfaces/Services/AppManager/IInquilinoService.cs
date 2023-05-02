using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
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

    }
}