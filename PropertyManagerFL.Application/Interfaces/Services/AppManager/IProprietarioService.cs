using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Proprietarios;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface IProprietarioService
    {
        Task<int> Insert(ProprietarioVM entity);
        Task<bool> Update(int id, ProprietarioVM entity);
        Task<bool> Delete(int id);
        Task<int> GetFirstId();
        IEnumerable<Proprietario> Query(string where = null);
        Task<Proprietario> Query_ById(int Id);
        string RegistoComErros(Proprietario proprietario);
        Task<bool> TableHasData();
        Task<ProprietarioVM> GetProprietario_ById(int id);
    }
}