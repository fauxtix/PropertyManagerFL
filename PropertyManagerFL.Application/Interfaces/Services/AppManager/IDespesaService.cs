using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Despesas;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
	public interface IDespesaService
	{
        Task<int> Insert(DespesaVM expense);
        Task<bool> Update(int id, DespesaVM expense);
        Task<bool> Delete(int id);
        Task<IEnumerable<DespesaVM>> GetAll();
        Task<DespesaVM> GetDespesa_ById(int ID);
        Task<IEnumerable<TipoDespesaVM>> GetTipoDespesa_ByCategoriaDespesa(int ID);
        List<DespesaVM> GetResumedData();
        decimal TotalDespesas(int iTipoDespesa = 0);
        List<DespesaVM> Query_ByYear(string sAno);
        Task<bool> AreThereProperties();
    }
}