using PropertyManagerFL.Application.Interfaces.SqLiteGenerics;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
	public interface ITipoRecebimentoRepository : IBaseRepository<TipoRecebimento>
	{
		int GetID_ByDescription(string sDescricao);
		IEnumerable<LookupTableVM> GetOutroTipoRecebimento();
	}
}
