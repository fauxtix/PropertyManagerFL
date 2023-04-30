using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Users;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
	public interface IUtilizadorService
	{
		void Delete(User_Info entity);
		List<User_Info> GetAll();
		UserVM GetData_ID(int Id);
		int GetFistId();
		long Insert(User_Info entity);
		IEnumerable<User_Info> Query(string where = null);
		User_Info Query_ById(int Id);
		string RegistoComErros(UserWithConfirmPwd user);
		bool TableHasData();
		void Update(User_Info entity);
		bool UpdateLastLoginDate(int userId);
	}
}