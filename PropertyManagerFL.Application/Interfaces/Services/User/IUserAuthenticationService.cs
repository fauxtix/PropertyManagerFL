using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Application.Interfaces.Services.User
{
    public interface IUserAuthenticationService
    {
        bool IsValidUser(string userName, string password, out int userId, UserRole role);
    }
}