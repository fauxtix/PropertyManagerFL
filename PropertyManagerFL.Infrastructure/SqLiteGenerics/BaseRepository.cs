using CommonLayer.Factories;
using Dapper;
using PropertyManagerFL.Application.Exceptions;
using PropertyManagerFL.Application.Interfaces.SqLiteGenerics;
using PropertyManagerFLApplication.Configurations;
using PropertyManagerFLApplication.Exceptions;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Text;

namespace PropertyManagerFL.Infrastructure.SqLiteGenerics
{
	public abstract class BaseRepository<T> : IBaseRepository<T>
	{

		protected string connectionString = ApplicationConfiguration.ConnectionString;
		DataAccessStatus dataAccessStatus = new DataAccessStatus();

		private IEnumerable<string> GetColumns()
		{
			return typeof(T)
					.GetProperties()
					.Where(e => e.Name != "Id" && !e.PropertyType.GetTypeInfo().IsGenericType)
					.Select(e => e.Name);
		}

		public virtual long Insert(T entity)
		{
			var columns = GetColumns();
			var stringOfColumns = string.Join(", ", columns);
			var stringOfParameters = string.Join(", ", columns.Select(e => "@" + e));

			StringBuilder sbInsert = new StringBuilder();
			sbInsert.Append($"INSERT INTO {typeof(T).Name} ");
			sbInsert.Append($"({stringOfColumns}) ");
			sbInsert.Append($"VALUES ({stringOfParameters})");

			string query = sbInsert.ToString();

			try
			{
				using (SQLiteConnection connection = new SQLiteConnection(connectionString))
				{
					connection.Execute(query, entity);
				}

				return GetLastId();
			}
			catch (DataAccessException ex)
			{
				ex.DataAccessStatusInfo.Status = "Erro";
				ex.DataAccessStatusInfo.OperationSucceeded = false;
				ex.DataAccessStatusInfo.CustomMessage = $"Erro na inserção de registo (tabela '{typeof(T).Name}').";
				ex.DataAccessStatusInfo.ExceptionMessage = string.Copy(ex.Message);
				ex.DataAccessStatusInfo.StackTrace = string.Copy(ex.StackTrace);
				throw ex;
			}
		}

		public virtual void Delete(T entity)
		{
			var query = $"DELETE FROM {typeof(T).Name} WHERE Id = @Id";

			try
			{
				using (var connection = new SQLiteConnection(connectionString))
				{
					connection.Execute(query, entity);
				}
			}
			catch (DataAccessException ex)
			{
				ex.DataAccessStatusInfo.Status = "Erro";
				ex.DataAccessStatusInfo.OperationSucceeded = false;
				ex.DataAccessStatusInfo.CustomMessage = $"Erro na anulação de registo (tabela '{typeof(T).Name}').";
				ex.DataAccessStatusInfo.ExceptionMessage = string.Copy(ex.Message);
				ex.DataAccessStatusInfo.StackTrace = string.Copy(ex.StackTrace);
				throw ex;
			}
		}

		public virtual void Update(T entity)
		{
			var columns = GetColumns();
			var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
			var query = $"UPDATE {typeof(T).Name} SET {stringOfColumns} WHERE Id = @Id";

			try
			{
				using (var connection = new SQLiteConnection(connectionString))
				{
					connection.Execute(query, entity);
				}
			}
			catch (DataAccessException ex)
			{
				ex.DataAccessStatusInfo.Status = "Erro";
				ex.DataAccessStatusInfo.OperationSucceeded = false;
				ex.DataAccessStatusInfo.CustomMessage = $"Erro na atualização de registo (tabela '{typeof(T).Name}').";
				ex.DataAccessStatusInfo.ExceptionMessage = string.Copy(ex.Message);
				ex.DataAccessStatusInfo.StackTrace = string.Copy(ex.StackTrace);
				throw ex;
			}
		}

		public virtual IEnumerable<T> Query(string where = "")
		{
			var query = $"SELECT * FROM {typeof(T).Name} ";

			if (!string.IsNullOrWhiteSpace(where))
			{
				query += $"WHERE {where}";
			}

			using (var connection = new SQLiteConnection(connectionString))
			{
				return connection.Query<T>(query);
			}
		}

		public virtual T Query_ById(int Id)
		{

			using (IDbConnection connection = ConnectionManager.GetConnection())
			{
				return connection.Query<T>($"SELECT * FROM {typeof(T).Name} WHERE Id=@Id",
					new { Id }).FirstOrDefault();
			}
		}

		public virtual bool EntradaExiste_BD(string campo, string str)
		{
			bool EntryExists = false;
			using (IDbConnection connection = ConnectionManager.GetConnection())
			{
				EntryExists = Query($"{campo} = '{str}'").Any(); // para debug
				return EntryExists;

				//    var output = Query($"{campo} = '{str}'");
				//    if (output.Count() > 0)
				//        return true;
				//}
				//return false;
			}
		}

		public bool TableHasData()
		{
			using (var connection = ConnectionManager.GetConnection())
			{
				string sql = $"SELECT COUNT(1) FROM {typeof(T).Name}";
				int rows = connection.Query<int>(sql).SingleOrDefault(); // para debug...
				return rows > 0;
			}
		}

		public int GetFirstId()
		{
			if (TableHasData())
			{
				using (var connection = ConnectionManager.GetConnection())
				{
					string sql = $"SELECT Id FROM {typeof(T).Name} ORDER BY Id LIMIT 1";
					int result = connection.ExecuteScalar<int>(sql);

					// Funciona, desde que T venha ordenado por Id...
					//int result = connection.Query<int>(sql).FirstOrDefault();
					return result;
				}
			}
			else
			{
				return 0;
			}
		}

		public int GetLastId()
		{
			if (TableHasData())
			{
				using (var connection = ConnectionManager.GetConnection())
				{
					string sql = $"SELECT Id FROM {typeof(T).Name} ORDER BY Id DESC LIMIT 1";
					int result = connection.ExecuteScalar<int>(sql);
					return result;
				}
			}
			else
			{
				return 0;
			}
		}

		public bool RecInUse(int Id)
		{
			var query = $"SELECT COUNT(1) FROM {typeof(T).Name} WHERE Id = @Id";

			using (var connection = ConnectionManager.GetConnection())
			{
				bool exists = connection.ExecuteScalar<bool>(query, new { Id });
				return exists;
			}
		}

		public void UpdateById(int Id)
		{
			var columns = GetColumns();
			var stringOfColumns = string.Join(", ", columns.Select(e => $"{e} = @{e}"));
			var query = $"UPDATE {typeof(T).Name} SET {stringOfColumns} WHERE Id = @Id";

			try
			{
				using (var connection = ConnectionManager.GetConnection())
				{
					connection.Execute(query, new { Id });
				}
			}
			catch (DataAccessException ex)
			{
				ex.DataAccessStatusInfo.Status = "Erro";
				ex.DataAccessStatusInfo.OperationSucceeded = false;
				ex.DataAccessStatusInfo.CustomMessage = $"Erro na atualização de registo (tabela '{typeof(T).Name}').";
				ex.DataAccessStatusInfo.ExceptionMessage = string.Copy(ex.Message);
				ex.DataAccessStatusInfo.StackTrace = string.Copy(ex.StackTrace);
				throw ex;
			}
		}

		public void DeleteById(int Id)
		{
			var query = $"DELETE FROM {typeof(T).Name} WHERE Id = @Id";

			try
			{
				using (var connection = ConnectionManager.GetConnection())
				{
					connection.Execute(query, new { Id });
				}
			}
			catch (DataAccessException ex)
			{
				ex.DataAccessStatusInfo.Status = "Erro";
				ex.DataAccessStatusInfo.OperationSucceeded = false;
				ex.DataAccessStatusInfo.CustomMessage = $"Erro na anulação - tabela '{typeof(T).Name}'.";
				ex.DataAccessStatusInfo.ExceptionMessage = string.Copy(ex.Message);
				ex.DataAccessStatusInfo.StackTrace = string.Copy(ex.StackTrace);
				throw ex;
			}
		}
	}
}
