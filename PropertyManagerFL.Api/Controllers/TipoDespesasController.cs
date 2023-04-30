using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Despesas;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoDespesasController : ControllerBase
    {
        private readonly ILogger<TipoDespesasController> _logger;
        private readonly ITipoDespesaRepository _repoTipoDespesas;
        private readonly IMapper _mapper;


        public TipoDespesasController(ILogger<TipoDespesasController> logger,
                                      ITipoDespesaRepository repoTipoDespesas,
                                      IMapper mapper)
        {
            _logger = logger;
            _repoTipoDespesas = repoTipoDespesas;
            _mapper = mapper;
        }


        [HttpPost("CriaTipoDespesa")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<int> CriaTipoDespesa([FromBody] TipoDespesaVM tipoDespesa)
        {
            var location = GetControllerActionNames();
            try
            {
                var expenseTypeToInsert = _mapper.Map<NovoTipoDespesa>(tipoDespesa);
                var createdId = await _repoTipoDespesas.InsereTipoDespesa(expenseTypeToInsert);
                var createdExpenseType = await _repoTipoDespesas.GetTipoDespesa_ById(createdId);
                var result = CreatedAtAction(nameof(GetTipoDespesaById), new { Id = createdId }, createdExpenseType);
                return createdId;
            }
            catch (Exception ex)
            {
                return -1; // InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpPut("AtualizaTipoDespesa/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizaTipoDespesa(int id, [FromBody] TipoDespesaVM expenseType)
        {
            var location = GetControllerActionNames();
            try
            {
                if (expenseType == null || id != expenseType.Id)
                    return BadRequest();
                if (_repoTipoDespesas.GetTipoDespesa_ById(id) == null)
                {
                    return NotFound();
                }
                var expenseToUpdate = _mapper.Map<AlteraTipoDespesa>(expenseType);

                await _repoTipoDespesas.AtualizaTipoDespesa(expenseToUpdate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpDelete("ApagaTipoDespesa/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApagaTipoDespesa(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }
                var despesaAApagar = await _repoTipoDespesas.GetTipoDespesa_ById(id);
                if (despesaAApagar == null)
                {
                    return NotFound($"Despesa com o Id {id} não encontrado");
                }

                var okToDelete = await _repoTipoDespesas.CanRecordBeDeleted(id);
                if (okToDelete)
                {
                    await _repoTipoDespesas.ApagaTipoDespesa(id);
                    return NoContent();
                }
                else
                    return BadRequest("Tipo de despesa com pagamentos. Pedido cancelado");
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }



        [HttpGet]
        [Route("GetAllTipoDespesas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetAllTipoDespesas()
        {
            var location = GetControllerActionNames();
            try
            {
                var result = await _repoTipoDespesas.GetAll();
                if (result.Any())
                {
                    return Ok(result);
                }

                return Ok(Enumerable.Empty<TipoDespesaVM>());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetTipoDespesas): {e.Message}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet]
        [Route("GetTipoDespesaById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetTipoDespesaById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }
                var result = await _repoTipoDespesas.GetTipoDespesa_ById(id);
                if (result != null)
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetExpenseTypeById): {e.Message}");
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
            return StatusCode(500, "Algo de errado ocorreu. Contacte o Administrador");
        }
    }


}

