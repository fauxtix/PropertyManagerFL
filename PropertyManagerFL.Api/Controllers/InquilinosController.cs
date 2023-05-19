using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Formatting;
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
        private readonly IFracaoRepository _repoFracoes;
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
        public InquilinosController(IMapper mapper, ILogger<InquilinosController> logger, IInquilinoRepository repoInquilinos, IWebHostEnvironment environment, IFracaoRepository repoFracoesInquilinos)
        {
            _mapper = mapper;
            this._logger = logger;
            _repoInquilinos = repoInquilinos;
            _environment = environment;
            _repoFracoes = repoFracoesInquilinos;
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
                    return NoContent();
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Conta corrente do inquilino
        /// </summary>
        /// <param name="id">Id Inquilino</param>
        /// <returns>IEnumerable</returns>
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
                if (tenantHistory.Any())
                {
                    var tenantHistoryList = _mapper.Map<IEnumerable<CC_InquilinoVM>>(tenantHistory);
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
        /// Get tenant by id 
        /// </summary>
        /// <param name="id">Tenant Id</param>
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
        /// Get id do inquilino da fracao a pesquisar
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
        /// Check if a rent update was already made for an unit (current year)
        /// </summary>
        /// <param name="tenantId">To check for the corresponding Unit Id, the one verified in this method</param>
        /// <returns></returns>
        [HttpGet("CheckForPriorRentUpdates_ThisYear/{tenantId:int}")]
        public async Task<IActionResult> CheckForPriorRentUpdates_ThisYear(int tenantId)
        {
            var location = GetControllerActionNames();
            try
            {
                var leaseData = await _repoInquilinos.GetLeaseData_ByTenantId(tenantId);
                var leaseStart = leaseData.Item1;
                var unitId = leaseData.Item2;

                var updateAlreadyMade = await _repoInquilinos.PriorRentUpdates_ThisYear(unitId);
                return Ok(updateAlreadyMade);
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

        [HttpGet("UpdateTenantRent/{Id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateTenantRent(int Id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (Id < 0)
                {
                    string msg = $"{location} - paràmetro incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                var tenant = await _repoInquilinos.GetInquilino_ById(Id);
                if (tenant is null)
                {
                    return BadRequest("Inquilino não encontrado");
                }

                var leaseParamsNeeded = await _repoInquilinos.GetLeaseData_ByTenantId(Id);
                var leaseStart = leaseParamsNeeded.Item1;
                var unitId = leaseParamsNeeded.Item2;

                if (unitId == 0)
                    return NotFound("Fração não está arrendada");

                var currentRentValue = (await _repoFracoes.GetUnit_ById(unitId)).ValorRenda;

                await _repoInquilinos.AtualizaRendaInquilino(unitId, leaseStart, currentRentValue);

                return Ok();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Atualização manual de renda 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="oldValue">Valor antes da atualização</param>
        /// <param name="newValue">Novo valor</param>
        /// <returns></returns>
        [HttpGet("UpdateTenantRent_Manually/{Id:int}/{oldValue}/{newValue}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> UpdateTenantRent_Manually(int Id, string oldValue, string newValue)
        {
            var location = GetControllerActionNames();
            try
            {
                if (Id < 0)
                {
                    string msg = $"{location} - paràmetro incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                var tenant = await GetTenant(Id);
                if (tenant is null)
                {
                    return BadRequest("Inquilino não encontrado");
                }

                var leaseParamsNeeded = await GetLeaseData(Id);

                var leaseStart = leaseParamsNeeded.leaseStart;
                var unitId = leaseParamsNeeded.unitId;

                if (unitId == 0)
                    return NotFound("Fração não está arrendada");

                if (!DataFormat.IsNumeric(oldValue))
                {
                    return BadRequest("valor corrente da renda inválido");
                }
                if (!DataFormat.IsNumeric(newValue))
                {
                    return BadRequest("Valor a atualizar inválido");
                }

                var oldValueParsed = DataFormat.DecimalParse(oldValue);
                var newValueParsed = DataFormat.DecimalParse(newValue);
                await _repoInquilinos.AtualizaRendaInquilino_Manual(unitId, leaseStart, oldValueParsed, newValueParsed);

                return Ok();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Atualização de rendas (se atualização estiver configurada como automática)
        /// </summary>
        /// <returns></returns>
        [HttpGet("RentUpdates")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetRentUpdates()
        {
            var location = GetControllerActionNames();
            try
            {
                var rentUpdates = await _repoInquilinos.GetAllRentUpdates();
                if (rentUpdates.Any())
                {
                    return Ok(rentUpdates);
                }
                else
                {
                    IEnumerable<HistoricoAtualizacaoRendasVM> empty = Enumerable.Empty<HistoricoAtualizacaoRendasVM>();
                    return Ok(empty);
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Devolve lista de atualizações de rendas (do inquilino)
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>IEnumerable</returns>
        [HttpGet("RentUpdates/{tenantId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> RentUpdates(int tenantId)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenantRentUpdates = await _repoInquilinos.GetRentUpdates_ByTenantId(tenantId);
                if (tenantRentUpdates.Any())
                {
                    return Ok(tenantRentUpdates);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Atualização/alteração de documento de um inquilino
        /// </summary>
        /// <param name="id">Id do Inquilino</param>
        /// <param name="alteraDocumentoInquilino">Dados do documento a atualizar</param>
        /// <returns></returns>
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

                var documentToUpdate = await GetTenantDocument(id);

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
        public async Task<IActionResult> AtualizaSaldo(int id, string saldoCorrente)
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

                if(DataFormat.IsNumeric(saldoCorrente) == false)
                {
                    return NotFound($"Formato do parâmetro 'SaldoCorrente' ({saldoCorrente}) não é válido");
                }

                decimal decSaldoCorrente = decimal.Parse(saldoCorrente);

                await _repoInquilinos.AtualizaSaldo(id, decSaldoCorrente);
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Renda paga pelo inquilino
        /// </summary>
        /// <param name="id">Id do inquilino</param>
        /// <returns></returns>
        [HttpGet("GetTenantRent/{id:int}")]
        public async Task<IActionResult> GetTenantRent(int id)
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

                var rentValue = await _repoInquilinos.GetTenantRent(id);
                return Ok(rentValue);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Delete tenant
        /// </summary>
        /// <param name="id">tenant Id</param>
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

        /// <summary>
        /// Cria novo documento de um inquilino
        /// </summary>
        /// <param name="novoDocumento">Dados do documento</param>
        /// <returns></returns>
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
                var insertedDocumentId = await _repoInquilinos.CriaDocumentoInquilino(novoDocumento);
                var viewDocument = await GetTenantDocument(insertedDocumentId);
                var actionReturned = CreatedAtAction(nameof(GetInquilinoById), new { id = viewDocument.Id }, viewDocument);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Delete tenant document
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

                var tenantDocumentToDelete = await GetTenantDocument(id);
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
                var document = await GetTenantDocument(id);
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

        
        /// <summary>
        /// All tenants documents
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// All Tenant Documents
        /// </summary>
        /// <param name="id">Tenant Id</param>
        /// <returns></returns>
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

        /// <summary>
        /// Todos os aumentos de rendas (dashboard)
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRentAdjustments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetRentAdjustments()
        {
            var location = GetControllerActionNames();
            try
            {
                var rentAdjustments = await _repoInquilinos.GetRentAdjustments();
                if (rentAdjustments.Any())
                {
                    return Ok(rentAdjustments);
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
        /// Get Late Payment Letters
        /// </summary>
        /// <returns></returns>
        [HttpGet("LatePaymentLetters")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetLatePaymentLetters()
        {
            var location = GetControllerActionNames();
            try
            {
                var letters = await _repoInquilinos.GetLatePaymentLetters();
                if (letters.Any())
                {
                    return Ok(letters);
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

        private async Task<InquilinoVM> GetTenant(int tenantId)
        {
            return await _repoInquilinos.GetInquilino_ById(tenantId);
        }

        private async Task<(DateTime leaseStart, int unitId)> GetLeaseData(int tenantId)
        {
            return await _repoInquilinos.GetLeaseData_ByTenantId(tenantId);
        }

        private async Task<DocumentoInquilinoVM> GetTenantDocument(int documentId)
        {
            return await _repoInquilinos.GetDocumentoById(documentId);
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
