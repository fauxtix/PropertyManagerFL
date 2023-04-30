using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Despesas;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class DespesaRepository : IDespesaRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<DespesaRepository> _logger;

        public DespesaRepository(DapperContext context, ILogger<DespesaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Insert(NovaDespesa expense)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_Despesas_Insert",
                         param: expense, commandType: CommandType.StoredProcedure);

                    return insertedId;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }


        public async Task<bool> Update(AlteraDespesa expense)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var updateOk = await connection.QueryFirstAsync<bool>("usp_Despesas_Update",
                         param: expense, commandType: CommandType.StoredProcedure);

                    return updateOk;
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
                    await connection.ExecuteAsync("usp_Despesas_Delete",
                     param: parameters, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<IEnumerable<DespesaVM>> GetAll()
        {
            try
            {
                _logger.LogInformation("Getting All Expenses");

                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<DespesaVM>("usp_Despesas_GetAll",
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

        public async Task<DespesaVM> GetDespesa_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<DespesaVM>("usp_Despesas_GetById",
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

        public List<DespesaVM> GetResumedData()
        {
            throw new NotImplementedException();
        }



        public List<DespesaVM> Query_ByYear(string sAno)
        {
            throw new NotImplementedException();
        }

        public decimal TotalDespesas(int iTipoDespesa = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TipoDespesaVM>> GetTipoDespesa_ByCategoriaDespesa(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<TipoDespesaVM>("usp_Despesas_GetTipoDespesaByCategory",
                     param: parameters, commandType: CommandType.StoredProcedure);
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
