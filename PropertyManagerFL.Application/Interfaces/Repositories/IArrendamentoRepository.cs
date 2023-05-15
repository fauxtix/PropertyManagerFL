using PropertyManagerFL.Core.Entities;
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
