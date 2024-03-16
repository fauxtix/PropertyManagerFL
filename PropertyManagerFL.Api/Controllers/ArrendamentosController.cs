using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Api.Controllers
{
    /// <summary>
    /// Leases controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArrendamentosController : ControllerBase
    {
        private readonly IArrendamentoRepository _repoArrendamentos;
        private readonly IInquilinoRepository _repoInquilinos;
        private readonly IMapper _mapper;
        private readonly ILogger<ArrendamentosController> _logger;
        private readonly IWebHostEnvironment _environment;


        /// <summary>
        /// Leases controller constructor
        /// </summary>
        /// <param name="repoArrendamentos"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        /// <param name="repoInquilinos"></param>
        /// <param name="environment"></param>
        public ArrendamentosController(IArrendamentoRepository repoArrendamentos,
                                       IMapper mapper,
                                       ILogger<ArrendamentosController> logger,
                                       IInquilinoRepository repoInquilinos,
                                       IWebHostEnvironment environment)
        {
            _repoArrendamentos = repoArrendamentos;
            _mapper = mapper;
            _logger = logger;
            _repoInquilinos = repoInquilinos;
            _environment = environment;
        }

        [HttpPost("InsertArrendamento")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertArrendamento([FromBody] NovoArrendamento novoArrendamento)
        {
            var location = GetControllerActionNames();
            try
            {
                if (novoArrendamento is null)
                {
                    return BadRequest();
                }

                int insertedId = await _repoArrendamentos.InsertArrendamento(novoArrendamento);
                if (insertedId == -1)
                {
                    _logger.LogError("Erro na transação (Lease Insert. Verifique log, p.f.");
                    return InternalError($"{location}: Erro na transação. Verifique log, p.f.");
                }

                var viewLease = await _repoArrendamentos.GetArrendamento_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetArrendamento_ById), new { id = viewLease.Id }, viewLease);

                return actionReturned;
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }

        }

        [HttpPut("UpdateArrendamento/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateArrendamento(int id, [FromBody] AlteraArrendamento alteraArrendamento)
        {
            var location = GetControllerActionNames();
            try
            {
                if (alteraArrendamento == null)
                {
                    string msg = "O Arrendamento passado como paràmetro é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != alteraArrendamento.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var leaseToUpdate = await _repoArrendamentos.GetArrendamento_ById(id);

                if (leaseToUpdate == null)
                {
                    return NotFound($"O Arrendamento com o Id {id} não foi encontrado");
                }

                var updateOk = await _repoArrendamentos.UpdateArrendamento(alteraArrendamento);
                if (!updateOk)
                {
                    return BadRequest("Lease not updated");
                }

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpDelete("DeleteArrendamento/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteArrendamento(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete 'Lease' failed with bad data - id: {id}");
                    return BadRequest();
                }

                var leaseToDelete = await _repoArrendamentos.GetArrendamento_ById(id);
                if (leaseToDelete == null)
                {
                    _logger.LogError($"Arrendamento com o Id {id} não encontrado");
                    return NotFound($"Arrendamento com o Id {id} não encontrado");
                }

                await _repoArrendamentos.DeleteArrendamento(id);
                _logger.LogInformation("Arrendamento apagado com sucesso");

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError($"{location}: {ex.Message} - {ex.InnerException}");
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        /// <summary>
        /// Get all leases
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var location = GetControllerActionNames();
            try
            {
                var leases = await _repoArrendamentos.GetAll();
                if (leases == null)
                    return NotFound("Tabela de arrendamentos não devolveu qualquer registo");

                return Ok(leases);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("RequirementsMet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequirementsMet()
        {
            var location = GetControllerActionNames();
            try
            {
                var okToLease = await _repoArrendamentos.RequirementsMet();
                return Ok(okToLease);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Get lease by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("GetArrendamento_ById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetArrendamento_ById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var lease = await _repoArrendamentos.GetArrendamento_ById(id);
                if (lease is not null)
                {
                    return Ok(lease);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get all leases (redundant?)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetResumedData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public async Task<IActionResult> GetResumedData()
        {
            var location = GetControllerActionNames();
            try
            {
                var leases = await _repoArrendamentos.GetResumedData();
                if (leases.Any())
                {
                    return Ok(leases);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get tenant name of the leasing (redundant?)
        /// </summary>
        /// <param name="id">Lease tenantd id</param>
        /// <returns></returns>
        [HttpGet("GetNomeInquilino/{id:int}"), Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetNomeInquilino(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var leaseTenantName = await _repoInquilinos.GetNomeInquilino(id);
                if (!string.IsNullOrEmpty(leaseTenantName))
                {
                    return Ok(leaseTenantName);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetIdInquilino/{unitId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetIdInquilino(int unitId)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenantId = await _repoInquilinos.GetInquilinoFracao(unitId);
                if (unitId > 0)
                {
                    return Ok(tenantId);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }



        /// <summary>
        /// When trying to delete lease, check first the the unit has payments made
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        [HttpGet("ChildrenExists/{unitId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ChildrenExists(int unitId)
        {
            var location = GetControllerActionNames();
            try
            {
                var hasPayments = await _repoArrendamentos.ChildrenExists(unitId);
                return Ok(hasPayments);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Gera pagamentos (inicial + caução) no ato de celebração
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="arrendamento"></param>
        /// <returns></returns>
        [HttpPost("GeraMovimentos/{unitId:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GeraMovimentos(int unitId, [FromBody] ArrendamentoVM arrendamento)
        {
            var location = GetControllerActionNames();
            try
            {
                if (arrendamento is null)
                {
                    return BadRequest();
                }
                var _lease = _mapper.Map<Arrendamento>(arrendamento);
                await _repoArrendamentos.GeraMovimentos(_lease, unitId);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get generated document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetDocumentoGerado/{id:int}"), Produces("text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult GetDocumentoGerado(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var documentoGerado = _repoArrendamentos.GetDocumentoGerado(id);
                if (!string.IsNullOrEmpty(documentoGerado))
                {
                    return Ok(documentoGerado);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Contrato foi emitido?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ContratoEmitido/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ContratoEmitido(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var resultOk = await _repoArrendamentos.ContratoEmitido(id);
                return Ok(resultOk);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Nome do ficheiro pdf para apresentar no cliente
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet("GetPdfFilename/{filename}"), Produces("text/plain")]
        public IActionResult GetPdfFilename(string? filename)
        {
            var location = GetControllerActionNames();
            try
            {
                string fullPath = Path.Combine(_environment.ContentRootPath, "Reports", "Docs", "Contratos", filename!);
                fullPath = fullPath.Replace("docx", "pdf");
                return Ok(fullPath);

            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Marca contrato como emitido
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        [HttpGet("MarcaContratoComoEmitido/{id:int}/{doc}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult MarcaContratoComoEmitido(int id, string? doc)
        {
            var location = GetControllerActionNames();
            try
            {
                _repoArrendamentos.MarcaContratoComoEmitido(id, doc!);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("MarcaCartaAtualizacaoComoEmitida/{id:int}/{doc}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> MarcaCartaAtualizacaoComoEmitida(int id, string? doc)
        {
            var location = GetControllerActionNames();

            try
            {
                var updateOk = await _repoArrendamentos.MarcaCartaAtualizacaoComoEmitida(id, doc!);
                if (updateOk)
                    return NoContent();
                else
                    return InternalError($"{location}: Erro ao marcar carta de atualização como emitida.");
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("MarcaCartaAtrasoRendaComoEmitida/{id:int}/{doc}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> MarcaCartaAtrasoRendaComoEmitida(int id, string? doc)
        {
            var location = GetControllerActionNames();

            try
            {
                var updateOk = await _repoArrendamentos.MarcaCartaAtrasoRendaComoEmitida(id, doc!);
                if (updateOk)
                    return NoContent();
                else
                    return InternalError($"{location}: Erro ao marcar carta de atraso de renda como emitida.");
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Verifica se carta anual de atualização de rendas foi emitida
        /// </summary>
        /// <param name="ano"></param>
        /// <returns></returns>

        [HttpGet("CartaAtualizacaoEmitida/{ano}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> CartaAtualizacaoEmitida(int ano)
        {
            var location = GetControllerActionNames();

            try
            {
                if (ano < 1900 || ano > DateTime.UtcNow.Year)
                {
                    return BadRequest();
                }

                var atualizacaoJaEfetuada = await _repoArrendamentos.CartaAtualizacaoRendasEmitida(ano!);
                return Ok(atualizacaoJaEfetuada);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("RegistaCartaAtraso/{id:int}/{referralDateAsString}/{tentativa}/{doc}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegistaCartaAtraso(int id, string referralDateAsString, string tentativa, string doc)
        {
            var location = GetControllerActionNames();

            try
            {
                var referralDate = DateTime.Parse(referralDateAsString);
                var registerOk = await _repoArrendamentos.RegistaCartaAtraso(id, referralDate, tentativa, doc);
                if (registerOk)
                    return NoContent();
                else
                    return InternalError($"{location}: Erro ao registar carta de atraso no pagamento de rendas.");
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Verifica se carta de atraso de renda foi emitida
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("VerificaEnvioCartaAtrasoRenda/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> CartaAtrasoRendaEmitida(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }

                var envioJaEfetuado = await _repoArrendamentos.VerificaEnvioCartaAtrasoEfetuado(id);
                return Ok(envioJaEfetuado);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet("RegistaCartaOposicao/{id:int}/{doc}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegistaCartaOposicao(int id, string? doc)
        {
            var location = GetControllerActionNames();

            try
            {
                var updateOk = await _repoArrendamentos.RegistaCartaRevogacao(id, doc!);
                if (updateOk)
                    return Ok(updateOk);
                else
                    return InternalError($"{location}: Erro ao registar carta de oposição.");
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("RegistaCartaAtualizacaoRendas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RegistaCartaAtualizacaoRendas()
        {
            var location = GetControllerActionNames();

            try
            {
                var registrationOk = await _repoArrendamentos.RegistaProcessamentoAtualizacaoRendas();
                return Ok(registrationOk);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet("VerificaSeExisteCartaRevogacao/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> VerificaSeExisteCartaRevogacao(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                var letterAlreadySent = await _repoArrendamentos.VerificaSeExisteCartaRevogacao(id);
                return Ok(letterAlreadySent);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("VerificaSeExisteRespostaCartaRevogacao/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<bool> VerificaSeExisteRespostaCartaRevogacao(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                var letterAlreadyAnswered = await _repoArrendamentos.VerificaSeExisteRespostaCartaRevogacao(id);
                return letterAlreadyAnswered;
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return false;
            }
        }




        /// <summary>
        /// Marca contrato como não emitido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("MarcaContratoComoNaoEmitido/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult MarcaContratoComoNaoEmitido(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                _repoArrendamentos.MarcaContratoComoNaoEmitido(id);
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Total de rendas recebidas
        /// </summary>
        /// <returns></returns>
        [HttpGet("TotalRendas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult TotalRendas()
        {
            var location = GetControllerActionNames();
            try
            {
                decimal totRendas = _repoArrendamentos.TotalRendas();
                return Ok(totRendas);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Renovação automática
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("RenovacaoAutomatica/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult RenovacaoAutomatica(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var resultOk = _repoArrendamentos.RenovacaoAutomatica(id);
                return Ok(resultOk);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Check for new rents
        /// </summary>
        /// <returns></returns>
        [HttpGet("CheckNewRents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult CheckNewRents()
        {
            var location = GetControllerActionNames();
            try
            {
                _repoArrendamentos.CheckNewRents();
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Arrendamento existe?
        /// </summary>
        /// <param name="id">Id da fração</param>
        /// <returns></returns>
        [HttpGet("ArrendamentoExiste/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult ArrendamentoExiste(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var result = _repoArrendamentos.ArrendamentoExiste(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetLastId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> GetLastId()
        {
            var location = GetControllerActionNames();
            try
            {
                var Id = await _repoArrendamentos.GetLastId();
                return Ok(Id);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("UpdateLastPaymentDate/{id}/{paymentDate}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> UpdateLastPaymentDate(int id, DateTime date)
        {
            var location = GetControllerActionNames();
            try
            {
                await _repoArrendamentos.UpdateLastPaymentDate(id, date);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetLastPaymentDate/{unitId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> GetLastPaymentDate(int unitId)
        {
            var location = GetControllerActionNames();

            if (unitId < 1)
            {
                return BadRequest();
            }
            try
            {
                var date = await _repoArrendamentos.GetLastPaymentDate(unitId);
                return Ok(date);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpPost("InsertRentCoefficient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InsertRentCoefficient([FromBody] CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas)
        {
            var location = GetControllerActionNames();
            try
            {
                if (coeficienteAtualizacaoRendas is null)
                {
                    return BadRequest();
                }

                int insertedId = await _repoArrendamentos.InsertRentCoefficient(coeficienteAtualizacaoRendas);
                if (insertedId < 1)
                {
                    _logger.LogError($"{location}: Erro no Insert (coefificiente). Verifique log, p.f.");
                    return InternalError($"{location}: Erro no Insert (coefificiente). Verifique log, p.f.");
                }

                var viewRentCoefficient = await _repoArrendamentos.GetRentUpdatingCoefficient_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(Get_RentCoefficient_ById), new { id = viewRentCoefficient.Id }, viewRentCoefficient);

                return actionReturned;
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }

        }

        [HttpPut("UpdateRentCoefficient/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateRentCoefficient(int id, [FromBody] CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas)
        {
            var location = GetControllerActionNames();
            try
            {
                if (coeficienteAtualizacaoRendas == null)
                {
                    string msg = "O Coeficiente passado como paràmetro é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != coeficienteAtualizacaoRendas.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var coefficientToUpdate = await _repoArrendamentos.GetRentUpdatingCoefficient_ById(id);

                if (coefficientToUpdate == null)
                {
                    return NotFound($"O Coeficiente com o Id {id} não foi encontrado");
                }

                var updateOk = await _repoArrendamentos.UpdateRentCoefficient(coeficienteAtualizacaoRendas);
                if (!updateOk)
                    return BadRequest("Coefficient not updated");

                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetCurrentRentCoefficient/{ano}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> GetCurrentRentCoefficient(string? ano)
        {
            var location = GetControllerActionNames();

            if (string.IsNullOrEmpty(ano))
            {
                return BadRequest();
            }
            try
            {
                var coeficoefficient = await _repoArrendamentos.GetCurrentRentCoefficient(ano);
                return Ok(coeficoefficient);
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("Get_RentCoefficients")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_RentCoefficients()
        {
            var location = GetControllerActionNames();
            try
            {
                var coefficients = await _repoArrendamentos.GetRentUpdatingCoefficients();
                if (coefficients.Any())
                {
                    return Ok(coefficients);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("Get_RentCoefficient_ById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_RentCoefficient_ById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1) return NotFound();

                var rentCoefficient = await _repoArrendamentos.GetRentUpdatingCoefficient_ById(id);
                if (rentCoefficient is not null)
                {
                    return Ok(rentCoefficient);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("Get_RentCoefficient_ByYear/{year:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get_RentCoefficient_ByYear(int year)
        {
            var location = GetControllerActionNames();
            try
            {
                if (year < 1) return NotFound();

                if (year > DateTime.Now.Year) return BadRequest("Ano inválido");

                var rentCoefficient = await _repoArrendamentos.GetCoefficient_ByYear(year);
                if (rentCoefficient is not null)
                {
                    return Ok(rentCoefficient);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet("ExtendLeaseTerm/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExtendLeaseTerm(int Id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (Id < 0) return NotFound();

                if (Id > 0)
                {
                    var result = await _repoArrendamentos.GetArrendamento_ById(Id);
                    if (result is null)
                    {
                        return NotFound();
                    }
                }

                await _repoArrendamentos.ExtendLeaseTerm(Id);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"{location}: {e.Message} - {e.InnerException}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet("ApplicableLaws")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetApplicableLaws()
        {
            var location = GetControllerActionNames();
            try
            {
                var laws = await _repoArrendamentos.GetApplicableLaws();
                if (laws.Any())
                {
                    return Ok(laws);
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
            _logger.LogError(message);
            return StatusCode(500, $"Algo de errado ocorreu ({message}). Contacte o Administrador");
        }
    }

}

