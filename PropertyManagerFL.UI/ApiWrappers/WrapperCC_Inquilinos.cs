using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Inquilinos;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperCC_Inquilinos : ICC_InquilinoService
    {
        public long Insert(CC_InquilinoNovo entity)
        {
            throw new NotImplementedException();
        }
        public void Update(CC_InquilinoAltera entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(CC_InquilinoVM entity)
        {
            throw new NotImplementedException();
        }

        public int GetFirstId()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<CC_InquilinoVM> Query(string where = null)
        {
            throw new NotImplementedException();
        }

        public CC_InquilinoVM Query_ById(int id)
        {
            throw new NotImplementedException();
        }

        public bool TableHasData()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CC_InquilinoVM>> GetTenantCheckingAcount()
        {
            throw new NotImplementedException();
        }
    }
}
