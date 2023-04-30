using PropertyManagerFL.Application.Interfaces.SqLiteGenerics;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Despesas;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
	public interface IDespesaRepository
	{
		Task<int> Insert(NovaDespesa expense);
		Task<bool> Update(AlteraDespesa expense);
		Task Delete(int id);
		Task<IEnumerable<DespesaVM>> GetAll();
		Task<DespesaVM> GetDespesa_ById(int ID);
        Task<IEnumerable< TipoDespesaVM>> GetTipoDespesa_ByCategoriaDespesa(int ID);
        List<DespesaVM> GetResumedData();
		decimal TotalDespesas(int iTipoDespesa = 0);
		List<DespesaVM> Query_ByYear(string sAno);
	}
}
