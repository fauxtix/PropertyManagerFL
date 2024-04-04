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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAppointment([FromBody] Appointment appointment)
    {
        var location = GetControllerActionNames();
        try
        {
            var appointmentToInsert = _mapper.Map<Appointment>(appointment);
            var createdId = await _appointmentRepository.InsertAsync(appointmentToInsert);
            var createdAppointment = await _appointmentRepository.GetById_Async(createdId);
            var result = CreatedAtAction(nameof(GetAppointment_ById), new { Id = createdId }, createdAppointment);
            return Ok(createdId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
            return BadRequest(ex);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentVM appointmentVM)
    {
        var location = GetControllerActionNames();
        try
        {
            if (appointmentVM == null || id != appointmentVM.Id)
                return BadRequest("Parâmetros inválidos. Verifique, p.f.");

            if (_appointmentRepository.GetById_Async(id) == null)
            {
                return NotFound("Não encontrado");
            }

            var appointmentToUpdate = _mapper.Map<Appointment>(appointmentVM);

            await _appointmentRepository.UpdateAsync(appointmentToUpdate);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> DeleteById(int id)
    {
        var location = GetControllerActionNames();
        try
        {
            if (_appointmentRepository.GetById_Async(id) == null)
            {
                return NotFound("Não encontrado");
            }

            await _appointmentRepository.DeleteAsync(id);
            return Ok();

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetAppointments()
    {
        var location = GetControllerActionNames();
        try
        {
            IEnumerable<Appointment> appointments = await _appointmentRepository.GetAllAsync();
            if (appointments.Count() > 0)
            {
                var clientAppointments = _mapper.Map<IEnumerable<AppointmentVM>>(appointments);
                return Ok(clientAppointments);
            }
            else
            {
                _logger.LogWarning("Api / Appointments - Sem registos para apresentar");
                return NotFound("Sem registos para apresentar");
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
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
