using Serilog;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Inquilinos;

namespace PropertyManagerFL.Api.Controllers
{
    /// <summary>
    /// Tenants Api
    /// </summary>
    /// 
    [Route("api/[controller]")]
    [ApiController]
    public class InquilinosController : ControllerBase
    {
        private readonly IInquilinoRepository _repoInquilinos;
        private readonly IMapper _mapper;
        private readonly ILogger<InquilinosController> _logger;
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Controller de Inquilinos
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="repoInquilinos"></param>
        /// <param name="environment"></param>
        public InquilinosController(IMapper mapper, ILogger<InquilinosController> logger, IInquilinoRepository repoInquilinos, IWebHostEnvironment environment)
        {
            _mapper = mapper;
            this._logger = logger;
            _repoInquilinos = repoInquilinos;
            _environment = environment;
        }

        /// <summary>
        /// Returns all database tenants
        /// </summary>
        /// <returns>IEnumerable Inquilino</returns>
        // GET: api/<InquilinosController>
        [HttpGet("GetInquilinos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetInquilinos()
        {
            _logger.LogInformation("Lendo todos os inquilinos do Api");
            var location = GetControllerActionNames();
            try
            {
                var tenants = await _repoInquilinos.GetAll();
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

        [HttpGet("TenantPaymentsHistory/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetTenantPaymentsHistory(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenantHistory = await _repoInquilinos.GetTenantPaymentsHistory(id);
                if (tenantHistory is not null && tenantHistory.Count() > 0)
                {
                   var tenantHistoryList =  _mapper.Map<IEnumerable<CC_InquilinoVM>>(tenantHistory);
                    return Ok(tenantHistoryList);
                }
                else
                {
                    return NoContent();
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
        [HttpGet("GetInquilinoById/{id:int}")]
        public async Task<IActionResult> GetInquilinoById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenant = await _repoInquilinos.GetInquilino_ById(id);
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
        /// Devolve fiador do inquilino
        /// </summary>
        /// <param name="id">Id do inquilino</param>
        /// <returns></returns>
        [HttpGet("GetFiadorInquilinoById/{id:int}")]
        public async Task<IActionResult> GetFiadorInquilinoById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var fiadores = await _repoInquilinos.GeFiadortInquilino_ById(id);
                if (fiadores is not null)
                    return Ok(fiadores);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Get nome do inquilino
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetNomeInquilino/{id:int}")]
        public async Task<IActionResult> GetNomeInquilino(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenantName = await _repoInquilinos.GetNomeInquilino(id);
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

        /// <summary>
        /// Get Inquilinos disponíveis
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInquilinosDisponiveis")]
        public async Task<IActionResult> GetInquilinosDisponiveis()
        {
            var location = GetControllerActionNames();
            try
            {
                var inquilinosDisponiveis = await _repoInquilinos.GetInquilinosDisponiveis();
                if (inquilinosDisponiveis.Any())
                    return Ok(inquilinosDisponiveis);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get Inquilinos sem contrato
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInquilinosSemContrato")]
        public async Task<IActionResult> GetInquilinosSemContrato()
        {
            var location = GetControllerActionNames();
            try
            {
                var inquilinosSemContrato = await _repoInquilinos.GetInquilinos_SemContrato();
                if (inquilinosSemContrato.Any())
                    return Ok(inquilinosSemContrato);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Get Inquilinos (to fill lookup combo)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetInquilinosAsLookup")]
        public async Task<IActionResult> GetInquilinosAsLookup()
        {
            var location = GetControllerActionNames();
            try
            {
                var inquilinos = await _repoInquilinos.GetInquilinosAsLookup();
                if (inquilinos.Any())
                    return Ok(inquilinos);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Get id do inquilino da fracao arrendada
        /// </summary>
        /// <param name="id">Id da fracão</param>
        /// <returns></returns>
        [HttpGet("GetInquilinoFracao/{id:int}")]
        public async Task<IActionResult> GetInquilinoFracao(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var IdInquilinoFracao = await _repoInquilinos.GetInquilinoFracao(id);
                if (IdInquilinoFracao > 0)
                    return Ok(IdInquilinoFracao);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get nome da fração
        /// </summary>
        /// <param name="id">id do inquilino</param>
        /// <param name="titular">1=inquilino, 0=fiador</param>
        /// <returns></returns>
        [HttpGet("GetNomeFracao/{id:int}/{titular}")]
        public IActionResult GetNomeFracao(int id, bool titular)
        {
            var location = GetControllerActionNames();
            try
            {
                var nomeFracao = _repoInquilinos.GetNomeFracao(id, titular);
                if (!string.IsNullOrEmpty(nomeFracao))
                    return Ok(nomeFracao);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get primeiro Id do inquilino
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetFirstIdInquilino")]
        public IActionResult GetFirstIdInquilino()
        {
            var location = GetControllerActionNames();
            try
            {
                var idInquilino = _repoInquilinos.GetFirstIdInquilino();
                if (idInquilino > 0)
                    return Ok(idInquilino);
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
        [HttpGet("GetFirstId_Inquilino")]
        public IActionResult GetFirstId_Inquilino()
        {
            var location = GetControllerActionNames();
            try
            {
                var idInquilino = _repoInquilinos.GetFirstId_Inquilino();
                if (idInquilino > 0)
                    return Ok(idInquilino);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Último mês pago pelo inquilino
        /// </summary>
        /// <param name="id">Id do inquilino</param>
        /// <returns></returns>
        [HttpGet("GetUltimoMesPago_Inquilino/{id:int}")]
        public IActionResult GetUltimoMesPago_Inquilino(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                string ultimoMesPagoPeloInquilino = _repoInquilinos.GetUltimoMesPago_Inquilino(id);
                if (!string.IsNullOrEmpty(ultimoMesPagoPeloInquilino))
                    return Ok(ultimoMesPagoPeloInquilino);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Insert new tenant
        /// </summary>
        /// <param name="novoInquilino"></param>
        /// <returns>url to access the tenant created</returns>
        [HttpPost("InsereInquilino")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereInquilino([FromBody] NovoInquilino novoInquilino)
        {
            var location = GetControllerActionNames();
            try
            {
                if (novoInquilino is null)
                {
                    return BadRequest();
                }
                var insertedId = await _repoInquilinos.InsereInquilino(novoInquilino);
                var viewInquilino = _repoInquilinos.GetInquilino_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetInquilinoById), new { id = viewInquilino.Id }, viewInquilino);
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
        /// <param name="alteraInquilinoDto"></param>
        /// <returns></returns>


        [HttpPut("AlteraInquilino/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AlteraInquilino(int id, [FromBody] AlteraInquilino alteraInquilinoDto)
        {
            var location = GetControllerActionNames();
            try
            {
                if (alteraInquilinoDto == null)
                {
                    string msg = $"{location}. O Inquilino passado como paràmetro, é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != alteraInquilinoDto.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var tenantToUpdate = await _repoInquilinos.GetInquilino_ById(id);

                if (tenantToUpdate == null)
                {
                    return NotFound($"O Inquilino com o Id {id} não foi encontrado");
                }

                var inquilinoAlterado = await _repoInquilinos.AtualizaInquilino(alteraInquilinoDto);
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpPut("AlteraDocumentoInquilino/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AlteraDocumentoInquilino(int id, [FromBody] AlteraDocumentoInquilino alteraDocumentoInquilino)
        {
            var location = GetControllerActionNames();
            try
            {
                if (alteraDocumentoInquilino == null)
                {
                    string msg = $"{location}. O documento passado como paràmetro, é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != alteraDocumentoInquilino.Id)
                {
                    string msg = $"O id ({id}) passado como paràmetro é incorreto";
                    _logger.LogWarning(msg);

                    return BadRequest(msg);
                }

                var documentToUpdate = await _repoInquilinos.GetDocumentoById(id);

                if (documentToUpdate == null)
                {
                    string msg = $"O documento com o Id {id} não foi encontrado";
                    _logger.LogWarning(msg);

                    return NotFound(msg);
                }

                var documentoInquilinoAlterado = await _repoInquilinos.AtualizaDocumentoInquilino(alteraDocumentoInquilino);
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Atualiza saldo do inquilino
        /// </summary>
        /// <param name="id">Id do inquilino</param>
        /// <param name="saldoCorrente">Verificar se saldo passado como decimal é processado corretamente!</param>
        /// <returns></returns>
        [HttpGet("AtualizaSaldo/{id:int}/{saldoCorrente}")]
        public async Task<IActionResult> AtualizaSaldo(int id, decimal saldoCorrente)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest("Id do inquilino não é válido");
                }

                var tenantToUpdate = await _repoInquilinos.GetInquilino_ById(id);

                if (tenantToUpdate == null)
                {
                    return NotFound($"O Inquilino com o Id {id} não foi encontrado");
                }

                await _repoInquilinos.AtualizaSaldo(id, saldoCorrente);
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
        [HttpDelete("ApagaInquilino/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaInquilino(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var tenantToDelete = await _repoInquilinos.GetInquilino_ById(id);
                if (tenantToDelete == null)
                {
                    _logger.LogWarning($"Inquilino com o Id {id} não encontrado");
                    return NotFound($"Inquilino com o Id {id} não encontrado");
                }

                var okToDelete = await _repoInquilinos.CanInquilinoBeDeleted(id);
                if (!okToDelete)
                {
                    _logger.LogWarning($"Inquilino com o Id {id} tem contratos associados!");
                    return BadRequest($"Inquilino com o Id {id} tem contratos associados!");
                }

                await _repoInquilinos.ApagaInquilino(id);
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpPost("InsereDocumento")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereDocumento([FromBody] NovoDocumentoInquilino novoDocumento)
        {
            var location = GetControllerActionNames();
            try
            {
                if (novoDocumento is null)
                {
                    return BadRequest();
                }
                var insertedId = await _repoInquilinos.CriaDocumentoInquilino(novoDocumento);
                var viewDocument = _repoInquilinos.GetDocumentoById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetInquilinoById), new { id = viewDocument.Id }, viewDocument);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpDelete("ApagaDocumentoInquilino/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaDocumentoInquilino(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var tenantDocumentToDelete = await _repoInquilinos.GetDocumentoById(id);
                if (tenantDocumentToDelete == null)
                {
                    _logger.LogWarning($"Documento do Inquilino com o Id {id} não encontrado");
                    return NotFound($"Documento do Inquilino com o Id {id} não encontrado");
                }

                await _repoInquilinos.ApagaDocumentoInquilino(id);
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }


        [HttpGet("GetDocumentoInquilinoById/{id:int}")]
        public async Task<IActionResult> GetDocumentoInquilinoById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var document = await _repoInquilinos.GetDocumentoById(id);
                if (document is not null)
                    return Ok(document);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        // GET: api/<InquilinosController>
        [HttpGet("GetDocumentosInquilinos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetDocumentosInquilinos()
        {
            var location = GetControllerActionNames();
            try
            {
                var documents = await _repoInquilinos.GetDocumentos();
                if (documents.Any())
                {
                    return Ok(documents);
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

        [HttpGet("GetDocumentosInquilino/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetDocumentosInquilino(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var documents = await _repoInquilinos.GetDocumentosInquilino(id);
                if (documents.Any())
                {
                    return Ok(documents);
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
