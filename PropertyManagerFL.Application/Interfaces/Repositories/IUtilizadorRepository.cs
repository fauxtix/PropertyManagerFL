using PropertyManagerFL.Application.Interfaces.SqLiteGenerics;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Users;
using System.Data;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
	public interface IUtilizadorRepository : IBaseRepository<User_Info>
	{
		bool CheckUserExist(string userName);
		IDataReader GetData();  // User_Info UserData
		UserVM GetData_ID(int ID);
		List<UserVM> GetData_User(string sUserName);
		string GetLastLastLoginDate(int userID);
		DateTime GetLastPasswordChange(int userID);
		string GetUserFirstLastName(int userID);
		string GetUserId(string userName);
		string GetUserName(int userID);
		User_Info GetUserProfile(int userID);
		bool IsActiveUser(int userID);
		bool IsUser_Admin(string sUserName);
		bool IsValidPassword(int userID, string password);
		bool UpdateLastLoginDate(int userID);
		bool UpdateUserDetails(User_Info User);

		bool UpdateUserProfile(User_Info userProfile);

		bool UpdateUserProfile_Extended(User_Info userProfile);

		bool UserCanChangeData(string userName);
		int UserCount();
		UserRole GetUserRole(int roleId);
		RolesDetail GetRoleDetails(int Id);
		List<User_Info> GetAll();
	}
}