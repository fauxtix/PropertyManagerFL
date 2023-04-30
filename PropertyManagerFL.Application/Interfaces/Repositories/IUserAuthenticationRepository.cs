using PropertyManagerFL.Application.Shared.Enums;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface IUserAuthenticationRepository
    {
        bool IsValidUser(string userName, string password, out int userId, out AppDefinitions.UserRole role);
    }
}