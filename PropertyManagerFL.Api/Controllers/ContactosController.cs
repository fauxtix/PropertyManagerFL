using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Contactos;

namespace PropertyManagerFL.Api.Controllers
{
    /// <summary>
    /// Api de Contactos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ContactosController : ControllerBase
    {
        private readonly IContactRepository _repoContactos;
        private readonly IMapper _mapper;
        private readonly ILogger<ContactosController> _logger;

        /// <summary>
        /// Construtor do api Contactos
        /// </summary>

        public ContactosController(IContactRepository repoContactos, IMapper mapper, ILogger<ContactosController> logger)
        {
            _repoContactos = repoContactos;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Todos os contactos
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetContactos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetContactos()
        {
            var location = GetControllerActionNames();
            try
            {
                var contacts = await _repoContactos.GetAll();
                if (contacts is not null)
                {
                    return Ok(contacts);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Pesquisa contacto por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("GetContactoById/{id:int}")]
        public async Task<IActionResult> GetContactoById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var contact = await _repoContactos.GetContacto_ById(id);
                if (contact is not null)
                    return Ok(contact);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Novo Contracto
        /// </summary>
        /// <param name="novoContacto"></param>
        /// <returns></returns>
        [HttpPost("InsereContacto")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereContacto([FromBody] NovoContacto novoContacto)
        {
            var location = GetControllerActionNames();
            try
            {
                if (novoContacto is null)
                {
                    return BadRequest();
                }
                int insertedId = await _repoContactos.InsereContacto(novoContacto);
                var viewContacto = await _repoContactos.GetContacto_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetContactoById), new { id = viewContacto.Id }, viewContacto);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Atualização de contacto
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alteraContacto"></param>
        /// <returns></returns>
        [HttpPut("AlteraContacto/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AlteraContacto(int id, [FromBody] AlteraContacto alteraContacto)
        {
            var location = GetControllerActionNames();
            try
            {
                if (alteraContacto == null)
                {
                    string msg = "O Contacto passado como paràmetro é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != alteraContacto.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var contactToUpdate = await _repoContactos.GetContacto_ById(id);

                if (contactToUpdate == null)
                {
                    return NotFound($"O Contacto com o Id {id} não foi encontrado");
                }

                var updatedContact = await _repoContactos.AtualizaContacto(alteraContacto);
                if (updatedContact is null)
                    return BadRequest("Contact not updated");

                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Apaga contacto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("ApagaContacto/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaContacto(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var contactToDelete = await _repoContactos.GetContacto_ById(id);
                if (contactToDelete == null)
                {
                    _logger.LogError($"Contacto com o Id {id} não encontrado");
                    return NotFound($"Contacto com o Id {id} não encontrado");
                }

                await _repoContactos.ApagaContacto(id);
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
