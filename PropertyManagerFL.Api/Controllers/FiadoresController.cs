using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Fiadores;

namespace PropertyManagerFL.Api.Controllers
{
    /// <summary>
    /// Tenants Api
    /// </summary>
    /// 
    [Route("api/[controller]")]
    [ApiController]
    public class FiadoresController : ControllerBase
    {
        private readonly IFiadorRepository _repoFiadores;
        private readonly IMapper _mapper;
        private readonly ILogger<FiadoresController> _logger;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Controller de Fiadores
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="repoFiadores"></param>
        /// <param name="environment"></param>
        public FiadoresController(IMapper mapper, ILogger<FiadoresController> logger, IFiadorRepository repoFiadores, IWebHostEnvironment environment)
        {
            _mapper = mapper;
            this._logger = logger;
            _repoFiadores = repoFiadores;
            _environment = environment;
        }

        /// <summary>
        /// Returns all database tenants
        /// </summary>
        /// <returns>IEnumerable Fiador</returns>
        // GET: api/<FiadoresController>
        [HttpGet("GetFiadores/{titular:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetFiadores(int titular = 1)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenants = await _repoFiadores.GetAll();
                if (tenants.Any())
                {
                    return Ok(tenants);
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
        /// Get tenant by id and ownership
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetFiadorById/{id:int}")]
        public async Task<IActionResult> GetFiadorById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenant = await _repoFiadores.GetFiador_ById(id);
                if (tenant is not null)
                    return Ok(tenant);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get nome do Fiador
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetNomeFiador/{id:int}")]
        public async Task<IActionResult> GetNomeFiador(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenantName = await _repoFiadores.GetNomeFiador(id);
                if (!string.IsNullOrEmpty(tenantName))
                    return Ok(tenantName);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetFiador_Inquilino/{id:int}")]
        public async Task<IActionResult> GetFiador_Inquilino(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var fiadorInquilino = await _repoFiadores.GetFiadorInquilino(id);
                if (fiadorInquilino is not null)
                    return Ok(fiadorInquilino);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Get Fiadores disponíveis
        /// </summary>
        /// <param name="titular">true/false</param>
        /// <returns></returns>
        [HttpGet("GetFiadoresDisponiveis")]
        public async Task<IActionResult> GetFiadoresDisponiveis()
        {
            var location = GetControllerActionNames();
            try
            {
                var FiadoresDisponiveis = await _repoFiadores.GetFiadoresDisponiveis();
                if (FiadoresDisponiveis.Any())
                    return Ok(FiadoresDisponiveis);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get Fiadores / fiadores
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFiadores_ForLookup")]
        public async Task<IActionResult> GetFiadores_ForLookup()
        {
            var location = GetControllerActionNames();
            try
            {
                var Fiadores_fiadores = await _repoFiadores.GetFiadores_ForLookUp();
                if (Fiadores_fiadores.Any())
                    return Ok(Fiadores_fiadores);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get primeiro Id do Fiador
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFirstIdFiador")]
        public IActionResult GetFirstIdFiador()
        {
            var location = GetControllerActionNames();
            try
            {
                var idFiador = _repoFiadores.GetFirstIdFiador();
                if (idFiador > 0)
                    return Ok(idFiador);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get primeiro Id do Fiador
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFirstId_Fiador")]
        public IActionResult GetFirstId_Fiador()
        {
            var location = GetControllerActionNames();
            try
            {
                var idFiador = _repoFiadores.GetFirstId_Fiador();
                if (idFiador > 0)
                    return Ok(idFiador);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Insert new fiador
        /// </summary>
        /// <param name="novoFiador"></param>
        /// <returns>url to access the tenant created</returns>
        [HttpPost("InsereFiador")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereFiador([FromBody] NovoFiador novoFiador)
        {
            var location = GetControllerActionNames();
            try
            {
                if (novoFiador is null)
                {
                    return BadRequest();
                }
                var insertedId = await _repoFiadores.InsereFiador(novoFiador);
                var viewFiador = _repoFiadores.GetFiador_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetFiadorById), new { id = viewFiador.Id }, viewFiador);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }

        }

        /// <summary>
        /// Update tenant record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alteraFiadorDto"></param>
        /// <returns></returns>


        [HttpPut("AlteraFiador/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AlteraFiador(int id, [FromBody] AlteraFiador alteraFiadorDto)
        {
            var location = GetControllerActionNames();
            try
            {
                if (alteraFiadorDto == null)
                {
                    string msg = "O Fiador passado como paràmetro, é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != alteraFiadorDto.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var tenantToUpdate = await _repoFiadores.GetFiador_ById(id);

                if (tenantToUpdate == null)
                {
                    return NotFound($"O Fiador com o Id {id} não foi encontrado");
                }

                var FiadorAlterado = await _repoFiadores.AtualizaFiador(alteraFiadorDto);
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Delete tenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [HttpDelete("ApagaFiador/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaFiador(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var tenantToDelete = await _repoFiadores.GetFiador_ById(id);
                if (tenantToDelete == null)
                {
                    _logger.LogError($"Fiador com o Id {id} não encontrado");
                    return NotFound($"Fiador com o Id {id} não encontrado");
                }

                var okToDelete = await _repoFiadores.CanFiadorBeDeleted(id);

                // Comentado código abaixo em 14/03/2024; os dados do fiador só são usados na redação do contrato.
                // Como pode haver a possibilidade de contratos sem fiador (!), não faz sentido a validação

                //if (!okToDelete)
                //{
                //    _logger.LogError($"Fiador com o Id {id} tem contratos associados!");
                //    return BadRequest($"Fiador com o Id {id} tem contratos associados!");
                //}

                await _repoFiadores.ApagaFiador(id);
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
            return StatusCode(500, "Algo de errado ocorreu. Contacte o Administrador");
        }
    }
}
