using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IFiadorRepository
    {
        Task ApagaFiador(int id);
        Task<Fiador> AtualizaFiador(AlteraFiador Fiador);
        Task AtualizaSaldo(int id, decimal saldoCorrente);
        Task<bool> CanFiadorBeDeleted(int id);
        Task<IEnumerable<FiadorVM>> GetAll();
        Task<IEnumerable<LookupTableVM>> GetFiadoresDisponiveis();
        Task<IEnumerable<LookupTableVM>> GetFiadores_ForLookUp();
        Task<FiadorVM> GetFiadorInquilino(int id);
        Task<FiadorVM> GetFiador_ById(int idFiador);
        int GetFirstIdFiador();
        int GetFirstId_Fiador();
        Task<string> GetNomeFiador(int id);
        Task<int> InsereFiador(NovoFiador Fiador);
        Task<bool> TableHasData();
    }
}