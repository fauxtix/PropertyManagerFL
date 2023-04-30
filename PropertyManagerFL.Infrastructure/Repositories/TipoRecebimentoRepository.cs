using CommonLayer.Factories;
using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Infrastructure.Context;
using PropertyManagerFL.Infrastructure.SqLiteGenerics;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories
{
	public class TipoRecebimentoRepository : BaseRepository<TipoRecebimento>, ITipoRecebimentoRepository
	{
		private readonly DapperContext _context;
		private readonly ILogger<FracaoRepository> _logger;

		public TipoRecebimentoRepository(DapperContext context, ILogger<FracaoRepository> logger)
		{
			_context = context;
			_logger = logger;
		}
		public int GetID_ByDescription(string Descricao)
		{
			var query = "SELECT Id FROM TipoRecebimento WHERE Descricao = @Descricao";

			using (var connection = _context.CreateConnection())
			{
				int output = connection.ExecuteScalar<int>(query, new { Descricao });
				return output;
			}
		}

		public IEnumerable<LookupTableVM> GetOutroTipoRecebimento()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("SELECT Id, Descricao FROM TipoRecebimento ");
			sb.Append("WHERE Descricao != 'Pagamento de renda'");

			using (var connection = _context.CreateConnection())
			{
				var list = connection.Query<LookupTableVM>(sb.ToString()).ToList();
				return list;
			}
		}
	}

}
