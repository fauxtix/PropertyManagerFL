using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class TipoDespesaRepository : ITipoDespesaRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<TipoDespesaRepository> _logger;

        public TipoDespesaRepository(DapperContext context, ILogger<TipoDespesaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ApagaTipoDespesa(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.ExecuteAsync("usp_Despesas_TipoDespesa_Delete",
                            new { Id = id }, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
            }
        }

        public async Task<bool> AtualizaTipoDespesa(AlteraTipoDespesa ExpenseSubCategory)
        {
            DynamicParameters paramCollection = new DynamicParameters();

            paramCollection.Add("@Id", ExpenseSubCategory.Id);
            paramCollection.Add("@Descricao", ExpenseSubCategory.Descricao);
            paramCollection.Add("@Id_CategoriaDespesa", ExpenseSubCategory.Id_CategoriaDespesa); ;
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Despesas_TipoDespesa_Update",
                            paramCollection, commandType: CommandType.StoredProcedure);
                }

                return true;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return false;
            }

        }

        public async Task<int> InsereTipoDespesa(NovoTipoDespesa ExpenseSubCategory)
        {
            DynamicParameters paramCollection = new DynamicParameters();

            paramCollection.Add("@Descricao", ExpenseSubCategory.Descricao);
            paramCollection.Add("@Id_CategoriaDespesa", ExpenseSubCategory.Id_CategoriaDespesa);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_Despesas_TipoDespesa_Insert",
                            paramCollection, commandType: CommandType.StoredProcedure);
                    return insertedId;

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return -1;
            }
        }

        public async Task<bool> CanRecordBeDeleted(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);
                parameters.Add("@OkToDelete", SqlDbType.Bit, direction: ParameterDirection.Output);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<int>("usp_Despesas_TipoDespesas_CheckDeleteConstraint",
                    param: parameters, commandType: CommandType.StoredProcedure);
                    var areThereExpenses = parameters.Get<int>("@OkToDelete");
                    return areThereExpenses == 0;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<TipoDespesaVM>> GetAll()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var lst = await connection.QueryAsync<TipoDespesaVM>("usp_Despesas_TipoDespesa_GetAll",
                        commandType: CommandType.StoredProcedure);

                    return lst;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return null;
            }
        }

        public async Task<TipoDespesaVM> GetTipoDespesa_ById(int idCategoria)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var subCategoria = await connection.QueryFirstOrDefaultAsync<TipoDespesaVM>("usp_Despesas_TipoDespesa_GetById",
                        new { Id = idCategoria },
                        commandType: CommandType.StoredProcedure);

                    return subCategoria;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message);
                return null;
            }
        }
    }
}
