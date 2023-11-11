using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Contactos;
using PropertyManagerFL.Infrastructure.Context;

using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly IDapperContext _context;
        private readonly ILogger<ContactRepository> _logger;

        public ContactRepository(DapperContext context, ILogger<ContactRepository> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<int> InsereContacto(NovoContacto novoContacto)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    int insertedId = await connection.QueryFirstAsync<int>("usp_Contactos_Insert",
                         param: novoContacto, commandType: CommandType.StoredProcedure);

                    return insertedId;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return -1;
            }
        }

        public async Task<ContactoVM?> AtualizaContacto(AlteraContacto alteraContacto)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var contacto = await connection.QueryFirstAsync<ContactoVM>("usp_Contactos_Update",
                         param: alteraContacto, commandType: CommandType.StoredProcedure);

                    return contacto;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task ApagaContacto(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Contactos_Delete",
                    param: parameters, commandType: CommandType.StoredProcedure);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }


        public async Task<IEnumerable<ContactoVM>> GetAll()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryAsync<ContactoVM>("usp_Contactos_GetAll",
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

        public async Task<ContactoVM> GetContacto_ById(int id)
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    var output = await connection.QueryFirstOrDefaultAsync<ContactoVM>("usp_Contactos_GetById",
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
