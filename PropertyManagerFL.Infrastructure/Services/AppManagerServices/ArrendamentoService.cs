using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Shared.ViewModels.Arrendamentos;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
	public class ArrendamentoService : IArrendamentoService
	{
		readonly IArrendamentoRepository _repoArrendamento;
		readonly IInquilinoRepository _repoInquilinos;

		public ArrendamentoService(IArrendamentoRepository repoArrendamento, IInquilinoRepository repoInquilinos)
		{
			_repoArrendamento = repoArrendamento;
			_repoInquilinos = repoInquilinos;
		}

		// 07/11

		// INSERT INTO HistoricoAtualizacaoRendas(DataAtualizacao, ID_Atualizacao, ID_Fracao, Valor_Renda)
		//select A.Data_Inicio, 1, F.id AS Fra_ID, A.valor_renda
		//from Arrendamentos A INNER JOIN Fracoes F
		//ON A.ID_Fracao = F.Id;

		// Dados para preenchimento da datagrid (dados vêm da view vwArrendamentos na base de dados)
		public List<ArrendamentoVM> GetResumedData()
		{
			return _repoArrendamento.GetResumedData();
		}

		public void Delete(int id)
		{
			_repoArrendamento.DeleteById_Async(id);
		}

		public bool ChildrenExists(int IdFracao)
		{
			return _repoArrendamento.ChildrenExists(IdFracao);
		}

		public int GetFirstId()
		{
			return _repoArrendamento.GetFirstId();
		}

		public long Insert(Arrendamento entity)
		{
			return _repoArrendamento.Insert(entity);
		}

		public IEnumerable<Arrendamento> Query(string where = "")
		{
			return _repoArrendamento.Query(where);
		}

		public async Task<string> GetNomeInquilino(int Id)
		{
			return await _repoArrendamento.GetNomeInquilino(Id);
		}

		public string GetDocumentoArrendamento(int Id)
		{
			return _repoArrendamento.GetDocumentoGerado(Id);
		}

		public Arrendamento Query_ById(int id)
		{
			return _repoArrendamento.Query_ById(id);
		}

		public bool TableHasData()
		{
			return _repoArrendamento.TableHasData();
		}

		public void Update(Arrendamento entity)
		{
			_repoArrendamento.Update(entity);
		}

		public bool ContratoEmitido(int Id)
		{
			return _repoArrendamento.ContratoEmitido(Id);
		}

		public void MarcaContratoComoEmitido(int Id, string docGerado)
		{
			_repoArrendamento.MarcaContratoComoEmitido(Id, docGerado);
		}

		public void MarcaContratoComoNaoEmitido(int Id)
		{
			_repoArrendamento.MarcaContratoComoNaoEmitido(Id);
		}

		public string GetNomeFracao(int IdInquilino, bool bInquilino)
		{
			return _repoInquilinos.GetNomeFracao(IdInquilino, bInquilino);
		}

		public decimal TotalRendas()
		{
			return _repoArrendamento.TotalRendas();
		}

		public string RegistoComErros(Arrendamento inquilino)
		{
			ArrendamentoValidator validator = new ArrendamentoValidator();
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

		public void GeraMovimentos(Arrendamento arrendamento, int IdFracao)
		{

			_repoArrendamento.GeraMovimentos(arrendamento, IdFracao);
		}

		public bool RenovacaoAutomatica(int Id)
		{
			return _repoArrendamento.RenovacaoAutomatica(Id);
		}

		public string GetDocumentoGerado(int Id)
		{
			return _repoArrendamento.GetDocumentoGerado(Id);
		}

		public void CriaRegistoHistorico(Arrendamento arrendamento)
		{
			_repoArrendamento.CriaRegistoHistorico(arrendamento);
		}

		public void CheckNewRents()
		{
			_repoArrendamento.CheckNewRents();
		}

		public bool ArrendamentoExiste(int IdFracao)
		{
			return _repoArrendamento.ArrendamentoExiste(IdFracao);
		}

	}
}
