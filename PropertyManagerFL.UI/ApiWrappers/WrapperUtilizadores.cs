using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Users;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperUtilizadores : IUtilizadorService
    {
        public void Delete(User_Info entity)
        {
            throw new NotImplementedException();
        }

        public List<User_Info> GetAll()
        {
            throw new NotImplementedException();
        }

        public UserVM GetData_ID(int Id)
        {
            throw new NotImplementedException();
        }

        public int GetFistId()
        {
            throw new NotImplementedException();
        }

        public long Insert(User_Info entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User_Info> Query(string where = null)
        {
            throw new NotImplementedException();
        }

        public User_Info Query_ById(int Id)
        {
            throw new NotImplementedException();
        }

        public string RegistoComErros(UserWithConfirmPwd user)
        {
            throw new NotImplementedException();
        }

        public bool TableHasData()
        {
            throw new NotImplementedException();
        }

        public void Update(User_Info entity)
        {
            throw new NotImplementedException();
        }

        public bool UpdateLastLoginDate(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
