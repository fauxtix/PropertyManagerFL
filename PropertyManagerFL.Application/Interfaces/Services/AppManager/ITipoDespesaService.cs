using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface ITipoDespesaService
    {
        Task<int> Insert(TipoDespesaVM entity);
        Task<bool> Update(int id, TipoDespesaVM entity);
        Task<bool> Delete(int id);
        Task<int> GetFirstId();
        Task<IEnumerable<TipoDespesaVM>> Query(string where = null);
        Task<TipoDespesaVM> Get_ById(int id);
        Task<IEnumerable<TipoDespesaVM>> GetAll();
        string RegistoComErros(TipoDespesa tipoDespesa);
        bool TableHasData();
        Task<int> GetID_ByDescription(string Descricao);
        Task<IEnumerable<TipoDespesaVM>> GetTipoDespesa_ByCategoria(int categoria);
    }
}