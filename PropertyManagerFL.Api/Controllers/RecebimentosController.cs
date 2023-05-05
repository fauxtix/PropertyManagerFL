using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Recebimentos;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RecebimentosController : ControllerBase
    {
        private readonly IRecebimentoRepository _repoRecebimentos;
        private readonly IArrendamentoRepository _repoArrendamentos;
        private readonly IInquilinoRepository _repoInquilinos;
        private readonly IMapper _mapper;
        private readonly ILogger<RecebimentosController> _logger;

        public RecebimentosController(IRecebimentoRepository repoRecebimentos,
                                      IMapper mapper,
                                      ILogger<RecebimentosController> logger,
                                      IInquilinoRepository repoInquilinos,
                                      IArrendamentoRepository repoArrendamentos)
        {
            _repoRecebimentos = repoRecebimentos;
            _mapper = mapper;
            _logger = logger;
            _repoInquilinos = repoInquilinos;
            _repoArrendamentos = repoArrendamentos;
        }

        [HttpPost("InsereRecebimento/{isBatchProcessing}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereRecebimento(bool isBatchProcessing, [FromBody] RecebimentoVM recebimento)
        {
            var location = GetControllerActionNames();
            try
            {
                if (recebimento is null)
                {
                    return BadRequest();
                }
                var recebimentoToInsert = _mapper.Map<NovoRecebimento>(recebimento);
                int insertedId = await _repoRecebimentos.InsertRecebimento(recebimentoToInsert, isBatchProcessing);

                if (insertedId < 1)
                {
                    return InternalError($"{location} => Erro ao inserir movimento "); ;
                }

                var viewRecebimento = await _repoRecebimentos.GetRecebimento_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetRecebimento_ById), new { id = viewRecebimento.Id }, viewRecebimento);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpPost("InsereRecebimentoTemp")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> InsereRecebimentoTemp([FromBody] RecebimentoVM recebimento)
        {
            var location = GetControllerActionNames();
            try
            {
                if (recebimento is null)
                {
                    return BadRequest();
                }
                var recebimentoToInsert = _mapper.Map<NovoRecebimento>(recebimento);
                int insertedId = await _repoRecebimentos.InsertRecebimentoTemp(recebimentoToInsert);

                if (insertedId < 1)
                {
                    return InternalError($"{location} => Erro ao inserir movimento "); ;
                }

                var viewRecebimento = await _repoRecebimentos.GetRecebimentoTemp_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetRecebimento_Temp_ById), new { id = viewRecebimento.Id }, viewRecebimento);
                return actionReturned;
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("ProcessMonthlyRentPayments")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ProcessMonthlyRentPayments()
        {
            var location = GetControllerActionNames();
            try
            {
                int response = await _repoRecebimentos.ProcessMonthlyRentPayments();

                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpPut("AlteraRecebimento/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AlteraRecebimento(int id, [FromBody] RecebimentoVM recebimento)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    string msg = "O paràmetro passado é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (recebimento == null)
                {
                    string msg = "O Recebimento passado como paràmetro é incorreto.";
                    _logger.LogWarning(msg);
                    return BadRequest(msg);
                }

                if (id != recebimento.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var recebimentoToUpdate = await _repoRecebimentos.GetRecebimento_ById(id);

                if (recebimentoToUpdate == null)
                {
                    return NotFound($"O Recebimento com o Id {id} não foi encontrado");
                }

                var alteraRecebimento = _mapper.Map<AlteraRecebimento>(recebimento);
                var updatedRecebimentoOk = await _repoRecebimentos.UpdateRecebimento(alteraRecebimento);
                if (!updatedRecebimentoOk)
                    return BadRequest("Recebimento não atualizado");

                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        [HttpPut("ChangedTempRentAmount/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> ChangedTempRentAmount(int id, [FromBody] RecebimentoVM recebimento)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning("O paràmetro passado é incorreto.");
                    return BadRequest("O paràmetro passado é incorreto.");
                }

                if (recebimento == null)
                {
                    _logger.LogWarning("Parâmetros inválidos.");
                    return BadRequest("Parâmetros inválidos.");
                }

                if (id != recebimento.Id)
                {
                    return BadRequest($"O id ({id}) passado como paràmetro é incorreto");
                }

                var recebimentoToUpdate = await _repoRecebimentos.GetRecebimentoTemp_ById(id);

                if (recebimentoToUpdate == null)
                {
                    return NotFound($"O Recebimento com o Id {id} não foi encontrado");
                }

                var alteraRecebimento = _mapper.Map<AlteraRecebimento>(recebimento);
                var updateOk = await _repoRecebimentos.UpdateRecebimentoTemp(alteraRecebimento);

                return Ok(updateOk);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpDelete("ApagaRecebimento/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaRecebimento(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }

                var recebimentoToDelete = await _repoRecebimentos.GetRecebimento_ById(id);
                if (recebimentoToDelete == null)
                {
                    _logger.LogError($"Recebimento com o Id {id} não encontrado");
                    return NotFound($"Recebimento com o Id {id} não encontrado");
                }

                var deleteOk = await _repoRecebimentos.DeleteRecebimento(id);
                if (deleteOk == false)
                {
                    return InternalError($"{location}: Erro ao apagar transação");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpDelete("ApagaRecebimentosTemp")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaRecebimentosTemp()
        {
            var location = GetControllerActionNames();
            try
            {

                await _repoRecebimentos.DeleteRecebimentosTemp();
                return NoContent();

            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }



        [HttpGet("GetRecebimento_ById/{id:int}")]
        public async Task<IActionResult> GetRecebimento_ById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var recebimento = await _repoRecebimentos.GetRecebimento_ById(id);
                if (recebimento is not null)
                    return Ok(recebimento);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetRecebimento_Temp_ById/{id:int}")]
        public async Task<IActionResult> GetRecebimento_Temp_ById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var recebimento = await _repoRecebimentos.GetRecebimentoTemp_ById(id);
                if (recebimento is not null)
                    return Ok(recebimento);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetProcessamentoRendas_ById/{id:int}")]
        public async Task<IActionResult> GetProcessamentoRendas_ById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var procRendas = await _repoRecebimentos.GetProcessamentoRendas_ById(id);
                if (procRendas is not null)
                    return Ok(procRendas);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Tipo de Recebimento (0 = todos)</param>
        /// <returns></returns>
        [HttpGet("GetTotalRecebimentos/{id:int}")]
        public async Task<IActionResult> GetTotalRecebimentos(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var recebimentos = await _repoRecebimentos.TotalRecebimentos(id);
                return Ok(recebimentos);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var location = GetControllerActionNames();
            try
            {
                var recebimentos = await _repoRecebimentos.GetAll();
                return Ok(recebimentos);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        [HttpGet("GetAllTemp")]
        public async Task<IActionResult> GetAllTemp()
        {
            var location = GetControllerActionNames();
            try
            {
                var recebimentosTemp = await _repoRecebimentos.GetAllTemp();
                return Ok(recebimentosTemp);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet("GetTotalRecebimentos_Inquilino/{id:int}")]
        public async Task<IActionResult> GetTotalRecebimentos_Inquilino(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var tenant = await _repoInquilinos.GetInquilino_ById(id);
                if (tenant is null)
                {
                    return NotFound("Inquilino não encontrado");
                }

                var totalRecebido = await _repoRecebimentos.TotalRecebimentos_Inquilino(id);
                return Ok(totalRecebido);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetMonthlyRentsProcessed/{year:int}")]
        public async Task<IActionResult> GetMonthlyRentsProcessed(int year)
        {
            var location = GetControllerActionNames();
            try
            {
                var output = (await _repoRecebimentos.GetMonthlyRentsProcessed(year)).ToList();
                if (output == null && output.Count == 0)
                    return BadRequest("Falha ao ler processamento de rendas");

                return Ok(output);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetLastPeriodProcessed")]
        public async Task<IActionResult> GetLastPeriodProcessed()
        {
            var location = GetControllerActionNames();
            try
            {
                var output = await _repoRecebimentos.GetLastPeriodProcessed();
                if (output == null)
                {
                    return Ok(new List<ProcessamentoRendasDTO>());
                }

                return Ok(output);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetTotalRecebimentosPrevistos_Inquilino/{id:int}")]
        public async Task<IActionResult> GetTotalRecebimentosPrevistos_Inquilino(int id) // Inquilino.Id
        {
            var location = GetControllerActionNames();
            try
            {
                var tenant = await _repoInquilinos.GetInquilino_ById(id);
                if (tenant is null)
                {
                    return NotFound("Inquilino não encontrado");
                }

                var totalPrevisto = await _repoRecebimentos.TotalRecebimentosPrevisto_Inquilino(id);
                return Ok(totalPrevisto);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetValorUltimaRendaPaga/{id}")]
        public async Task<IActionResult> GetValorUltimaRendaPaga(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var valorUltRendaPaga = await _repoRecebimentos.GetValorUltimaRendaPaga(id);
                if (valorUltRendaPaga < 1)
                    return BadRequest("Falha ao ler valor da última renda paga");

                return Ok(valorUltRendaPaga);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("GetMaxValueAllowed_ManualInput/{id}")]
        public async Task<IActionResult> GetMaxValueAllowed_ManualInput(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                var maxValueAllowed = await _repoRecebimentos.GetMaxValueAllowed_ManualInput(id);
                if (maxValueAllowed < 1)
                    return BadRequest("Falha ao ler valor (Max value allowed)");

                return Ok(maxValueAllowed);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        /// <summary>
        /// Gera pagamento de rendas temporário, sujeito a confirmação
        /// </summary>
        /// <returns></returns>
        [HttpGet("GeneratePagamentoRendas/{month:int}/{year:int}")]
        public async Task<IActionResult> GeneratePagamentoRendas(int month, int year)
        {
            var location = GetControllerActionNames();
            try
            {
                IEnumerable<ArrendamentoVM> arrendamentos = (await _repoArrendamentos.GetAll())
                    .Where(p => p.Ativo == true);
                if (arrendamentos.Any())
                {
                    var pagamentosGerados = await _repoRecebimentos.GeneratePagamentoRendas(arrendamentos, month, year);
                    if (pagamentosGerados is null)
                        return BadRequest("Falha na criação de pagamentos de rendas");

                    return Ok(pagamentosGerados);
                }
                else
                {
                    return BadRequest("Não há arrendamentos ativos! Confirme");
                }

            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet("CheckIfPagamentoRendasAlreadyPerformed/{month:int}/{year:int}")]
        public async Task<IActionResult> CheckIfPagamentoRendasAlreadyPerformed(int month, int year)
        {
            var location = GetControllerActionNames();
            try
            {
                bool AlreadyPerformed = await _repoRecebimentos.RentalProcessingPerformed(month, year);
                return Ok(AlreadyPerformed);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet("AcertaPagamentoRenda/{id:int}/{paymentState}/{valueToUpdate}")]
        public async Task<IActionResult> AcertaPagamentoRenda(int id, int paymentState, string valueToUpdate)
        {
            var location = GetControllerActionNames();
            try
            {

                var rec = await _repoRecebimentos.GetRecebimento_ById(id);
                if (rec is null)
                {
                    return NotFound("Id não encontrado");
                }

                if (!DataFormat.IsNumeric(valueToUpdate))
                {
                    return BadRequest("Valor a acertar inválido");
                }

                var parsedValue = DataFormat.DecimalParse(valueToUpdate);

                var updateOk = await _repoRecebimentos.AcertaPagamentoRenda(id, paymentState, parsedValue);
                return Ok(updateOk);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpPost("LogRentProcessingPerformed")]
        public async Task<IActionResult> LogRentProcessingPerformed([FromBody] NovoProcessamentoRendas record)
        {
            var location = GetControllerActionNames();
            try
            {
                var insertedId = await _repoRecebimentos.LogRentProcessingPerformed(record);
                var viewLog = await _repoRecebimentos.GetProcessamentoRendas_ById(insertedId);
                var actionReturned = CreatedAtAction(nameof(GetProcessamentoRendas_ById), new { id = viewLog.Id }, viewLog);
                return actionReturned;
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

