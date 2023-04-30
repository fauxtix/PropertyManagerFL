using FluentValidation.Results;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Validator;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Shared.ViewModels.Inquilinos;
using PropertyManagerFL.Core.Shared.ViewModels.Recebimentos;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.AppManagerServices
{
	public class RecebimentoService : IRecebimentoService
	{
		private readonly IRecebimentoRepository _repoRecebimentos;
		private readonly ITipoRecebimentoRepository _repoTipoRecebimento;
		private readonly ICC_InquilinoRepository _repoCC_Inquilino;
		private readonly IArrendamentoRepository _repoArrendamento;
		private readonly IInquilinoRepository _repoInquilino;

		private Arrendamento? arrendamento;

		public RecebimentoService(IRecebimentoRepository repoRecebimento,
			ITipoRecebimentoRepository repoTipoRecebimento, ICC_InquilinoRepository repoCC_Inquilino,
			IArrendamentoRepository repoArrendamento, IInquilinoRepository repoInquilino)
		{
			_repoRecebimentos = repoRecebimento;
			_repoTipoRecebimento = repoTipoRecebimento;
			_repoCC_Inquilino = repoCC_Inquilino;
			_repoArrendamento = repoArrendamento;
			_repoInquilino = repoInquilino;
		}

		// Dados para preenchimento da datagrid (dados vêm da view vwRecebimentos na base de dados)
		public List<RecebimentoVM> GetResumedData()
		{
			return _repoRecebimentos.GetResumedData();
		}

		public void Delete(Recebimento entity)
		{
			_repoRecebimentos.Delete(entity);
		}

		/// <summary>
		/// Devolve numero do primeiro Id de uma tabela
		/// Verifica primeiro se tabela tem dados (método TableHasData)
		/// </summary>
		/// <returns>long</returns>
		public int GetFirstId()
		{
			return _repoRecebimentos.GetFirstId();
		}

		public long Insert(Recebimento entity)
		{
			return _repoRecebimentos.Insert(entity);
		}

		public IEnumerable<Recebimento> Query(string where = "")
		{
			return _repoRecebimentos.Query(where);
		}

		public async Task<string> GetNomeInquilino(int Id)
		{
			return await _repoInquilino.GetNomeInquilino(Id);
		}

		public Recebimento Query_ById(int Id)
		{
			return _repoRecebimentos.Query_ById(Id);
		}

		public bool TableHasData()
		{
			return _repoRecebimentos.TableHasData();
		}

		public void Update(Recebimento entity)
		{
			_repoRecebimentos.Update(entity);
		}

		public decimal TotalRecebimentos(int idTipoRecebimentoFiltrado)
		{
			return _repoRecebimentos.TotalRecebimentos(idTipoRecebimentoFiltrado);
		}

		public string RegistoComErros(Recebimento recebimento)
		{
			RecebimentoValidator validator = new RecebimentoValidator();
			ValidationResult results = validator.Validate(recebimento);

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

		public void CriaMovimento_CC_Inquilino(int IdPropriedade, decimal decValorRecebido, DateTime dtMovimento)
		{
			arrendamento = _repoArrendamento.Query($"ID_Fracao = {IdPropriedade}").SingleOrDefault();

			// conta-corrente 
			CC_Inquilino cc_I = new CC_Inquilino()
			{
				ValorPago = decValorRecebido,
				ValorEmDivida = arrendamento.Valor_Renda - decValorRecebido,
				IdInquilino = arrendamento.ID_Inquilino,
				DataMovimento = dtMovimento
			};

			_repoCC_Inquilino.Insert(cc_I);
		}

		public bool RegistoArrendamentoCriado(int IdPropriedade)
		{
			bool bArrendamentoCriado = false;
			arrendamento = _repoArrendamento.Query($"ID_Fracao = {IdPropriedade}").SingleOrDefault();
			if (arrendamento != null)
			{
				bArrendamentoCriado = true;
			}
			return bArrendamentoCriado;
		}

		public int GetID_TipoRecebimento_ByDescription(string Descricao_TipoRec)
		{
			return _repoTipoRecebimento.GetID_ByDescription(Descricao_TipoRec);
		}

		public IEnumerable<TipoRecebimento> GetLista_TipoRecebimento()
		{
			// TODO - linq query não filtra pagamento de rendas, ver se resultado se adapta no código cliente que chama este método

			List<TipoRecebimento> output = _repoTipoRecebimento.Query().OrderBy(p => p.Descricao)
				//.Where(r => r.Descricao != "Pagamento de renda")
				.ToList();
			return output;
		}

		public decimal GetValorRenda(int IdFracao)
		{
			arrendamento = _repoArrendamento.Query($"ID_Fracao = {IdFracao}").FirstOrDefault();
			return arrendamento.Valor_Renda;
		}

		public DateTime Get_Data_Prox_Pagamento(int IdFracao)
		{
			var pagamento = _repoRecebimentos.Query_ById(IdFracao);
			if (pagamento == null)  // fração tem pagamentos?
			{
				return DateTime.MinValue; // não, devolde data inválida e testa depois de chamar este método
			}

			DateTime dtProxMov = _repoRecebimentos.Query()
				.Where(p => p.ID_Propriedade == IdFracao)
				.OrderByDescending(p => p.DataMovimento)
				.Select(r => r.DataMovimento)
				.First().AddMonths(1);

			return dtProxMov;
		}

		public async Task AtualizaSaldoInquilino(int IdFracao, decimal decValorRecebido)
		{
			// Ver quem é o inquilino desta fracao no contrato de arrendamento
			int IdInquilino = _repoArrendamento.Query().Where(p => p.ID_Fracao == IdFracao)
				.Select(r => r.ID_Inquilino).FirstOrDefault();

			// TODO criar tabela de mes/ano para saber se inquilino tem rendas em falta?
			// TODO Para verificar se há montantes em dívida, somar total de pag.tos do inquilino e comparar com saldo corrente

			Inquilino inquilino = await _repoInquilino.GetInquilino_ById(IdInquilino);

			var novoSaldoCorrente = inquilino.SaldoCorrente + decValorRecebido;
			await _repoInquilino.AtualizaSaldo(IdInquilino, novoSaldoCorrente);
		}

		public List<Arrendamento> GetPendingContracts()
		{
			throw new NotImplementedException();
		}

		//public List<Arrendamento> GetPendingContracts()
		//{
		//    IArrendamentoRepository repo_Arrendamentos = new ArrendamentoRepository();
		//    List<Arrendamento> lstArrendamentos = repo_Arrendamentos.Query()
		//        .Where(p => p.Estado_Pagamento.Equals("Pendente")).ToList();

		//    foreach (var contrato in lstArrendamentos)
		//    {
		//        contrato.

		//    }

		//}

		public async Task<int> GetInquilinoFracao(int IdFracao)
		{
			return await _repoInquilino.GetInquilinoFracao(IdFracao);
		}

		public bool RenovacaoAutomatica(int Id)
		{
			return _repoArrendamento.RenovacaoAutomatica(Id);
		}
	}
}
