using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Shared.ViewModels.Fracoes;
using PropertyManagerFL.Core.Shared.ViewModels.SituacaoFracao;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
	public class FracaoService : IFracaoService
	{
		private readonly IFracaoRepository repo;
		public FracaoService(IFracaoRepository repoFracoes)
		{
			repo = repoFracoes;
		}

		public async Task<Fracao> InsereFracao(Fracao novaFracao)
		{
			return await repo.InsereFracao(novaFracao);
		}
		public async Task<Fracao> AtualizaFracao(Fracao alteraFracao)
		{
			return await repo.AtualizaFracao(alteraFracao);
		}
		public async Task ApagaFracao(int id)
		{
			await repo.ApagaFracao(id);
		}

		// Dados para preenchimento da datagrid (dados vêm da view vwFracoes na base de dados)
		public async Task<List<FracaoVM>> GetResumedData()
		{
			return await repo.GetResumedData();
		}


		/// <summary>
		/// Devolve numro do primeiro Id de uma tabela
		/// Verifica primeiro se tabela tem dados (método TableHasData)
		/// </summary>
		/// <returns>long</returns>
		public int GetFirstId()
		{
			return repo.GetFirstId();
		}

		public async Task<Fracao> GetFracao_ById(int id)
		{
			return await repo.GetFracao_ById(id);
		}

		public async Task<List<SituacaoFracaoVM>> GetSituacaoFracao()
		{
			return await repo.GetSituacaoFracao();
		}

		public async Task<string> GetNomeFracao(int Id)
		{
			return await repo.GetNomeFracao(Id);
		}

		public async Task<int> GetIDSituacao_ByDescription(string sDescricaoSituacao)
		{
			return await repo.GetIDSituacao_ByDescription(sDescricaoSituacao);
		}

		public string RegistoComErros(Fracao fracao)
		{
			FracaoValidator validator = new FracaoValidator();
			ValidationResult results = validator.Validate(fracao);

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

		public async Task<bool> MarcaFracaoComoAlugada(int Id)
		{
			return await repo.MarcaFracaoComoAlugada(Id);
		}

		public async Task<bool> MarcaFracaoComoLivre(int Id)
		{
			return await repo.MarcaFracaoComoLivre(Id);
		}

		public async Task<IEnumerable<Fracao>> GetAll()
		{
			return await repo.GetAll();
		}

		public async Task<IEnumerable<Fracao>> GetFracoes_Imovel(int id = 0)
		{
			return await repo.GetFracoes_Imovel	(id);
		}
	}
}

