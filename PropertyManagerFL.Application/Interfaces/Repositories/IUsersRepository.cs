using PropertyManagerFL.Application.ViewModels.Users;
using System.Threading.Tasks;

namespace MediaOrganizerApp.Application.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task<string> GetUserRoleId(string sUserID);
        Task<string> GetUserRoleName(string sUserID);
        Task<string> GetUserRoleName_ByName(string sRoleName);
        Task<string> GetRoleIdByName(string roleName);
        Task<Users> GetUser(string username);
        Task<Users> AddUser(string username, string password, string role);
        Task<Users> GetUserRoleById(int userId);
    }
}