using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.SituacaoFracao;
using PropertyManagerFL.Application.ViewModels.Despesas;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
	public interface IFracaoService
	{
        Task<bool> InsereFracao(FracaoVM fracao);
        Task<bool> AtualizaFracao(int id, FracaoVM fracao);
        Task ApagaFracao(int id);
        Task<List<FracaoVM>> GetResumedData();
        Task<bool> MarcaFracaoComoAlugada(int Id);
        Task<bool> MarcaFracaoComoLivre(int Id);
        Task<bool> FracaoEstaLivre(int Id);
        Task<List<SituacaoFracaoVM>> GetSituacaoFracao();
        Task<int> GetIDSituacao_ByDescription(string sDescricao);
        Task<string> GetNomeFracao(int IdFracao);
        Task<IEnumerable<LookupTableVM>> GetFracoes_ComArrendamentoCriado();
        Task<IEnumerable<LookupTableVM>> GetFracoes_Disponiveis(int IdImovel = 0);
        Task<IEnumerable<LookupTableVM>> GetFracoes(int idImovel = 0);
        Task<FracaoVM> GetFracao_ById(int id);
        Task<FracaoVM> GetUnit_ById(int id);
        int GetFirstId();
        Task<IEnumerable<FracaoVM>> GetAll();
        Task<IEnumerable<FracaoVM>> GetFracoes_Imovel(int id = 0);

        Task<IEnumerable<ImagemFracao>> GetImages_ByUnitId(int id);
        Task<int> InsereImagemFracao(NovaImagemFracao imagem);
        Task<bool> AtualizaImagemFracao(int id, AlteraImagemFracao imagem);
        Task ApagaImagemFracao(int id);
        Task<ImagemFracao> GetImage_ByUnitId(int id);
        Task<bool> AtualizaValorRenda(int id, decimal novoValorRenda);

        Task<IEnumerable<LookupTableVM>> GetFracoes_SemContrato(int propertyId);
        Task<IEnumerable<LookupTableVM>> GetFracoes_WithDuePayments();
        Task<int> InsereApoliceFracao(Seguro apolice);
        Task<bool> AtualizaApoliceFracao(Seguro seguro);
        Task ApagaApoliceFracao(int id);
        Task<SeguroVM> GetApoliceFracao_ById(int id);
        Task<IEnumerable<SeguroVM>> GetAllApolices();
        Task<IEnumerable<InsuranceResults>> GetUnitsInsuranceData();
    }
}