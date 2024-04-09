using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories;
public class AppointmentRepository : IAppointmentRepository
{
    private readonly IDapperContext _context;
    private readonly ILogger<AppointmentRepository> _logger;

    public AppointmentRepository(IDapperContext context, ILogger<AppointmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> InsertAsync(Appointment appointment)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                int insertedId = await connection.QueryFirstAsync<int>("usp_Appointments_Insert",
                     param: appointment, commandType: CommandType.StoredProcedure);

                return insertedId;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return -1;
        }
    }

    public async Task<bool> UpdateAsync(Appointment appointment)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("usp_Appointments_Update",
                     param: appointment, commandType: CommandType.StoredProcedure);

                return true;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return false;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync("usp_Appointments_Delete",
                param: parameters, commandType: CommandType.StoredProcedure);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var output = await connection.QueryAsync<Appointment>("usp_Appointments_GetAll",
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

    public async Task<Appointment> GetById_Async(int id)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var output = await connection.QueryFirstOrDefaultAsync<Appointment>("usp_Appointments_GetById",
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
