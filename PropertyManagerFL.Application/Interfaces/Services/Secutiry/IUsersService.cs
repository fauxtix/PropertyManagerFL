namespace PropertyManagerFL.Application.Interfaces.Services.Security
{
    public interface IUsersService
    {
        Task<string> GetRoleIdByName(string roleName);
        Task<string> GetUserRoleId(string sUserID);
        Task<string> GetUserRoleName(string sUserID);
        Task<string> GetUserRoleName_ByName(string roleName);
        string GetUserRoleName_ByEmail(string userEmail);
    }
}
