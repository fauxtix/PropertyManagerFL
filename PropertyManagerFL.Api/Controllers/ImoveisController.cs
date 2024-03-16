using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Imoveis;

namespace PropertyManagerFL.Api.Controllers
{
    /// <summary>
    /// Controller dos imóveis
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ImoveisController : ControllerBase
    {
        private readonly IImovelRepository _repoImoveis;
        private readonly IMapper _mapper;

        private readonly ILogger<ImoveisController> _logger;

        /// <summary>
        /// Controller de imóveis
        /// </summary>
        /// <param name="repoImoveis"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
		public ImoveisController(IImovelRepository repoImoveis, ILogger<ImoveisController> logger, IMapper mapper)
        {
            _repoImoveis = repoImoveis;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        ///  Get imóveis
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetImoveis")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetImoveis()
        {
            var location = GetControllerActionNames();
            try
            {
                var imoveis = await _repoImoveis.GetAll();
                if (imoveis is not null)
                {
                    return Ok(imoveis);
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
        /// Get properties to fill combo
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet("GetPropertiesLookup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetPropertiesLookup()
        {
            var location = GetControllerActionNames();
            try
            {
                var propertiesLookup = await _repoImoveis.GetPropertiesAsLookupTables();
                if (propertiesLookup.Any())
                {
                    return Ok(propertiesLookup);
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
        /// Get property by Id
        /// </summary>
        /// <param name="id">Id do imóvel</param>
        /// <returns></returns>
        [HttpGet("GetImovelById/{id:int}")]
        public async Task<IActionResult> GetImovelById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var imovel = await _repoImoveis.GetImovel_ById(id);
                if (imovel is not null)
                    return Ok(imovel);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get código do imóvel (na fração) - deveria estar no controller de frações?
        /// </summary>
        /// <param name="id">Id da fração</param>
        /// <returns></returns>
        [HttpGet("GetCodigo_Imovel/{id:int}"), Produces("text/plain")]
        public async Task<IActionResult> GetCodigo_Imovel(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var idImovel = await _repoImoveis.GetCodigo_Imovel(id);
                if (idImovel > 0)
                    return Ok(idImovel);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("TableHasData")]
        public async Task<IActionResult> TableHasData()
        {
            var location = GetControllerActionNames();
            try
            {
                var hasRecords = await _repoImoveis.TableHasData();
                    return Ok(hasRecords);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Get descrição do imóvel - deveria estar no controller de frações?
        /// </summary>
        /// <param name="id">Id da fração</param>
        /// <returns></returns>
        [HttpGet("GetDescricao_Imovel/{id:int}"), Produces("text/plain")]
        public async Task<IActionResult> GetDescricao_Imovel(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var descricaoImovel = await _repoImoveis.GetDescricao_Imovel(id);
                if (!string.IsNullOrEmpty(descricaoImovel))
                    return Ok(descricaoImovel);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get Nº de porta do imóvel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("GetNumeroPorta/{id:int}")]
        public async Task<IActionResult> GetNumeroPorta(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var numeroPorta = await _repoImoveis.GetNumeroPorta(id);
                if (!string.IsNullOrEmpty(numeroPorta))
                    return Ok(numeroPorta);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Cria novo imóvel
        /// </summary>
        /// <param name="novoImovel"></param>
        /// <returns></returns>
        [HttpPost("InsereImovel")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereImovel([FromBody] NovoImovel novoImovel)
        {
            var location = GetControllerActionNames();
            try
            {
                if (novoImovel is null)
                {
                    return BadRequest();
                }

                var insertedId = await _repoImoveis.InsereImovel(novoImovel);
                var insertedProperty = await _repoImoveis.GetImovel_ById(insertedId);

                var actionReturned = CreatedAtAction(nameof(GetImovelById), new { id = insertedProperty.Id }, insertedProperty);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }

        }

        /// <summary>
        /// Altera dados do imóvel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="alteraImovel"></param>
        /// <returns></returns>
        [HttpPut("AlteraImovel/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AlteraImovel(int id, [FromBody] AlteraImovel alteraImovel)
        {
            var location = GetControllerActionNames();
            try
            {
                if (alteraImovel == null)
                {
                    string msg = "O Imóvel passado como paràmetro, é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != alteraImovel.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                if (await _repoImoveis.GetImovel_ById(id) == null)
                {
                    return NotFound($"O Imóvel com o Id {id} não foi encontrado");
                }

                var imovelAlterado = await _repoImoveis.AtualizaImovel(alteraImovel);
                return Ok(_mapper.Map<ImovelVM>(imovelAlterado));
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Apaga imóvel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("ApagaImovel/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaImovel(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var imovelToDelete = await _repoImoveis.GetImovel_ById(id);
                if (imovelToDelete == null)
                {
                    _logger.LogError($"Imóvel com o Id {id} não encontrado");
                    return NotFound($"Imóvel com o Id {id} não encontrado");
                }

                var okToDelete = await _repoImoveis.CanPropertyBeDeleted(id);
                if (okToDelete)
                {
                    await _repoImoveis.ApagaImovel(id);
                    return NoContent();
                }
                else
                    return BadRequest("Imóvel com frações. Pedido negado");

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
