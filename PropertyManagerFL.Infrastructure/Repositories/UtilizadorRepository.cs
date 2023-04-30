using CommonLayer.Factories;
using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.SqLiteGenerics;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Users;
using PropertyManagerFL.Infrastructure.Context;
using System.Data;
using System.Text;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Infrastructure.Repositories
{
	public class UtilizadorRepository : IBaseRepository<User_Info>, IUtilizadorRepository
	{
		private readonly DapperContext _context;
		private readonly ILogger<UtilizadorRepository> _logger;

		public UtilizadorRepository(DapperContext context, ILogger<UtilizadorRepository> logger)
		{
			_context = context;
			_logger = logger;
		}
		public bool CheckUserExist(string userName)
		{
			var query = "SELECT COUNT(1) FROM User_Info where User_Name=@user_Name AND IsActive=@isActive";

			using (var connection = _context.CreateConnection())
			{
				bool exists = connection.ExecuteScalar<bool>(query, new { user_Name = userName, isActive = 1 });
				return exists;
			}
		}

		public IDataReader GetData()
		{
			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append("SELECT * FROM vwUtilizadores ");
			sqlCommand.Append("WHERE U.RoleId = R.RoleId ");
			sqlCommand.Append("ORDER BY U.First_Name");

			using (var connection = _context.CreateConnection())
			{
				IDataReader dr = connection.ExecuteReader(sqlCommand.ToString());
				return dr;
			}
		}

		public UserVM? GetData_ID(int Id)
		{
			using (var connection = _context.CreateConnection())
			{
				string sql = "SELECT * FROM vwUtilizadores WHERE Id = @Id";

				var result = connection.Query<UserVM>(sql, new { Id }).FirstOrDefault();
				return result;
			}
		}

		public List<UserVM> GetData_User(string sUserName)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT * FROM vwUtilizadores ");
			sb.Append($"WHERE U.RoleId = R.RoleId AND ");
			sb.Append("U.User_Name LIKE '%{sUserName}%'");

			using (var connection = _context.CreateConnection())
			{
				//connection.Open();
				var list = connection.Query<UserVM>(sb.ToString()).ToList();
				return list;
			}
		}

		public string GetLastLastLoginDate(int userID)
		{
			throw new NotImplementedException();
		}

		public DateTime GetLastPasswordChange(int userID)
		{
			throw new NotImplementedException();
		}
		public string GetUserFirstLastName(int userID)
		{
			throw new NotImplementedException();
		}

		public string GetUserId(string userName)
		{
			throw new NotImplementedException();
		}

		public string GetUserName(int userID)
		{
			throw new NotImplementedException();
		}

		public User_Info GetUserProfile(int Id)
		{

			using (var connection = _context.CreateConnection())
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("SELECT * ");
				sb.Append("FROM User_Info ");
				sb.Append("WHERE Id = @Id");

				string query = sb.ToString();
				//connection.Open();
				var result = connection.Query<User_Info>(query, new { Id }).FirstOrDefault();
				return result;
			}
		}

		public bool IsActiveUser(int userID)
		{
			throw new NotImplementedException();
		}

		public bool IsUser_Admin(string sUserName)
		{
			throw new NotImplementedException();
		}

		public bool IsValidPassword(int Id, string password)
		{
			string query = "SELECT Pwd FROM User_Info WHERE Id=@Id";
			using (var connection = _context.CreateConnection())
			{
				string pwd = connection.Query<string>(query, new { Id }).FirstOrDefault();
				if (pwd.Trim().Equals(password))
					return true;
				else
					return false;
			}
		}

		public bool UpdateLastLoginDate(int Id)
		{
			DateTime dLastLogin = DateTime.Now;

			string query = "UPDATE User_Info SET Last_Login_Date=@lastLoginDate WHERE Id=@Id";

			using (var connection = _context.CreateConnection())
			{
				connection.Execute(query, new { lastLoginDate = dLastLogin, Id });
				return true;
			}
		}

		public bool UpdateUserDetails(User_Info User)
		{
			throw new NotImplementedException();
		}

		public bool UpdateUserProfile(User_Info userProfile)
		{
			try
			{
				Update(userProfile);
				return true;
			}
			catch
			{
				throw;
			}
		}

		public bool UpdateUserProfile_Extended(User_Info userProfile)
		{
			try
			{
				Update(userProfile);
				return true;
			}
			catch
			{
				throw;
			}
		}

		public bool UserCanChangeData(string userName)
		{
			throw new NotImplementedException();
		}

		public int UserCount()
		{
			throw new NotImplementedException();
		}


		public RolesDetail GetRoleDetails(int Id)
		{
			string query = "SELECT Descricao from RolesDetail WHERE Id=@Id";

			using (var connection = _context.CreateConnection())
			{
				RolesDetail roleDetails = connection.QueryFirstOrDefault<RolesDetail>(query, new { Id });
				return roleDetails;
			}

		}
		/// <summary>
		/// Returns the UserRole for the specified RoleId
		/// </summary>
		/// <param name="roleId">RoleId</param>
		/// <returns>UserRole</returns>
		public UserRole GetUserRole(int Id)
		{

			UserRole userRole = new UserRole();

			string query = "SELECT Descricao from RolesDetail WHERE Id=@Id";

			using (var connection = _context.CreateConnection())
			{
				RolesDetail roleDetails = connection.QueryFirstOrDefault<RolesDetail>(query, new { Id });

				if (roleDetails != null)
				{
					string role = roleDetails.Descricao;
					switch (role.ToString().ToUpper())
					{
						case "ADMIN":
							userRole = UserRole.Admin;
							break;
						case "USER":
							userRole = UserRole.GeneralUser;
							break;
						default:
							userRole = UserRole.GeneralUser;
							break;
					}
				}

				return userRole;
			}
		}

		public List<User_Info> GetAll()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT * FROM User_Info ");

			using (var connection = _context.CreateConnection())
			{
				//connection.Open();
				List<User_Info> userList = connection.Query<User_Info>(sb.ToString()).ToList();
				return userList;
			}
		}

		public long Insert(User_Info entity)
		{
			throw new NotImplementedException();
		}

		public void Update(User_Info entity)
		{
			throw new NotImplementedException();
		}

		public void Delete(User_Info entity)
		{
			throw new NotImplementedException();
		}

		public void UpdateById(int Id)
		{
			throw new NotImplementedException();
		}

		public void DeleteById(int Id)
		{
			throw new NotImplementedException();
		}

		public bool EntradaExiste_BD(string campo, string str)
		{
			throw new NotImplementedException();
		}

		public int GetFirstId()
		{
			throw new NotImplementedException();
		}

		public int GetLastId()
		{
			throw new NotImplementedException();
		}

		public User_Info Query_ById(int Id)
		{
			throw new NotImplementedException();
		}

		public bool RecInUse(int Id)
		{
			throw new NotImplementedException();
		}

		public bool TableHasData()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<User_Info> Query(string where = "")
		{
			throw new NotImplementedException();
		}
	}
}
