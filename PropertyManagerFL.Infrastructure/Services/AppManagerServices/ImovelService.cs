using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
	public class ImovelService : IImovelService
	{
		private readonly IImovelRepository repo;
		public ImovelService(IImovelRepository repoImovel)
		{
			repo = repoImovel;
		}


		public async Task<string> GetNumeroPorta(int Id)
		{
			return await repo.GetNumeroPorta(Id);
		}


		public string RegistoComErros(Imovel imovel)
		{
			ImovelValidator validator = new ImovelValidator();
			ValidationResult results = validator.Validate(imovel);

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

		public async Task<int> GetCodigo_Imovel(int Id)
		{
			return await repo.GetCodigo_Imovel(Id);
		}

		public async Task<string> GetDescricao_Imovel(int Id)
		{
			return await repo.GetDescricao_Imovel(Id);
		}

		public async Task<Imovel> InsereImovel(Imovel novoImovel)
		{
			return await repo.InsereImovel(novoImovel);
		}

		public async Task<Imovel> AtualizaImovel(Imovel alteraImovel)
		{
			return await repo.AtualizaImovel(alteraImovel);
		}

		public async Task ApagaImovel(int id)
		{
			await repo.ApagaImovel(id);
		}

		public async Task<Imovel> GetImovel_ById(int id)
		{
			return await repo.GetImovel_ById(id);
		}

		public async Task<IEnumerable<Imovel>> GetAll()
		{
			return await repo.GetAll();
		}
	}
}
