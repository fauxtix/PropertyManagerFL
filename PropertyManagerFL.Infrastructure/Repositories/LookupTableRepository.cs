using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Infrastructure.Context;
using PropertyManagerFL.Infrastructure.Security;
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
        public async Task<IEnumerable<LookupTableVM>> GetLookupTableData(string tableName)
        {
            tableName = SafePath.SqlIdentifier(tableName);
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
            tableName = SafePath.SqlIdentifier(tableName);
            DynamicParameters paramCollection = new DynamicParameters();
            paramCollection.Add("@Id", id);

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT Descricao ");
            sql.Append($"FROM {tableName} WHERE Id = @Id ");
            using (var connection = _context.CreateConnection())
            {
                var result = connection.QueryFirstOrDefault<string>(sql.ToString(), paramCollection);
                if (string.IsNullOrEmpty(result)) return string.Empty;
                if (result.StartsWith("'") && result.EndsWith("'"))
                    result = result.Substring(1, result.Length - 1).Replace("\\\\", "\\");
                return result;
            }
        }

        public bool CriaNovoRegisto(LookupTableVM tableRecord)
        {
            var table = SafePath.SqlIdentifier(tableRecord.Tabela);
            string descricao = "Descricao";

            DynamicParameters paramCollection = new DynamicParameters();
            paramCollection.Add("@Descricao", tableRecord.Descricao);

            string Query = $"INSERT INTO {table} ({descricao}) VALUES (@Descricao)";
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
                _logger.LogError(ex, "Lookup table operation failed");
                return false;
            }
        }

        /// <summary>
        /// Atualiza Registo - gen�rico - tabelas auxiliares
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="Descricao"></param>
        /// <param name="Tabela"></param>
        /// <returns></returns>
        public bool ActualizaDetalhes(LookupTableVM tableRecord)
        {
            var table = SafePath.SqlIdentifier(tableRecord.Tabela);

            DynamicParameters paramCollection = new DynamicParameters();
            paramCollection.Add("@Id", tableRecord.Id);
            paramCollection.Add("@Description", tableRecord.Descricao);

            string Query = $"UPDATE {table} SET Descricao = @Description WHERE Id = @Id";

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
                _logger.LogError(ex, "Lookup table operation failed");
                return false;
            }
        }


        /// <summary>
        /// Check Registo Exist - gen�rico - tabelas auxiliares
        /// </summary>
        /// <param name="Descricao"></param>
        /// <param name="Tabela"></param>
        /// <returns></returns>
        public bool CheckRegistoExist(string Descricao, string Tabela)
        {
            Tabela = SafePath.SqlIdentifier(Tabela);
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
                _logger.LogError(ex, "Lookup table operation failed");
                return false;
            }
        }

        /// <summary>
        /// Check record Exist in child tables => gen�rico - tabelas auxiliares
        /// </summary>
        /// <param name="Descricao"></param>
        /// <param name="Tabela"></param>
        /// <returns></returns>
        public bool CheckFKInUse(int sourceFk, string fieldToCheck, string tableToCheck)
        {
            fieldToCheck = SafePath.SqlIdentifier(fieldToCheck);
            tableToCheck = SafePath.SqlIdentifier(tableToCheck);
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
                _logger.LogError(ex, "Lookup table operation failed");
                return false;
            }
        }

        /// <summary>
        /// Delete record => gen�rico - tabelas auxiliares
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="Tabela"></param>
        /// <returns></returns>
        public bool DeleteRegisto(int iCodigo, string Tabela)
        {
            Tabela = SafePath.SqlIdentifier(Tabela);
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
                _logger.LogError(ex, "Lookup table operation failed");
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
            Tabela = SafePath.SqlIdentifier(Tabela);
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
                _logger.LogError(ex, "Lookup table operation failed");
                return 0;
            }
        }

        public int GetCodByDescricao(string sDescr, string sTabela)
        {
            sTabela = SafePath.SqlIdentifier(sTabela);
            string descricao = "Descricao";
            string codigo = "Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Descricao", sDescr);

            StringBuilder sqlCommand = new StringBuilder();
            sqlCommand.Append($"SELECT {codigo} ");
            sqlCommand.Append("FROM " + sTabela);
            sqlCommand.Append($" WHERE {descricao} LIKE @Descricao");
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return Convert.ToInt32(connection.QueryFirstOrDefault<int>(sqlCommand.ToString(), parameters));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lookup table operation failed");
                return 0;
            }
        }

        /// <summary>
        /// Get Descricao - gen�rico - tabelas auxiliares
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="Tabela"></param>
        /// <returns></returns>
        public string GetDescricao(int Codigo, string Tabela)
        {
            Tabela = SafePath.SqlIdentifier(Tabela);
            string descricao = "Descricao";
            string codigo = "Id";

            DynamicParameters paramCollection = new DynamicParameters();
            paramCollection.Add("@Codigo", Codigo);

            string Query = $"SELECT {descricao} FROM {Tabela} WHERE {codigo} = @Codigo";

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = connection.QueryFirstOrDefault<string>(Query, paramCollection).ToString();

                    if (result.IndexOf("\\") == -1) return result;

                    result = result.Substring(1, result.Length - 1).Replace("\\\\", "\\");
                    result = result.Substring(0, result.Length - 1);
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lookup table operation failed");
                return "";
            }
        }

        /// <summary>
        /// Get Descri��o with LIKE... (vers�o 2)
        /// </summary>
        /// <param name="Descricao"></param>
        /// <param name="sTabela">Tabela a pesquisar Descri��o</param>
        /// <returns></returns>
        public IEnumerable<LookupTableVM> GetDataByDescricao(string sDescricao, string sTabela, bool TemAlertas = false)
        {
            sTabela = SafePath.SqlIdentifier(sTabela);
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
                _logger.LogError(ex, "Lookup table operation failed");
                return null;
            }
        }

        public IEnumerable<LookupTableVM> GenericGetAll(string sTabela)
        {
            sTabela = SafePath.SqlIdentifier(sTabela);
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
                _logger.LogError(ex, "Lookup table operation failed");
                return null;
            }
        }

        public IEnumerable<LookupTableVM> GetDescricaoByDescricao(string sDescricao, string sTabela)
        {
            sTabela = SafePath.SqlIdentifier(sTabela);
            string descricao = "Descricao";

            var parameters = new DynamicParameters();
            parameters.Add("@Descricao", $"%{sDescricao}%");

            StringBuilder sqlCommand = new StringBuilder();

            sqlCommand.Append($"SELECT {descricao} ");

            sqlCommand.Append($"FROM {sTabela} ");
            sqlCommand.Append($"WHERE {descricao} LIKE @Descricao ");
            sqlCommand.Append($"ORDER BY {descricao}");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return connection.Query<LookupTableVM>(sqlCommand.ToString(), parameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lookup table operation failed");
                return null;
            }
        }

        public IEnumerable<LookupTableVM> GetDataFromTabela(string sTabela, string sDescricao = "")
        {
            sTabela = SafePath.SqlIdentifier(sTabela);
            string descricao = "Descricao";
            string codigo = "Id";

            DynamicParameters paramCollection = new DynamicParameters();
            paramCollection.Add("@Descricao", sDescricao);

            StringBuilder sqlCommand = new StringBuilder($"SELECT {codigo}, {descricao} ");

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
                _logger.LogError(ex, "Lookup table operation failed");
                return null;
            }
        }

        public int GetLastInsertedId(string tableToCheck)
        {
            tableToCheck = SafePath.SqlIdentifier(tableToCheck);
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
                _logger.LogError(ex, "Lookup table operation failed");
                return 0;
            }
        }

        public LookupTableVM GetRecordById(int id, string tableName)
        {
            tableName = SafePath.SqlIdentifier(tableName);
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
                _logger.LogError(ex, "Lookup table operation failed");
                return null;
            }
        }
    }
}
