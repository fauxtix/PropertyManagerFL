using PropertyManagerFL.Application.Shared.Enums;
using PropertyManagerFL.Application.ViewModels.Users;
using PropertyManagerFL.Core.Entities;
using System.Data;

namespace PropertyManagerFL.Application.Interfaces.Services.User
{
    public interface IUserService
    {
        bool CheckUserExist(string userName);
        List<User_Info> DecryptUsers(IEnumerable<User_Info> users);
        List<User_Info> GetAllUsers();
        UserVM GetData_ID(int Id);
        List<UserVM> GetData_User(string sUserName);
        RolesDetail GetRoleDetails(int Id);
        User_Info GetUserProfile(int Id);
        AppDefinitions.UserRole GetUserRole(int Id);
        IDataReader GetUsersData(User_Info UserData);
        bool IsValidPassword(int Id, string password);
        bool UpdateLastLoginDate(int Id);
        bool UpdateUserProfile(int userID, string userName, string firstName, string email, string mobile, int Ativo, int roleId);
        bool UpdateUserProfile(int userID, string userName, string firstName, string password, string email, string mobile, int Ativo, int roleId);

        Task<string> GetRoleIdByName(string roleName);
        Task<string> GetUserRoleId(string sUserID);
        Task<string> GetUserRoleName(string sUserID);
        Task<string> GetUserRoleName_ByName(string roleName);

    }
}