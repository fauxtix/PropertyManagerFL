using CommonLayer.Factories;
using Dapper;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.SqlServerGenerics;
using System.Data;

namespace PropertyManagerFL.Infrastructure.SqlServerGenerics
{
	public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		public IDapperContext _dapperContext;

		protected BaseRepository(IDapperContext dapperContext)
		{
			_dapperContext = dapperContext;
		}
		public async Task<long> Insert_Async(T entity)
		{
			var spInsert = $"usp_{typeof(T).Name}_Insert";
			long recordInserted = 0;
			try
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					var regs = await connection.ExecuteAsync(spInsert, entity, commandType: CommandType.StoredProcedure);
				}
			}
			catch
			{
				throw;
			}

			return recordInserted;
		}

		public async Task Update_Async(T entity)
		{
			var spUpdate = $"usp_{typeof(T).Name}_Update";

			try
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					int operationSucceded = await connection.ExecuteAsync(spUpdate, entity, commandType: CommandType.StoredProcedure);
				}
			}
			catch
			{
				throw;
			}

		}

		public async Task DeleteById_Async(int Id)
		{
			var spDelete = $"usp_{typeof(T).Name}_Delete";

			try
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					await connection.ExecuteAsync(spDelete, new { Id }, commandType: CommandType.StoredProcedure);
				}
			}
			catch
			{
				throw;
			}
		}

		public virtual long Insert(T entity)
		{
			var spInsert = $"usp_{typeof(T).Name}_Insert";
			long RecordInserted = 0;
			try
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					var regs = connection.Execute(spInsert, entity, commandType: CommandType.StoredProcedure);
					RecordInserted = (long)GetLastId();
				}

			}
			catch
			{
				throw;
			}

			return RecordInserted;
		}

		public virtual void DeleteById(int Id)
		{
			var spDelete = $"usp_{typeof(T).Name}_Delete";

			try
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					connection.Execute(spDelete, new { Id }, commandType: CommandType.StoredProcedure);
				}
			}
			catch
			{
				throw;
			}
		}

		public virtual void Update(T entity)
		{
			var spUpdate = $"usp_{typeof(T).Name}_Update";

			try
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					connection.Execute(spUpdate, entity, commandType: CommandType.StoredProcedure);
				}
			}
			catch
			{
				throw;
			}
		}

		public virtual IEnumerable<T> Query(string where = "")
		{

			bool CallSP = string.IsNullOrEmpty(where);
			var query = $"SELECT * FROM {typeof(T).Name} ";

			if (!string.IsNullOrWhiteSpace(where))
			{
				query += $"WHERE {where}";
			}
			else
			{
				query = $"usp_{typeof(T).Name}_GetAll";
			}

			using (var connection = _dapperContext.CreateConnection())
			{
				if (!CallSP)
					return connection.Query<T>(query);
				else
					return connection.Query<T>(query, commandType: CommandType.StoredProcedure);
			}
		}

		public virtual T Query_ById(int Id)
		{
			var spQueryId = $"usp_{typeof(T).Name}_GetById";

			using (IDbConnection connection = ConnectionManager.GetConnection())
			{
				return connection.QueryFirstOrDefault<T>(spQueryId, new { Id },
					commandType: CommandType.StoredProcedure);
			}
		}
		public virtual bool EntradaExiste_BD(string campo, string str2Search)
		{
			using (IDbConnection connection = ConnectionManager.GetConnection())
			{
				var output = Query($"{campo} = '{str2Search}'");
				if (output.Count() > 0)
					return true;
			}
			return false;
		}

		public bool TableHasData()
		{
			var spQueryAll = $"usp_{typeof(T).Name}_GetAll";

			using (var connection = _dapperContext.CreateConnection())
			{
				string sql = $"SELECT * FROM {typeof(T).Name}";
				int result = connection.Query<int>(sql).Count();
				//int result = connection.Query<int>(spQueryAll, commandType: CommandType.StoredProcedure).Count();
				return result > 0;
			}
		}

		public int GetFirstId()
		{
			if (TableHasData())
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					string sql = $"SELECT TOP 1 Id FROM {typeof(T).Name} ORDER BY Id ";
					int result = connection.Query<int>(sql).SingleOrDefault();
					return result;
				}
			}
			else
				return 0;
		}

		public int GetLastId()
		{
			try
			{
				if (TableHasData())
				{
					using (var connection = _dapperContext.CreateConnection())
					{
						string sql = $"SELECT TOP 1 Id FROM {typeof(T).Name} ORDER BY Id DESC";
						int lastId = connection.Query<int>(sql).FirstOrDefault();
						return lastId;
					}
				}
				else
					return 0;
			}
			catch
			{
				throw;
			}
		}

		public bool RecInUse(int Id)
		{
			var uSP = $"usp_{typeof(T).Name}_RecInUse_ById";
			using (var connection = _dapperContext.CreateConnection())
			{
				bool exists = connection.Query<bool>(uSP, new { Id }, commandType: CommandType.StoredProcedure)
					.SingleOrDefault();
				return exists;
			}
		}
		public bool RecInUse(string description)
		{
			var uSP = $"usp_{typeof(T).Name}_RecInUse_ByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				bool exists = connection.Query<bool>(uSP, new { Description = description }, commandType: CommandType.StoredProcedure)
					.SingleOrDefault();
				return exists;
			}
		}

		public int GetIdByDescription(string Descricao)
		{
			var uSP = $"usp_{typeof(T).Name}_GetId_ByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				int iID = connection.Query<int>(uSP, new { Descricao }, commandType: CommandType.StoredProcedure)
					.SingleOrDefault();
				return iID;
			}
		}

		public string GetDescriptionById(int Id)
		{
			var uSP = $"usp_{typeof(T).Name}_GetDescription_ById";
			using (var connection = _dapperContext.CreateConnection())
			{
				string sDescricao = connection.QuerySingleOrDefault<string>(uSP, new { Id }, commandType: CommandType.StoredProcedure);
				return sDescricao;
			}
		}

		public bool RecInUse_ByDescription(string Descricao)
		{
			var uSP = $"usp_{typeof(T).Name}_RecInUse_ByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				bool exists = connection.Query<bool>(uSP, new { Descricao }, commandType: CommandType.StoredProcedure)
					.SingleOrDefault();
				return exists;
			}
		}

		public List<T> Listar()
		{
			var uSP = $"usp_{typeof(T).Name}_GetAll";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = connection.Query<T>(uSP, commandType: CommandType.StoredProcedure)
					.ToList();
				return recordList;
			}
		}

		public IEnumerable<T> ListByDescription(string Descricao)
		{
			var uSP = $"usp_{typeof(T).Name}_ListByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = connection.Query<T>(uSP, new { Descricao }, commandType: CommandType.StoredProcedure)
					.ToList();
				return recordList;
			}
		}

		public IDataReader ListByDescription_Reader(string Descricao)
		{
			var uSP = $"usp_{typeof(T).Name}_ListByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = connection.ExecuteReader(uSP, new { Descricao }, commandType: CommandType.StoredProcedure);

				return recordList;
			}
		}

		public IEnumerable<T> Listar_ID(int Id)
		{
			var uSP = $"usp_{typeof(T).Name}_ListById";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = connection.Query<T>(uSP, new { Id }, commandType: CommandType.StoredProcedure)
					.ToList();
				return recordList;
			}
		}

		public IDataReader Listar_ID_DR(int Id)
		{
			var uSP = $"usp_{typeof(T).Name}_ListById";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = connection.ExecuteReader(uSP, new { Id }, commandType: CommandType.StoredProcedure);
				return recordList;
			}

		}

		public async Task<IEnumerable<T>> Query_Async(string where = "")
		{
			using (var connection = _dapperContext.CreateConnection())
			{
				var query = $"usp_{typeof(T).Name}_GetAll";
				var output = await connection.QueryAsync<T>(query, commandType: CommandType.StoredProcedure);
				return output;
			}
		}

		// Async methods
		public async Task<T> Query_ById_Async(int Id)
		{
			var spQueryId = $"usp_{typeof(T).Name}_GetById";

			using (IDbConnection connection = ConnectionManager.GetConnection())
			{
				var result = await connection.QueryFirstOrDefaultAsync<T>(spQueryId, new { Id },
					commandType: CommandType.StoredProcedure);
				return result;
			}
		}

		public async Task<int> GetIdByDescription_Async(string Descricao)
		{
			var uSP = $"usp_{typeof(T).Name}_GetId_ByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				int iID = await connection.QuerySingleOrDefaultAsync<int>(uSP, new { Descricao },
					commandType: CommandType.StoredProcedure);
				return iID;
			}
		}

		public async Task<string> GetDescriptionById_Async(int Id)
		{
			var uSP = $"usp_{typeof(T).Name}_GetDescription_ById";
			using (var connection = _dapperContext.CreateConnection())
			{
				string sDescricao = await connection.QuerySingleOrDefaultAsync<string>(uSP, new { Id },
					commandType: CommandType.StoredProcedure);
				return sDescricao;
			}
		}

		public async Task<List<T>> Listar_Async()
		{
			var uSP = $"usp_{typeof(T).Name}_GetAll";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = (await connection.QueryAsync<T>(uSP, commandType: CommandType.StoredProcedure))
					.ToList();
				return recordList;
			}
		}

		public async Task<IEnumerable<T>> ListByDescription_Async(string Descricao)
		{
			var uSP = $"usp_{typeof(T).Name}_ListByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = (await connection.QueryAsync<T>(uSP, new { Descricao },
					commandType: CommandType.StoredProcedure))
					.ToList();
				return recordList;
			}
		}

		public async Task<IDataReader> ListByDescription_Reader_Async(string Descricao)
		{
			var uSP = $"usp_{typeof(T).Name}_ListByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = await connection.ExecuteReaderAsync(uSP, new { Descricao },
					commandType: CommandType.StoredProcedure);

				return recordList;
			}
		}

		public async Task<IEnumerable<T>> Listar_ID_Async(int Id)
		{
			var uSP = $"usp_{typeof(T).Name}_ListById";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = (await connection.QueryAsync<T>(uSP, new { Id },
					commandType: CommandType.StoredProcedure))
					.ToList();
				return recordList;
			}
		}

		public async Task<IDataReader> Listar_ID_DR_Async(int Id)
		{
			var uSP = $"usp_{typeof(T).Name}_ListById";
			using (var connection = _dapperContext.CreateConnection())
			{
				var recordList = await connection.ExecuteReaderAsync(uSP, new { Id },
					commandType: CommandType.StoredProcedure);
				return recordList;
			}
		}

		public async Task<bool> EntradaExiste_BD_Async(string campo, string str)
		{
			using (IDbConnection connection = ConnectionManager.GetConnection())
			{
				var output = await Query_Async($"{campo} = '{str}'");
				if (output.Count() > 0)
					return true;
			}
			return false;
		}

		public async Task<bool> RecInUse_Async(int Id)
		{
			var uSP = $"usp_{typeof(T).Name}_RecInUse_ById";
			using (var connection = _dapperContext.CreateConnection())
			{
				bool exists = (await connection.QueryAsync<bool>(uSP, new { Id }, commandType: CommandType.StoredProcedure))
					.SingleOrDefault();
				return exists;
			}
		}

		public async Task<bool> RecInUse_ByDescription_Async(string description)
		{
			var uSP = $"usp_{typeof(T).Name}_RecInUse_ByDescription";
			using (var connection = _dapperContext.CreateConnection())
			{
				bool exists = (await connection.QueryAsync<bool>(uSP, new { Descricao = description }, commandType: CommandType.StoredProcedure))
					.SingleOrDefault();
				return exists;
			}
		}

		public async Task<bool> TableHasData_Async()
		{
			var spQueryAll = $"usp_{typeof(T).Name}_GetAll";

			using (var connection = _dapperContext.CreateConnection())
			{
				int result = (await connection.QueryAsync<int>(spQueryAll, commandType: CommandType.StoredProcedure)).Count();
				return result > 0;
			}
		}

		public async Task<int> GetFirstId_Async()
		{
			string sql = "";
			if (await TableHasData_Async())
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					int result = (await connection.QueryAsync<int>(sql)).SingleOrDefault();
					return result;
				}
			}
			else
				return 0;
		}

		public async Task<int> GetLastId_Async()
		{
			if (await TableHasData_Async())
			{
				using (var connection = _dapperContext.CreateConnection())
				{
					var sql = $"SELECT TOP 1 Id FROM {typeof(T).Name} ORDER BY Id DESC";
					int lastId = (await connection.QueryAsync<int>(sql)).FirstOrDefault();
					return lastId;
				}
			}
			else
				return 0;
		}
	}
}
