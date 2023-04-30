using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Proprietarios;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class ProprietarioRepository : IProprietarioRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<ProprietarioRepository> _logger;

        public ProprietarioRepository(DapperContext context, ILogger<ProprietarioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Insert(NovoProprietario entity)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_Proprietarios_Insert",
                         param: entity, commandType: CommandType.StoredProcedure);

                    return insertedId;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<bool> Update(AlteraProprietario entity)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var updateOk = await connection.ExecuteAsync("usp_Proprietarios_Update",
                         param: entity, commandType: CommandType.StoredProcedure);

                    return updateOk > 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Proprietarios_Delete",
                    param: parameters, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public int GetFistId()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Proprietario> Query(string where = null)
        {
            throw new NotImplementedException();
        }

        public async Task<Proprietario> Query_ById(int Id)
        {
            throw new NotImplementedException();
        }

        public string RegistoComErros(Proprietario proprietario)
        {
            throw new NotImplementedException();
        }

        public async Task< bool> TableHasData()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<bool>("usp_Proprietarios_Existe",
                        commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }

        }
        public int GetFirstId()
        {
            var query = "SELECT TOP 1 Id FROM Proprietarios";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = connection.QueryFirstOrDefault<int>(query);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }

        }

        public async Task<ProprietarioVM> GetProprietario_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<ProprietarioVM>("usp_Proprietarios_GetById",
                        param: new { Id = id }, commandType: CommandType.StoredProcedure);
                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }
    }
}
