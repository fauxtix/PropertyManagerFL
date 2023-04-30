using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Logins;
using PropertyManagerFL.Application.ViewModels.Logs;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Infrastructure.Context;
using System.Data;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<LogRepository> _logger;

        public LogRepository(DapperContext context, ILogger<LogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        /// <summary>
        /// Cria log de operações realizadas pelo utilizador
        /// </summary>
        /// <param name="IdReg"></param>
        /// <param name="sTabela"></param>
        /// <param name="iUser"></param>
        /// <param name="Operacao"></param>
        /// <param name="dMovimento"></param>
        public void LogCRUD(LogCRUDModel model)
        {
            string SpName = "";
            DynamicParameters paramCollection = new DynamicParameters();

            switch (model.Action)
            {
                case OpcaoCRUD.Inserir:
                    paramCollection.Add("@Tabela", model.TableName);
                    paramCollection.Add("@IdReg", model.Id, DbType.Int16);
                    paramCollection.Add("@QuemCriou", model.UserId);
                    paramCollection.Add("@DataCriacao", DateTime.UtcNow, dbType: DbType.DateTime);

                    SpName = "usp_Log_Operacoes_Create";
                    break;
                case OpcaoCRUD.Atualizar:
                    paramCollection.Add("@Tabela", model.TableName);
                    paramCollection.Add("@IdReg", model.Id, DbType.Int16);
                    paramCollection.Add("@QuemModificou", model.UserId);
                    paramCollection.Add("@DataUltimaAlteracao", DateTime.UtcNow, dbType: DbType.DateTime);

                    SpName = "usp_Log_Operacoes_Update";
                    break;
                case OpcaoCRUD.Anular:
                    paramCollection.Add("@Tabela", model.TableName);
                    paramCollection.Add("@IdReg", model.Id, DbType.Int16);
                    paramCollection.Add("@QuemApagou", model.UserId);
                    paramCollection.Add("@DataAnulacao", DateTime.UtcNow, dbType: DbType.DateTime);

                    SpName = "usp_Log_Operacoes_Delete";
                    break;
                case OpcaoCRUD.Ler:
                    SpName = "usp_Log_Login_Select";
                    break;
            }
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Execute(SpName, paramCollection, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        public void LogLogin(string userId, OpcaoCRUD Operacao, string sessionId)
        {
            string SpName = "";
            DynamicParameters paramCollection = new DynamicParameters();

            switch (Operacao)
            {
                case OpcaoCRUD.Inserir:
                    paramCollection.Add("@UserId", userId);
                    paramCollection.Add("@SessionId", sessionId);
                    paramCollection.Add("@LoginDate", DateTime.Now, dbType: DbType.DateTime);

                    SpName = "usp_Log_Login_Create";
                    break;
                case OpcaoCRUD.Atualizar:
                    paramCollection.Add("@SessionId", sessionId);
                    paramCollection.Add("@LogoutDate", DateTime.Now, dbType: DbType.DateTime);

                    SpName = "usp_Log_Login_Update";
                    break;
            }
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Execute(SpName, paramCollection, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public async Task<IEnumerable<LoginLogVM>> GetLogins()
        {
            string SpName = "usp_Log_Login_Select";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<LoginLogVM>(SpName, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public async Task<LoginLogVM> GeUserNameAndEmail(string userId)
        {
            string SpName = "usp_GetUserEmailAndName";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryFirstAsync<LoginLogVM>(SpName,
                       new { UserId = userId },
                       commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// PMLogs
        /// </summary>
        /// <returns>List of PMLog</returns>
        public async Task<IEnumerable<AppLog>> GetAppLogs()
        {
            string SpName = "usp_Logs_GetLogs";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<AppLog>(SpName,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, exc);
                return null;
            }
        }
        public async Task<AppLog> GetAppLog_ById(int Id)
        {
            string SpName = "usp_Logs_ById";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryFirstOrDefaultAsync<AppLog>(SpName,
                        new { Id },
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, exc);
                return null;
            }
        }
        public async Task DeleteAll()
        {
            string SpName = "usp_Logs_DeleteAll";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(SpName,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, exc);
            }
        }

        public async Task DeleteById(int Id)
        {
            string SpName = "usp_Logs_DeleteById";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(SpName, 
                        new { Id},
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, exc);
            }
        }

        public async Task DeleteByFilter(IEnumerable<AppLogDto> filteredLogs)
        {
            string SpName = "usp_Logs_DeleteById";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    foreach (var log in filteredLogs)
                    {
                        await connection.ExecuteAsync(SpName,
                            new { log.Id },
                            commandType: CommandType.StoredProcedure);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, exc);
            }
        }


        public async Task<IEnumerable<AppLog>> FilterLogins()
        {
            string SpName = "usp_Logs_GetLoginEntries";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                   return await connection.QueryAsync<AppLog>(SpName,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, exc);
                return null;
            }
        }
    }
}
