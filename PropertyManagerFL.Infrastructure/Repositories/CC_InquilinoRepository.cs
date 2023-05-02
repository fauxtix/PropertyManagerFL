using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class CC_InquilinoRepository : ICC_InquilinoRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<CC_InquilinoRepository> _logger;


        public CC_InquilinoRepository(DapperContext context, ILogger<CC_InquilinoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<int> Insert(CC_InquilinoNovo entity)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_CC_Inquilinos_Insert",
                         param: entity,
                         commandType: CommandType.StoredProcedure);

                    return insertedId;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<bool> Update(CC_InquilinoAltera entity)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var updateOk = await connection.ExecuteAsync("usp_CC_Inquilinos_Update",
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
                    await connection.ExecuteAsync("usp_CC_Inquilinos_Delete",
                        param: parameters,
                        commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        public async Task<bool> EntradaExiste_BD(string campo, string str)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetFirstId()
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetLastId()
        {
            throw new NotImplementedException();
        }


        public async Task<IEnumerable<CC_InquilinoVM>> GetAll()
        {
            try
            {
                _logger.LogInformation("Get conta-corrente dos inquilinos");

                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<CC_InquilinoVM>("usp_CC_Inquilinos_GetAll",
                        commandType: CommandType.StoredProcedure);
                    return output.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null!;
            }
        }

        public async Task<CC_InquilinoVM> Query_ById(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RecInUse(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TableHasData()
        {
            throw new NotImplementedException();
        }

    }
}
