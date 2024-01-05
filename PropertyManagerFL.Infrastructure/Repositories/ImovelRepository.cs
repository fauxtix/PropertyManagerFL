using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class ImovelRepository : IImovelRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<ImovelRepository> _logger;


        public ImovelRepository(DapperContext context, ILogger<ImovelRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> InsereImovel(NovoImovel novoImovel)
        {
            try
            {

                using (var connection = _context.CreateConnection())
                {
                    var insertedId = await connection.QueryFirstAsync<int>("usp_Imoveis_Insert",
                         param: novoImovel, commandType: CommandType.StoredProcedure);

                    return insertedId; ;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<AlteraImovel?> AtualizaImovel(AlteraImovel alteraImovel)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var imovelAlterado = await connection.QuerySingleOrDefaultAsync<AlteraImovel>("usp_Imoveis_Update",
                        param: alteraImovel, commandType: CommandType.StoredProcedure);
                    return imovelAlterado;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return null; ;
            }
        }

        public async Task ApagaImovel(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Imoveis_Delete",
                    param: new { Id = id }, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }
        public async Task<IEnumerable<ImovelVM>> GetAll()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var imoveis = await connection.QueryAsync<ImovelVM>("usp_Imoveis_GetAll",
                    commandType: CommandType.StoredProcedure);
                    return imoveis.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Enumerable.Empty<ImovelVM>();
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetPropertiesAsLookupTables()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var propertiesLookup = await connection.QueryAsync<LookupTableVM>("usp_Imoveis_GetImoveisAsLookupTable",
                    commandType: CommandType.StoredProcedure);
                    return propertiesLookup.ToList();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return Enumerable.Empty<LookupTableVM>();
            }
        }


        public async Task<ImovelVM> GetImovel_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<ImovelVM>("usp_Imoveis_GetById",
                    param: new { Id = id }, commandType: CommandType.StoredProcedure);

                    return output;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new ImovelVM();
            }
        }


        public async Task<int> GetCodigo_Imovel(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var output = await connection.QueryFirstOrDefaultAsync<int>("usp_Imoveis_GetCodigoImovel",
                param: new { Id = id }, commandType: CommandType.StoredProcedure);

                return output;
            }
        }

        public async Task<string> GetDescricao_Imovel(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                var output = await connection.QueryFirstOrDefaultAsync<string>("usp_Imoveis_GetDescricaoImovel",
                param: new { Id = id }, commandType: CommandType.StoredProcedure);

                return output;
            }
        }

        public async Task<string> GetNumeroPorta(int id)
        {

            using (var connection = _context.CreateConnection())
            {
                var output = await connection.QueryFirstOrDefaultAsync<string>("usp_Imoveis_GetNumeroPorta",
                param: new { Id = id }, commandType: CommandType.StoredProcedure);

                return output;
            }
        }

        public async Task<bool> CanPropertyBeDeleted(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                using (var connection = _context.CreateConnection())
                {
                    var result = await connection.QuerySingleOrDefaultAsync<int>("usp_Imoveis_CheckDeleteConstraint",
                    param: parameters, commandType: CommandType.StoredProcedure);
                    return result == 0;
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
            sb.Append("FROM Imoveis");

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QuerySingleOrDefaultAsync<int>(sb.ToString());
                return result > 0;
            }

        }
    }
}
