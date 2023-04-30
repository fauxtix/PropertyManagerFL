using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class FiadorRepository : IFiadorRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<FiadorRepository> _logger;

        /// <summary>
        /// Repositório de Fiadores
        /// </summary>
        /// <param name="context"></param>
        public FiadorRepository(DapperContext context, ILogger<FiadorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsereFiador(NovoFiador Fiador)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_Fiadores_Insert",
                         param: Fiador, commandType: CommandType.StoredProcedure);

                    return insertedId;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }
        public async Task<Fiador> AtualizaFiador(AlteraFiador Fiador)
        {
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var FiadorAlterado = await connection.QuerySingleOrDefaultAsync<Fiador>("usp_Fiadores_Update",
                param: Fiador, commandType: CommandType.StoredProcedure);
                    return FiadorAlterado;

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return null;
                }
            }
        }

        public async Task ApagaFiador(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("usp_Fiadores_Delete",
                param: parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<FiadorVM>> GetAll()
        {
            try
            {
                _logger.LogInformation("Get todos os fiadores");

                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<FiadorVM>("usp_Fiadores_GetAll",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<FiadorVM> GetFiador_ById(int idFiador)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<FiadorVM>("usp_Fiadores_GetById",
                        param: new { Id = idFiador }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (System.Data.SqlClient.SqlException ex2)
            {
                _logger.LogError(ex2.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public int GetFirstId_Fiador()
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = $"SELECT TOP 1 Id FROM Fiadores WHERE Titular = 0 ORDER BY Id";
                int result = connection.QueryFirstOrDefault<int>(sql);
                return result;
            }
        }
        public int GetFirstIdFiador()
        {
            using (var connection = _context.CreateConnection())
            {
                string sql = $"SELECT TOP 1 Id FROM Fiadores WHERE Titular = 1 ORDER BY Id";
                int result = connection.QueryFirstOrDefault<int>(sql);
                return result;
            }
        }

        /// <summary>
        /// Preenche combos de Fiadores
        /// </summary>
        public async Task<IEnumerable<LookupTableVM>> GetFiadores_ForLookUp()
        {
            using (var connection = _context.CreateConnection())
            {
                var FiadoresOuFiadores = await connection.QueryAsync<LookupTableVM>("usp_Fiadores_GetForLookup",
                     commandType: CommandType.StoredProcedure);

                return FiadoresOuFiadores;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetFiadoresDisponiveis()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var FiadoresDisponiveis = await connection.QueryAsync<LookupTableVM>("usp_Fiadores_Get_Disponiveis",
                        commandType: CommandType.StoredProcedure);

                    return FiadoresDisponiveis;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<string> GetNomeFiador(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var nomeFiador = await connection.QueryFirstOrDefaultAsync<string>("usp_Fiadores_GetNome",
                     param: new { Id = id }, commandType: CommandType.StoredProcedure);

                return nomeFiador;
            }

        }
        public async Task<FiadorVM> GetFiadorInquilino(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var fiadorInquilino = await connection.QueryFirstOrDefaultAsync<FiadorVM>("usp_Fiadores_Get_ByTenant",
                     param: new { Id = id }, commandType: CommandType.StoredProcedure);

                return fiadorInquilino;
            }

        }
        public async Task AtualizaSaldo(int id, decimal saldoCorrente)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@NovoSaldoCorrente", saldoCorrente);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("usp_Fiadores_UpdateSaldoCorrente",
                param: parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> CanFiadorBeDeleted(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@OkToDelete", SqlDbType.Bit, direction: ParameterDirection.Output);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<int>("usp_Fiadores_CheckDeleteConstraint",
                    param: parameters, commandType: CommandType.StoredProcedure);

                    var areThereContracts = parameters.Get<int>("@OkToDelete");
                    return areThereContracts == 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> TableHasData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT COUNT(1) ");
            sb.Append("FROM Fiadores");

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<int>(sb.ToString());
                return result > 0;
            }

        }

    }
}
