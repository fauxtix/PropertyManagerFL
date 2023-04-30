using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.User;
using PropertyManagerFL.Application.Security;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Users;
using System.Data;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Infrastructure.Services.UserServices
{
	public class UserService : IUserService
	{
		readonly IDataSecurityRepository _securityProvider;
		readonly IUtilizadorRepository _repoUser;

		public UserService(IDataSecurityRepository securityProvider, IUtilizadorRepository repoUser)
		{
			_securityProvider = securityProvider;
			_repoUser = repoUser;
		}

		public List<User_Info> DecryptUsers(IEnumerable<User_Info> users)
		{
			List<User_Info> output = new List<User_Info>();
			foreach (var user in users)
			{
				user.User_Name = _securityProvider.Decrypt(user.User_Name);
				user.EMail = !string.IsNullOrEmpty(user.EMail) ? _securityProvider.Decrypt(user.EMail) : "";
				user.Mobile = !string.IsNullOrEmpty(user.Mobile) ? _securityProvider.Decrypt(user.Mobile) : "";
				output.Add(user);
			}

			return output;
		}

		public List<User_Info> GetAllUsers()
		{
			return _repoUser.GetAll();
		}

		public RolesDetail GetRoleDetails(int Id)
		{
			return _repoUser.GetRoleDetails(Id);
		}

		public UserRole GetUserRole(int Id)
		{
			UserRole userRole = new UserRole();
			RolesDetail roleDetails = GetRoleDetails(Id);
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

		public bool CheckUserExist(string userName)
		{
			string sUserName = _securityProvider.Encrypt(userName);
			return _repoUser.CheckUserExist(sUserName);
		}

		public User_Info GetUserProfile(int Id)
		{
			return _repoUser.GetUserProfile(Id);
		}

		public IDataReader GetUsersData(User_Info UserData)
		{
			return _repoUser.GetData();
			//if (UserData != null)
			//{
			//    if (UserData.User_Name != string.Empty)
			//        sqlCommand.Append(" AND U.User_Name = '" + _securityProvider.Encrypt(UserData.User_Name) + "'");

			//    if (UserData.First_Name != string.Empty)
			//        sqlCommand.Append("AND U.First_Name LIKE '%" + UserData.First_Name + "%' ");

			//    if (UserData.RoleId != -1)
			//        sqlCommand.Append("AND U.RoleId = " + UserData.RoleId.ToString());

			//    if (UserData.EMail != string.Empty)
			//        sqlCommand.Append("AND U.Email = '" + _securityProvider.Encrypt(UserData.EMail) + "'");

			//    if (UserData.Mobile != string.Empty)
			//        sqlCommand.Append("AND U.Mobile = '" + _securityProvider.Encrypt(UserData.Mobile) + "'");
			//}


			//sqlCommand.Append(" ORDER BY U.First_Name");
		}

		public UserVM GetData_ID(int Id)
		{
			return _repoUser.GetData_ID(Id);
		}

		public List<UserVM> GetData_User(string sUserName)
		{
			return _repoUser.GetData_User(sUserName);
		}

		public bool IsValidPassword(int Id, string password)
		{
			string sPassword = _securityProvider.Encrypt(password);
			return _repoUser.IsValidPassword(Id, sPassword);
		}

		public bool UpdateLastLoginDate(int Id)
		{
			return _repoUser.UpdateLastLoginDate(Id);
		}

		public bool UpdateUserProfile(int userID, string userName, string firstName,
			string email, string mobile, int Ativo, int roleId)
		{
			try
			{
				User_Info userProfile = new User_Info
				{
					Id = userID,
					User_Name = _securityProvider.Encrypt(userName),
					First_Name = firstName,
					EMail = _securityProvider.Encrypt(email),
					Mobile = _securityProvider.Encrypt(mobile),
					IsActive = Ativo,
					RoleId = roleId,
					Password_Change_Date = DateTime.UtcNow
				};

				_repoUser.Update(userProfile);
				return true;
			}
			catch
			{
				throw;
			}
		}

		public bool UpdateUserProfile(int userID, string userName, string firstName,
			string password, string email, string mobile, int Ativo, int roleId)
		{
			try
			{
				User_Info userProfile = new User_Info
				{
					Id = userID,
					User_Name = _securityProvider.Encrypt(userName),
					First_Name = firstName,
					Pwd = _securityProvider.Encrypt(password),
					EMail = _securityProvider.Encrypt(email),
					Mobile = _securityProvider.Encrypt(mobile),
					IsActive = Ativo,
					RoleId = roleId,
					Password_Change_Date = DateTime.UtcNow
				};

				_repoUser.UpdateUserProfile_Extended(userProfile);
				return true;
			}
			catch
			{
				throw;
			}
		}

        public Task<string> GetRoleIdByName(string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserRoleId(string sUserID)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserRoleName(string sUserID)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserRoleName_ByName(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
