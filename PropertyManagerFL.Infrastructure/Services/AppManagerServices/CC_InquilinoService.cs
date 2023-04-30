using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Shared.ViewModels.Inquilinos;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
	public class CC_InquilinoService : ICC_InquilinoService
	{
		private readonly ICC_InquilinoRepository repo;

		public CC_InquilinoService(ICC_InquilinoRepository repoInquiilino)
		{
			repo = repoInquiilino;
		}

		public void Delete(CC_Inquilino entity)
		{
			repo.Delete(entity);
		}

		/// <summary>
		/// Devolve numro do primeiro Id de uma tabela
		/// Verifica primeiro se tabela tem dados (método TableHasData)
		/// </summary>
		/// <returns>long</returns>
		public int GetFirstId()
		{
			return repo.GetFirstId();
		}

		public long Insert(CC_Inquilino entity)
		{
			return repo.Insert(entity);
		}

		public IEnumerable<CC_Inquilino> Query(string where = "")
		{
			return repo.Query(where);
		}

		public CC_Inquilino Query_ById(int id)
		{
			return repo.Query_ById(id);
		}

		public bool TableHasData()
		{
			return repo.TableHasData();
		}

		public void Update(CC_Inquilino entity)
		{
			repo.Update(entity);
		}
	}
}
