using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Appointments;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AppointmentsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentsController> _logger;

    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentsController(IAppointmentRepository appointmentRepository, IMapper mapper, ILogger<AppointmentsController> logger)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost]
    public async Task<int> CreateAppointment([FromBody] Appointment appointment)
    {
        var location = GetControllerActionNames();
        try
        {
            var createdId = await _appointmentRepository.InsertAsync(appointment);
            var createdAppointment = await _appointmentRepository.GetById_Async(createdId);
            var result = CreatedAtAction(nameof(GetAppointment_ById), new { Id = createdId }, createdAppointment);
            return createdId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return -1;
        }
    }

    [HttpPut]
    public async Task UpdateAppointment([FromBody] AppointmentVM appointmentToUpdate)
    {
        var location = GetControllerActionNames();
        try
        {
            if (appointmentToUpdate == null)
                _logger.LogWarning("Appointments - Parâmetros inválidos. Verifique, p.f.");

            if (_appointmentRepository.GetById_Async(appointmentToUpdate!.Id) == null)
            {
                _logger.LogWarning("Event not found. Check log, please");
            }

            var appointment = _mapper.Map<Appointment>(appointmentToUpdate);
            await _appointmentRepository.UpdateAsync(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{location}: {ex.Message} - {ex.InnerException}");
        }
    }

    [HttpDelete("{Id:int}")]

    public async Task Delete(int Id)
    {
        var location = GetControllerActionNames();
        try
        {
            if (_appointmentRepository.GetById_Async(Id) == null)
            {
                _logger.LogWarning("Evento não encontrado. Verifique, p.f.");
            }

            await _appointmentRepository.DeleteAsync(Id);

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
        }
    }

    [HttpGet]

    public async Task<IEnumerable<AppointmentVM>> GetAppointments()
    {
        var location = GetControllerActionNames();
        try
        {
            IEnumerable<Appointment> appointments = await _appointmentRepository.GetAllAsync();
            if (appointments.Count() > 0)
            {
                var clientAppointments = _mapper.Map<IEnumerable<AppointmentVM>>(appointments);
                return clientAppointments;
            }
            else
            {
                _logger.LogWarning("Api / Appointments - Sem registos para apresentar");
                return Enumerable.Empty<AppointmentVM>();
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            return Enumerable.Empty<AppointmentVM>();
        }
    }


    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetAppointment_ById(int id)
    {
        var location = GetControllerActionNames();
        try
        {
            Appointment appointment = await _appointmentRepository.GetById_Async(id);
            if (appointment is not null)
            {
                var clientAppointment = _mapper.Map<AppointmentVM>(appointment);
                return Ok(clientAppointment);
            }
            else
            {
                return NotFound("Não encontrado");
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }

    private string GetControllerActionNames()
    {
        var controller = ControllerContext.ActionDescriptor.ControllerName;
        var action = ControllerContext.ActionDescriptor.ActionName;

        return $"{controller} - {action}";
    }

    private ObjectResult InternalError(string message)
    {
        _logger.LogWarning(message);
        return StatusCode(500, "Algo de errado ocorreu. Contacte o Administrador");
    }
}
