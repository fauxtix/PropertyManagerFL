using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories;
public class HistoricoEnvioCartasRepository : IHistoricoEnvioCartasRepository
{
    private readonly IDapperContext _context;
    private readonly ILogger<HistoricoEnvioCartasRepository> _logger;


    public HistoricoEnvioCartasRepository(IDapperContext context, ILogger<HistoricoEnvioCartasRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> InsertLetterSent(HistoricoEnvioCartas letterSent)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@DataEnvio", letterSent.DataEnvio);
        parameters.Add("@IdInquilino", letterSent.IdInquilino);
        parameters.Add("@IdTipoCarta", letterSent.IdTipoCarta);
        parameters.Add("@Tentativa", letterSent.Tentativa);

        try
        {
            using (var connection = _context.CreateConnection())
            {
                int insertedId = await connection.QueryFirstAsync<int>("usp_Letters_Insert",
                     param: parameters, commandType: CommandType.StoredProcedure);
                return insertedId;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

    public async Task<bool> UpdateLetterAnsweredDate(int Id, DateTime answerDate)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", Id);
        parameters.Add("@DataResposta", answerDate);

        try
        {
            using (var connection = _context.CreateConnection())
            {
                int insertedId = await connection.QueryFirstAsync<int>("usp_Letters_Answer_Update",
                     param: parameters, commandType: CommandType.StoredProcedure);
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public async Task<IEnumerable<HistoricoEnvioCartas>> GetLettersSent()
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var lettersSent = await connection.QueryAsync<HistoricoEnvioCartas>("usp_Letters_GetAll",
                         commandType: CommandType.StoredProcedure);
                return lettersSent;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Enumerable.Empty<HistoricoEnvioCartas>();
        }
    }

    public async Task<HistoricoEnvioCartas> GetLetterSent(int Id)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var letterSent = await connection.QueryFirstOrDefaultAsync<HistoricoEnvioCartas>("usp_Letters_GetAnswerLetter",
                    new { Id },     commandType: CommandType.StoredProcedure);
                return letterSent;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return new();
        }
    }

}
