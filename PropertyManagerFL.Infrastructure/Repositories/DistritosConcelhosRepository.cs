using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;
using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories;
public class DistritosConcelhosRepository : IDistritosConcelhosRepository
{
    private readonly IDapperContext _context;
    private readonly ILogger<DistritosConcelhosRepository> _logger;


    public DistritosConcelhosRepository(IDapperContext context, ILogger<DistritosConcelhosRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<IEnumerable<Concelho>> GetConcelhos()
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Concelho>("usp_GetConcelhos", commandType: CommandType.StoredProcedure);
                return result;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return Enumerable.Empty<Concelho>();
        }
    }

    public async Task<IEnumerable<Concelho>> GetConcelhosByDistrito(int id)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<Concelho>("usp_GetConcelhosDeDistrito", 
                    new {IdDistrito = id},
                    commandType: CommandType.StoredProcedure);
                return result;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return Enumerable.Empty<Concelho>();
        }
    }

    public async Task<IEnumerable<LookupTableVM>> GetDistritos()
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<LookupTableVM>("usp_GetDistritos", commandType: CommandType.StoredProcedure);
                return result;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return Enumerable.Empty<LookupTableVM>();
        }
    }
}
