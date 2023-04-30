using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IImovelRepository 
    {
        Task<string> GetNumeroPorta(int idImovel);
        Task<int> GetCodigo_Imovel(int Id);
        Task<string> GetDescricao_Imovel(int Id);
        Task<int> InsereImovel(NovoImovel novoImovel);
        Task<AlteraImovel?> AtualizaImovel(AlteraImovel alteraImovel);
        Task ApagaImovel(int id);
        Task<ImovelVM> GetImovel_ById(int id);
        Task<IEnumerable<ImovelVM>> GetAll();
        Task<IEnumerable<LookupTableVM>> GetPropertiesAsLookupTables();
        Task<bool> CanPropertyBeDeleted(int id);
        Task<bool> TableHasData();
    }
}