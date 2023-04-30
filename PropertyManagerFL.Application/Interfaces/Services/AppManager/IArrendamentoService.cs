using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
	public interface IArrendamentoService
	{
        Task<bool> RequirementsMet();

        Task<bool> InsertArrendamento(ArrendamentoVM arrendamento);
        Task<bool> UpdateArrendamento(int id, ArrendamentoVM arrendamento);
        Task<bool> DeleteArrendamento(int id);
        Task<List<ArrendamentoVM>> GetResumedData();
        Task<bool> ContratoEmitido(int Id);
        Task MarcaContratoComoEmitido(int Id, string docGerado);
        Task MarcaCartaAtualizacaoComoEmitida(int Id, string docGerado);
        Task MarcaContratoComoNaoEmitido(int Id);
        Task<string> GetDocumentoGerado(int Id);
        Task<bool> RenovacaoAutomatica(int Id);
        Task<decimal> TotalRendas();
        void CriaRegistoHistorico(Arrendamento arrendamento); // incluído no módulo de libertação de fração
        Task GeraMovimentos(ArrendamentoVM arrendamento, int IdFracao);
        Task<bool> ChildrenExists(int IdFracao);
        Task<string> GetNomeInquilino(int Id);
        Task CheckNewRents();
        Task<bool> ArrendamentoExiste(int IdFracao);
        Task<ArrendamentoVM> GetArrendamento_ById(int id);
        Task<IEnumerable< ArrendamentoVM>> GetAll();

        Task<int> GetLastId();

        Task<string> GetPdfFilename(string? filename);
        Task<int> GetIdInquilino_ByUnitId(int unitId);

        Task<bool> UpdateLastPaymentDate(int id, DateTime date);
        Task<DateTime> GetLastPaymentDate(int unitId);

        // Coeficientes de atualização de rendas
        Task<bool> InsertRentCoefficient(CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas);
        Task<bool> UpdateRentCoefficient(int id, CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas);
        Task<float> GetCurrentRentCoefficient(string? ano);
        Task<IEnumerable<CoeficienteAtualizacaoRendas>> GetRentUpdatingCoefficients();
        Task<CoeficienteAtualizacaoRendas> GetRentUpdatingCoefficient_ById(int id);

        // Atualização de rendas
        Task<string> EmiteCartaAtualizacao(CartaAtualizacao DadosAtualizacao);
        Task<CartaAtualizacao> GetDadosCartaAtualizacao(ArrendamentoVM DadosArrendamento);
        Task<bool> CartaAtualizacaoRendasEmitida(int ano);
        Task<CartaOposicaoRenovacaoContrato> GetDadosCartaOposicaoRenovacaoContrato(ArrendamentoVM DadosArrendamento);
        Task<string> EmiteCartaOposicaoRenovacaoContrato(CartaOposicaoRenovacaoContrato DadosCartaOposicao);
        Task<bool> RegistaCartaOposicao(int Id, string docGerado);
        Task<bool> RegistaProcessamentoAtualizacaoRendas();
        Task RegistaCartaAtrasoRendas(int Id, string docGerado);
        Task<bool> VerificaSeExisteCartaRevogacao(int id);
        Task<bool> VerificaSeExisteRespostaCartaRevogacao(int id);

        Task<CartaRendasAtraso> GetDadosCartaRendasAtraso(ArrendamentoVM DadosArrendamento);
        Task<string> EmiteCartaRendasAtraso(CartaRendasAtraso dadosCartaAtraso);
        Task<bool> VerificaEnvioCartaAtrasoEfetuado(int id);
        Task<bool> RegistaCartaAtraso(int id, string docGerado);
        Task MarcaCartaAtrasoRendaComoEmitida(int Id, string docGerado);
        Task<IEnumerable<LookupTableVM>> GetApplicableLaws();
        Task<bool> ExtendLeaseTerm(int Id);
    }
}