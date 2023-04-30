using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Infrastructure.Context;

using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories
{
	public class LookupTableRepository : ILookupTableRepository
	{
		private readonly DapperContext _context;
		private readonly ILogger<LookupTableRepository> _logger;

		public LookupTableRepository(DapperContext context, ILogger<LookupTableRepository> logger)
		{
			_context = context;
			_logger = logger;
		}
		public async Task< IEnumerable<LookupTableVM>> GetLookupTableData(string tableName)
		{
			StringBuilder sql = new StringBuilder();
			sql.Append("SELECT Id, Descricao ");
			sql.Append($"FROM {tableName} ");

			using (var connection = _context.CreateConnection())
			{
				var result = await connection.QueryAsync<LookupTableVM>(sql.ToString());
				return result.ToList();
			}
		}

		public string GetDescription(int id, string tableName)
		{
			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Id", id);
			paramCollection.Add("@TableName", tableName);

			StringBuilder sql = new StringBuilder();
			sql.Append("SELECT Descricao ");
			sql.Append("FROM @TableName WHERE Id = @Id ");
			using (var connection = _context.CreateConnection())
			{
				var result = connection.QueryFirstOrDefault<string>(sql.ToString());
				return result;
			}
		}

		public bool CriaNovoRegisto(LookupTableVM tableRecord)
		{
			string descricao = "Descricao";

			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Descricao", tableRecord.Descricao);

			string Query = $"INSERT INTO {tableRecord.Tabela} ({descricao}) VALUES (@Descricao)";
			try
			{
				using (var connection = _context.CreateConnection())
				{
					var output = connection.Execute(Query, param: paramCollection);
					return true;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
		}

		/// <summary>
		/// Atualiza Registo - genérico - tabelas auxiliares 
		/// </summary>
		/// <param name="Codigo"></param>
		/// <param name="Descricao"></param>
		/// <param name="Tabela"></param>
		/// <returns></returns>
		public bool ActualizaDetalhes(LookupTableVM tableRecord)
		{

			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Id", tableRecord.Id);
			paramCollection.Add("@Description", tableRecord.Descricao);

			string Query = $"UPDATE {tableRecord.Tabela} SET Descricao = @Description WHERE Id = @Id";

			try
			{
				using (var connection = _context.CreateConnection())
				{
					int i = connection.Execute(Query, paramCollection);
					return true;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
		}


		/// <summary>
		/// Check Registo Exist - genérico - tabelas auxiliares 
		/// </summary>
		/// <param name="Descricao"></param>
		/// <param name="Tabela"></param>
		/// <returns></returns>
		public bool CheckRegistoExist(string Descricao, string Tabela)
		{
			string Query = string.Empty;
			int iCnt = 0;
			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Descricao", Descricao);

			string descricao = "Descricao";

			Query = $"SELECT COUNT(*) FROM {Tabela} WHERE {descricao} = @Descricao";
			try
			{
				using (var connection = _context.CreateConnection())
				{
					iCnt = Convert.ToInt32(connection.QueryFirstOrDefault<bool>(Query, paramCollection));
					return (iCnt > 0);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
		}

		/// <summary>
		/// Check record Exist in child tables => genérico - tabelas auxiliares 
		/// </summary>
		/// <param name="Descricao"></param>
		/// <param name="Tabela"></param>
		/// <returns></returns>
		public bool CheckFKInUse(int sourceFk, string fieldToCheck, string tableToCheck)
		{
			string Query = string.Empty;
			int iCnt = 0;
			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@IdFk", sourceFk);

			Query = $"SELECT COUNT(*) FROM {tableToCheck} WHERE {fieldToCheck} = @IdFk";
			try
			{
				using (var connection = _context.CreateConnection())
				{
					iCnt = Convert.ToInt32(connection.QueryFirstOrDefault<int>(Query, paramCollection));
					return (iCnt > 0);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
		}

		/// <summary>
		/// Delete record => genérico - tabelas auxiliares
		/// </summary>
		/// <param name="Codigo"></param>
		/// <param name="Tabela"></param>
		/// <returns></returns>
		public bool DeleteRegisto(int iCodigo, string Tabela)
		{
			string codigo = "Id";

			string Query = $"DELETE {Tabela} WHERE {codigo} = @Codigo";

			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Codigo", iCodigo);

			try
			{
				using (var connection = _context.CreateConnection())
				{
					connection.Execute(Query, paramCollection);
					return true;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return false;
			}
		}


		/// <summary>
		/// Get IdByDescription/Table, de uma determinada tabela
		/// </summary>
		/// <param name="Descricao"></param>
		/// <param name="Tabela"></param>
		/// <returns></returns>
		public int GetId(string Descricao, string Tabela)
		{
			string descricao = "Descricao";
			string codigo = "Id";

			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Descricao", Descricao);
			string Query = $"SELECT {codigo} FROM {Tabela} WHERE {descricao} = @Descricao";

			try
			{
				using (var connection = _context.CreateConnection())
				{
					return connection.QueryFirstOrDefault<int>(Query, paramCollection);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return 0;
			}
		}

		public int GetCodByDescricao(string sDescr, string sTabela)
		{
			string descricao = "Descricao";
			string codigo = "Id";

			StringBuilder sqlCommand = new StringBuilder();
			sqlCommand.Append($"SELECT {codigo} ");
			sqlCommand.Append("FROM " + sTabela);
			sqlCommand.Append($" WHERE {descricao} LIKE '" + sDescr + "'");
			try
			{
				using (var connection = _context.CreateConnection())
				{
					return Convert.ToInt32(connection.QueryFirstOrDefault<int>(sqlCommand.ToString()));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return 0;
			}
		}

		/// <summary>
		/// Get Descricao - genérico - tabelas auxiliares
		/// </summary>
		/// <param name="Codigo"></param>
		/// <param name="Tabela"></param>
		/// <returns></returns>
		public string GetDescricao(int Codigo, string Tabela)
		{
			string descricao = "Descricao";
			string codigo = "Id";

			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Codigo", Codigo);

			string Query = $"SELECT {descricao} FROM {Tabela} WHERE {codigo} = @Codigo";

			try
			{
				using (var connection = _context.CreateConnection())
				{
					return connection.QueryFirstOrDefault<string>(Query, paramCollection).ToString();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return "";
			}
		}

		/// <summary>
		/// Get Descrição with LIKE... (versão 2)
		/// </summary>
		/// <param name="Descricao"></param>
		/// <param name="sTabela">Tabela a pesquisar Descrição</param>
		/// <returns></returns>
		public IEnumerable<LookupTableVM> GetDataByDescricao(string sDescricao, string sTabela, bool TemAlertas = false)
		{
			string descricao = "Descricao";
			string codigo = "Id";

			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Descricao", sDescricao);

			StringBuilder sqlCommand = new StringBuilder($"SELECT {codigo}, {descricao} ");
			if (TemAlertas)
				sqlCommand.Append(", Alerta ");

			sqlCommand.Append($"FROM {sTabela} ");

			if (sDescricao != string.Empty)
				sqlCommand.Append($" WHERE {descricao} LIKE + '%' + @Descricao + '%' ");

			sqlCommand.Append($" ORDER BY {descricao} ");

			try
			{
				using (var connection = _context.CreateConnection())
				{
					return connection.Query<LookupTableVM>(sqlCommand.ToString(), paramCollection);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return null;
			}
		}

		public IEnumerable<LookupTableVM> GenericGetAll(string sTabela)
		{
			string descricao = "Descricao";
			string codigo = "Id";

			StringBuilder sb = new StringBuilder();
			//caso especial - especialidades
			sb.Append($"SELECT {codigo}, {descricao} ");

			sb.Append($"FROM {sTabela} ");

			try
			{
				using (var connection = _context.CreateConnection())
				{
					var lst = connection.Query<LookupTableVM>(sb.ToString());
					return lst;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return null;
			}
		}

		public IEnumerable<LookupTableVM> GetDescricaoByDescricao(string sDescricao, string sTabela)
		{
			string descricao = "Descricao";
			string where = descricao;
			StringBuilder sqlCommand = new StringBuilder();

			sqlCommand.Append($"SELECT {descricao} ");

			sqlCommand.Append($"FROM {sTabela} ");
			sqlCommand.Append($"WHERE {descricao} LIKE '%{sDescricao}%' ");
			sqlCommand.Append("ORDER BY {descricao}");

			try
			{
				using (var connection = _context.CreateConnection())
				{
					return connection.Query<LookupTableVM>(sqlCommand.ToString());
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return null;
			}
		}

		public IEnumerable<LookupTableVM> GetDataFromTabela(string sTabela, string sDescricao = "")
		{
			string descricao = "Descricao";
			string codigo = "Id";

			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Descricao", sDescricao);

			StringBuilder sqlCommand = new StringBuilder($"SELECT {codigo}, {descricao} ");

			sqlCommand.Append($"FROM {sTabela} ");

			if (sDescricao != string.Empty)
				sqlCommand.Append($" WHERE {descricao} LIKE + '%' + @Descricao + '%' ");

			sqlCommand.Append(" ORDER BY {descricao} ");

			try
			{
				using (var connection = _context.CreateConnection())
				{
					return connection.Query<LookupTableVM>(sqlCommand.ToString(), paramCollection);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return null;
			}
		}

		public int GetLastInsertedId(string tableToCheck)
		{
			string id = "Id";

			StringBuilder sb = new StringBuilder();
			sb.Append($"SELECT TOP 1 {id} FROM {tableToCheck} ");
			sb.Append($"ORDER BY {id} DESC");
			try
			{
				using (var connection = _context.CreateConnection())
				{
					var lastId = connection.QueryFirstOrDefault<int>(sb.ToString());
					return lastId;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return 0;
			}
		}

		public LookupTableVM GetRecordById(int id, string tableName)
		{
			DynamicParameters paramCollection = new DynamicParameters();
			paramCollection.Add("@Id", id);

			StringBuilder sb = new StringBuilder();
			sb.Append($"SELECT Id, Descricao FROM {tableName} ");
			sb.Append("WHERE Id = @Id");
			try
			{
				using (var connection = _context.CreateConnection())
				{
					var record = connection.QueryFirstOrDefault<LookupTableVM>(sb.ToString(), param: paramCollection);
					return record;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				return null;
			}
		}
	}
}
