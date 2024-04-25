using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels;
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
    public async Task<IEnumerable<DistritoConcelho>> GetConcelhos()
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryAsync<DistritoConcelho>("usp_GetConcelhos", commandType: CommandType.StoredProcedure);
                return result;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return Enumerable.Empty<DistritoConcelho>();
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

    public async Task<Concelho> GetConcelho_ById(int Id)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.QueryFirstOrDefaultAsync<Concelho>("usp_Concelho_GetById",
                    new { Id },
                    commandType: CommandType.StoredProcedure);
                return result;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return new();
        }
    }

    public async Task UpdateCoeficienteIMI(int Id, decimal coeficienteIMI)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync("usp_CoeficienteIMI_Update",
                    new { IdConcelho = Id,  CoeficienteIMI = coeficienteIMI},
                    commandType: CommandType.StoredProcedure);
                return;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return;
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
