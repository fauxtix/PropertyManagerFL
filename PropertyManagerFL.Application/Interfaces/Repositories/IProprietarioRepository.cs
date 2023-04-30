using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Proprietarios;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IProprietarioRepository
    {
        Task<int> Insert(NovoProprietario entity);
        Task<bool> Update(AlteraProprietario entity);
        Task Delete(int id);
        int GetFistId();
        IEnumerable<Proprietario> Query(string where = null);
        Task<Proprietario> Query_ById(int Id);
        string RegistoComErros(Proprietario proprietario);
        Task<bool> TableHasData();
        int GetFirstId();
        Task<ProprietarioVM> GetProprietario_ById(int id);
    }
}
