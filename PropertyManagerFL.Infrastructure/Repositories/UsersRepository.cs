using Dapper;
using MediaOrganizerApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.ViewModels.Security.Models;
using PropertyManagerFL.Application.ViewModels.Users;
using PropertyManagerFL.Infrastructure.Context;
using PropertyManagerFL.Infrastructure.Services.SecurityServices;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<FracaoRepository> _logger;

        public UsersRepository(DapperContext context, ILogger<FracaoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GetRoleIdByName(string roleName)
        {
            DynamicParameters paramCollection = new();
            paramCollection.Add("@RoleName", roleName);

            StringBuilder sb = new();
            sb.Append("SELECT R.Id RoleId ");
            sb.Append("FROM AspNetRoles R ");
            sb.Append("WHERE R.Name = @RoleName");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QuerySingleAsync<string>
                    (sb.ToString(), paramCollection);
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }


        public async Task<string> GetUserRoleName_ByName(string sRoleName)
        {
            DynamicParameters paramCollection = new();
            paramCollection.Add("@RoleName", sRoleName);

            StringBuilder sb = new();
            sb.Append("SELECT DISTINCT R.Id RoleId ");
            sb.Append("FROM AspNetUsers U ");
            sb.Append("INNER JOIN AspNetUserRoles UR ON U.Id = UR.UserId ");
            sb.Append("INNER JOIN AspNetRoles R ON UR.RoleId = R.Id ");
            sb.Append("WHERE R.Name = @RoleName");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var roleName = await connection.QuerySingleAsync<string>
                    (sb.ToString(), paramCollection);

                    return roleName;
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }

        }

        public async Task<string> GetUserRoleId(string sUserID)
        {
            DynamicParameters paramCollection = new();
            paramCollection.Add("@UserId", sUserID);
            StringBuilder sb = new();
            sb.Append("SELECT a1.Id RoleId ");
            sb.Append("FROM AspNetUsers a ");
            sb.Append("INNER JOIN AspNetUserRoles a0 ON a.Id = a0.UserId ");
            sb.Append("INNER JOIN AspNetRoles a1 ON a0.RoleId = a1.Id ");
            sb.Append("WHERE UserId = @UserId");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    string output = await connection.QuerySingleAsync<string>
                    (sb.ToString(), paramCollection);
                    return output;
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }

        }

        public async Task<string> GetUserRoleName(string sUserID)
        {
            DynamicParameters paramCollection = new();
            paramCollection.Add("@UserId", sUserID);
            StringBuilder sb = new();
            sb.Append("SELECT a1.Name RoleName ");
            sb.Append("FROM AspNetUsers a ");
            sb.Append("INNER JOIN AspNetUserRoles a0 ON a.Id = a0.UserId ");
            sb.Append("INNER JOIN AspNetRoles a1 ON a0.RoleId = a1.Id ");
            sb.Append("WHERE UserId = @UserId");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QuerySingleAsync<string>
                    (sb.ToString(), paramCollection);
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        // Capstone 'try'

        public async Task<Users> GetUser(string username)
        {
            Users? returnUser = null;

            DynamicParameters paramCollection = new();
            paramCollection.Add("@username", username);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT user_id, username, password_hash, salt, user_role ");
            sb.Append("FROM users ");
            sb.Append("WHERE username = @username");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var reader = await connection.ExecuteReaderAsync(sb.ToString(), paramCollection);
                    if (reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }

                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnUser;
        }

        public async Task<Users> AddUser(string username, string password, string role)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            DynamicParameters paramCollection = new();
            paramCollection.Add("@username", username);
            paramCollection.Add("@password_hash", hash.Password);
            paramCollection.Add("@salt", hash.Salt);
            paramCollection.Add("@user_role", role);

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO users (username, password_hash, salt, user_role) ");
            sb.Append("VALUES (@username, @password_hash, @salt, @user_role)");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(sb.ToString(), paramCollection);
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return await GetUser(username);
        }


        public async Task<Users> GetUserRoleById(int userId)
        {
            Users returnUser = new Users();
            DynamicParameters paramCollection = new();
            paramCollection.Add("@user_Id", userId);

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT user_id, user_role, username ");
            sb.Append("FROM users WHERE user_id = @user_Id ");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var reader = await connection.ExecuteReaderAsync(sb.ToString(), param: paramCollection);

                    if (reader.Read())
                    {

                        returnUser.UserId = Convert.ToInt32(reader["user_id"]);
                        returnUser.Username = Convert.ToString(reader["username"]);
                        returnUser.PasswordHash = "XXXX";
                        returnUser.Salt = "XXXX";
                        returnUser.Role = Convert.ToString(reader["user_role"]);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUser;
        }


        private Users GetUserFromReader(IDataReader reader)
        {
            Users u = new Users()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),
                Role = Convert.ToString(reader["user_role"]),
            };

            return u;
        }

        public string GetUserRoleName_ByEmail(string userEmail)
        {
            DynamicParameters paramCollection = new DynamicParameters();
            paramCollection.Add("@UserEmail", userEmail);

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return connection.QueryFirstOrDefault<string>
                        ("usp_GetUserRoleName_ByEmail", paramCollection, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return "";
            }
        }
    }
}

