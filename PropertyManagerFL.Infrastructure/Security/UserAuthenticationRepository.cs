using CommonLayer.Factories;
using Dapper;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Security;
using PropertyManagerFL.Application.Shared.Enums;
using PropertyManagerFL.Application.ViewModels.Users;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Security
{
    public class UserAuthenticationRepository : IUserAuthenticationRepository
    {
        private readonly IDataSecurityRepository _securityProvider;
        private readonly IUtilizadorRepository _repo_Users;

        public UserAuthenticationRepository(IDataSecurityRepository securityProvider, IUtilizadorRepository repo_Users)
        {
            _securityProvider = securityProvider;
            _repo_Users = repo_Users;
        }

        /// <summary>
        /// Autenticação
        /// </summary>
        /// <param name="userName">Utilizador</param>
        /// <param name="password">Password</param>
        /// <param name="userId">Um parâmetro 'out' que devolve o UserId se o login foi bem sucedido</param>
        /// <param name="role">Um parâmetro 'out' que devolve o RoleId se o login foi bem sucedido</param>
        /// <returns>true - se o Utilizador e Password forem válidos, senão false</returns>
        public bool IsValidUser(string userName, string password,
            out int userId, out AppDefinitions.UserRole role)
        {
            using (var connection = ConnectionManager.GetConnection())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("SELECT * FROM vwUtilizadores ");
                sb.Append("WHERE User_Name=@userName AND Pwd=@password AND IsActive=1");
                string query = sb.ToString();

                var result = connection.Query<UserVM>
                    (query, new { userName, password }).FirstOrDefault();

                if (result == null)
                {
                    userId = 0;
                    role = AppDefinitions.UserRole.GeneralUser;
                    return false;
                }
                else
                {
                    userId = result.Id;
                    role = _repo_Users.GetUserRole(result.RoleId);
                    return true;
                }
            }
        }
    }
}
