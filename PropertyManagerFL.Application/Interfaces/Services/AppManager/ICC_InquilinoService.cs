using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Inquilinos;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
	public interface ICC_InquilinoService
	{
        long Insert(CC_InquilinoNovo entity);
        void Update(CC_InquilinoAltera entity);
        void Delete(CC_InquilinoVM entity);
		int GetFirstId();
		CC_InquilinoVM Query_ById(int id);
        IEnumerable<CC_InquilinoVM> Query(string where = null);
        bool TableHasData();
        Task<IEnumerable<CC_InquilinoVM>> GetTenantCheckingAcount();
    }
}