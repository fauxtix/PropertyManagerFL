using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
	public interface IFiadorService
	{
        Task<bool> ApagaFiador(int id);
        Task<bool> AtualizaFiador(int id, FiadorVM Fiador);
        Task<IEnumerable<FiadorVM>> GetAll();
        Task<IEnumerable<LookupTableVM>> GetFiadoresDisponiveis();
        Task<IEnumerable<LookupTableVM>> GetFiadores_ForLookUp();
        Task<FiadorVM> GetFiador_ById(int idFiador);
        Task<FiadorVM> GetFiador_Inquilino(int id);
        int GetFirstIdFiador();
        int GetFirstId_Fiador();
        Task<string> GetNomeFiador(int id);
        Task<bool> InsereFiador(FiadorVM Fiador);
    }
}