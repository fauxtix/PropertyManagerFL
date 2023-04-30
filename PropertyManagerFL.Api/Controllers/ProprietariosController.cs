using Serilog;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Proprietarios;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProprietariosController : ControllerBase
    {
        private readonly IProprietarioRepository _repoProprietarios;
        private readonly IMapper _mapper;
        private readonly ILogger<ProprietariosController> _logger;

        public ProprietariosController(IProprietarioRepository repoProprietarios, IMapper mapper, ILogger<ProprietariosController> logger)
        {
            _repoProprietarios = repoProprietarios;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("GetProprietario_ById/{id:int}")]
        public async Task<IActionResult> GetProprietario_ById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var landlord = await _repoProprietarios.GetProprietario_ById(id);
                if (landlord is not null)
                    return Ok(landlord);
                else
                    return null;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetFirstId")]
        public IActionResult GetFirstId()
        {
            var location = GetControllerActionNames();
            try
            {
                var landlordId = _repoProprietarios.GetFirstId();
                if (landlordId > 0)
                    return Ok(landlordId);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("TableHasData")]
        public async Task<IActionResult> Landlord_Created()
        {
            var location = GetControllerActionNames();
            try
            {
                var landlordCreated = await _repoProprietarios.TableHasData();
                    return Ok(landlordCreated);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpPost("InsereProprietario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereContacto([FromBody] NovoProprietario novoProprietario)
        {
            var location = GetControllerActionNames();
            try
            {
                if (novoProprietario is null)
                {
                    return BadRequest();
                }
                int insertedId = await _repoProprietarios.Insert(novoProprietario);
                var viewLandlord = await _repoProprietarios.GetProprietario_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetProprietario_ById), new { id = viewLandlord.Id }, viewLandlord);

                _logger.LogInformation("Criado Proprietário (API)");

                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpPut("AlteraProprietario/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AlteraProprietario(int id, [FromBody] AlteraProprietario alteraProprietario)
        {
            var location = GetControllerActionNames();
            try
            {
                if (alteraProprietario == null)
                {
                    string msg = "O Proprietário passado como paràmetro é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != alteraProprietario.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var landlordToUpdate = await _repoProprietarios.GetProprietario_ById(id);

                if (landlordToUpdate == null)
                {
                    return NotFound($"O Proprietário com o Id {id} não foi encontrado");
                }

                var updatedLandlord = await _repoProprietarios.Update(alteraProprietario);
                if (!updatedLandlord)
                    return BadRequest("Proprietário not updated");

                _logger.LogInformation("Proprietário atualizado (API)");

                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpDelete("ApagaProprietario/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaProprietario(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var contactToDelete = await _repoProprietarios.Query_ById(id);
                if (contactToDelete == null)
                {
                    _logger.LogError($"Contacto com o Id {id} não encontrado");
                    return NotFound($"Contacto com o Id {id} não encontrado");
                }

                await _repoProprietarios.Delete(id);
                _logger.LogInformation("Proprietário apagado (API)");

                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
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
            _logger.LogError(message);
            return StatusCode(500, $"Algo de errado ocorreu ({message}). Contacte o Administrador");
        }
    }

}

