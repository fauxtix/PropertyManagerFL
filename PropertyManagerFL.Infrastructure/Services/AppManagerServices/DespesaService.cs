using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Shared.ViewModels.Despesas;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
	public class DespesaService : IDespesaService
	{
		private readonly IDespesaRepository _repoDespesas;
		private readonly IArrendamentoRepository _repoArrendamento;
		private readonly IFracaoRepository _repoFracao;

		public DespesaService(IDespesaRepository repoDespesas,
			IArrendamentoRepository repoArrendamento,
			IFracaoRepository repoFracao)
		{
			_repoDespesas = repoDespesas;
			_repoArrendamento = repoArrendamento;
			_repoFracao = repoFracao;
		}

		// Dados para preenchimento da datagrid (dados vêm da view vwDespesas na base de dados)
		public List<DespesaVM> GetResumedData()
		{
			return _repoDespesas.GetResumedData();
		}

		public void Delete(Despesa entity)
		{
			_repoDespesas.Delete(entity);
		}

		/// <summary>
		/// Devolve numro do primeiro Id de uma tabela
		/// Verifica primeiro se tabela tem dados (método TableHasData)
		/// </summary>
		/// <returns>long</returns>
		public int GetFirstId()
		{
			return _repoDespesas.GetFirstId();
		}

		public long Insert(Despesa entity)
		{
			return _repoDespesas.Insert(entity);
		}

		public IEnumerable<Despesa> Query(string where = "")
		{
			return _repoDespesas.Query(where);
		}

		public async Task<string> GetNomeInquilino(int Id)
		{
			return await _repoArrendamento.GetNomeInquilino(Id);
		}

		public Despesa Query_ById(int id)
		{
			return _repoDespesas.Query_ById(id);
		}

		public bool TableHasData()
		{
			return _repoDespesas.TableHasData();
		}

		public void Update(Despesa entity)
		{
			_repoDespesas.Update(entity);
		}

		public async Task<string> GetNomeFracao(int IdInquilino)
		{
			Despesa? despesa = _repoDespesas.Query($"ID_Inquilino = {IdInquilino}").FirstOrDefault();

			if (despesa == null) // Inquilino sem fração atribuída...
				return "";

			return await _repoFracao.GetNomeFracao(despesa.ID_Fracao);
		}

		public string RegistoComErros(Despesa despesa)
		{
			DespesaValidator validator = new DespesaValidator();
			ValidationResult results = validator.Validate(despesa);

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

		public decimal TotalDespesas(int IdTipoDespesa)
		{
			return _repoDespesas.TotalDespesas(IdTipoDespesa);
		}

		public int GetLastId()
		{
			return _repoDespesas.GetLastId();
		}

		public List<DespesaVM> Query_ByYear(string sAno)
		{
			return _repoDespesas.Query_ByYear(sAno);
		}
	}
}
