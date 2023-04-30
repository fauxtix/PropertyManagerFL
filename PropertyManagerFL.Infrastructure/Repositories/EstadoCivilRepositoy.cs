using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Infrastructure.Context;
using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories
{
    public class EstadoCivilRepository : IEstadoCivil
    {
        private readonly DapperContext _context;
        private readonly ILogger<EstadoCivilRepository> _logger;

        public EstadoCivilRepository(DapperContext context, ILogger<EstadoCivilRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<EstadoCivil> GetAllEstadoCivil()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    return connection.Query<EstadoCivil>("usp_GetAll",
                        CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        public EstadoCivil GetEstadoCivil(int Id)
        {
            try
            {
                DynamicParameters paramCollection = new DynamicParameters();
                paramCollection.Add("@Id", Id);
                using (var connection = _context.CreateConnection())
                {
                    return connection.QueryFirstOrDefault<EstadoCivil>("usp_FormaPagamento_GetById", param: paramCollection,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        public int GetId_EstadoCivil(string estadoCivil)
        {
            try
            {
                DynamicParameters paramCollection = new DynamicParameters();
                paramCollection.Add("@EstadoCivil", estadoCivil);
                using (var connection = _context.CreateConnection())
                {
                    return connection.QueryFirstOrDefault<int>("usp_GetId_EstadoCivil", param: paramCollection,
                        commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

    }
}
