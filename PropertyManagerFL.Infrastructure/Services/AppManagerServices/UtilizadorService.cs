using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Shared.ViewModels.Users;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
	public class UtilizadorService : IUtilizadorService
	{
		private readonly IUtilizadorRepository _repoUser;
		public UtilizadorService(IUtilizadorRepository repoUser)
		{
			_repoUser = repoUser;
		}

		public User_Info Query_ById(int Id)
		{
			return _repoUser.Query_ById(Id);
		}

		public int GetFistId()
		{
			return _repoUser.GetFirstId();
		}


		public void Delete(User_Info entity)
		{
			_repoUser.Delete(entity);
		}

		public long Insert(User_Info entity)
		{
			return _repoUser.Insert(entity);
		}

		public IEnumerable<User_Info> Query(string where = "")
		{
			return _repoUser.Query(where);
		}

		public bool TableHasData()
		{
			return _repoUser.TableHasData();
		}

		public void Update(User_Info entity)
		{
			_repoUser.Update(entity);
		}

		public string RegistoComErros(UserWithConfirmPwd user)
		{
			UtilizadorValidator validator = new UtilizadorValidator();
			ValidationResult results = validator.Validate(user);

			if (!results.IsValid)
			{
				StringBuilder sb = new StringBuilder();
				foreach (var failure in results.Errors)
				{
					sb.AppendLine(failure.ErrorMessage);
				}
				return sb.ToString();
			}

			return "";
		}

		public UserVM GetData_ID(int Id)
		{
			return _repoUser.GetData_ID(Id);
		}

		public List<User_Info> GetAll()
		{
			return _repoUser.GetAll();
		}

		public bool UpdateLastLoginDate(int userId)
		{
			return _repoUser.UpdateLastLoginDate(userId);
		}
	}
}
