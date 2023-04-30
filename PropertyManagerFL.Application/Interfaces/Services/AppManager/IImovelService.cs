using PropertyManagerFL.Application.ViewModels.GeoApi.CodigosPostais;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
	public interface IImovelService
	{
		Task<string> GetNumeroPorta(int idImovel);
		Task<int> GetCodigo_Imovel(int Id);
		Task<string> GetDescricao_Imovel(int Id);
		Task<bool> InsereImovel(ImovelVM imovel);
		Task<bool> AtualizaImovel(int id, ImovelVM imovel);
		Task<bool> ApagaImovel(int id);
		Task<ImovelVM> GetImovel_ById(int id);
        Task<IEnumerable<ImovelVM>> GetAll();
        Task<IEnumerable<LookupTableVM>> GetPropertiesAsLookupTables();
        Task<GeoApi_CP7> GetFreguesiaConcelho(string? pst, string? pstEx);
    }
}