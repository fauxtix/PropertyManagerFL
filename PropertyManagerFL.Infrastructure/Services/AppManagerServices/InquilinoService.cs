using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Shared.ViewModels.Inquilinos;
using PropertyManagerFL.Core.Shared.ViewModels.LookupTables;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
	public class InquilinoService : IInquilinoService
	{
		private readonly IInquilinoRepository repo;
		public InquilinoService(IInquilinoRepository repoInquilino)
		{
			repo = repoInquilino;
		}

		//// Dados para preenchimento da datagrid (dados vêm da view vwFracoes na base de dados)
		//public List<Inquilinos> GetResumedData()
		//{
		//    return repo.GetResumedData();
		//}


		public int GetFirstId_Inquilino()
		{
			return repo.GetFirstId_Inquilino();
		}

		public string RegistoComErros(Inquilino inquilino)
		{
			InquilinoValidator validator = new InquilinoValidator();
			ValidationResult results = validator.Validate(inquilino);

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

		public async Task<string> GetNomeInquilino(int Id)
		{
			return await repo.GetNomeInquilino(Id);
		}

		public void AtualizaSaldo(int IdInquilino, decimal decSaldoCorrente)
		{
			repo.AtualizaSaldo(IdInquilino, decSaldoCorrente);
		}

		public string UltimoMesPago(int IdInquilino)
		{
			return repo.GetUltimoMesPago_Inquilino(IdInquilino);
		}

		public async Task<IEnumerable<LookupTableVM>> GetInquilinosDisponiveis(bool titular)
		{
			return await repo.GetInquilinosDisponiveis(titular);
		}

		public async Task<int> GetInquilinoFracao(int ID_Fracao)
		{
			return await repo.GetInquilinoFracao(ID_Fracao);
		}

		public string GetNomeFracao(int IdInquilino, bool bTitular)
		{
			return repo.GetNomeFracao(IdInquilino, bTitular);
		}

		public string GetUltimoMesPago_Inquilino(int ID_Inquilino)
		{
			return repo.GetUltimoMesPago_Inquilino(ID_Inquilino);
		}

		public async Task<IEnumerable<LookupTableVM>> GetInquilinos_Fiadores(bool Titular)
		{
			return await repo.GetInquilinos_Fiadores(Titular);
		}

		public async Task<IEnumerable<InquilinoVM>> GetAll()
		{
			return await repo.GetAll();
		}

		public int GetFirstIdInquilino()
		{
			return repo.GetFirstIdInquilino();
		}

		public IEnumerable<Inquilino> Query(string where = "")
		{
			throw new NotImplementedException();
		}

		public Inquilino Query_ById(int id)
		{
			throw new NotImplementedException();
		}

		public bool TableHasData()
		{
			throw new NotImplementedException();
		}
	}
}

