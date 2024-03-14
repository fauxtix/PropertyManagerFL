using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Logs;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppLogController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AppLogController> _logger;

        private readonly ILogRepository _logRepository;

        public AppLogController(ILogRepository logRepository, IMapper mapper, ILogger<AppLogController> logger)
        {
            _logRepository = logRepository;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AppLogs()
        {
            var location = GetControllerActionNames();
            try
            {
                IEnumerable<AppLog> DBLogs = await _logRepository.GetAppLogs();
                if (DBLogs.Count() > 0)
                {
                    var clientLogs = _mapper.Map<IEnumerable<AppLogDto>>(DBLogs);
                    return Ok(clientLogs);
                }
                else
                {
                    _logger.LogWarning("Api / Logs - Sem registos para apresentar");
                    return NotFound();
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

        public async Task<IActionResult> GetAppLog_ById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                AppLog DBLog = await _logRepository.GetAppLog_ById(id);
                if (DBLog is not null)
                {
                    var clientLogs = _mapper.Map<AppLogDto>(DBLog);
                    return Ok(clientLogs);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet("Logins")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetAppLogins()
        {
            var location = GetControllerActionNames();
            try
            {
                var logins = await _logRepository.FilterLogins();
                if (logins is not null)
                {
                    if (logins.Any())
                    {
                        var clientLogins = _mapper.Map<IEnumerable<AppLogDto>>(logins);
                        return Ok(clientLogins);
                    }
                    else { return NotFound(); }
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteAllLogs()
        {
            var location = GetControllerActionNames();
            try
            {
                await _logRepository.DeleteAll();
                return Ok();

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        [HttpDelete("ById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                await _logRepository.DeleteById(id);
                return Ok();

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
}
