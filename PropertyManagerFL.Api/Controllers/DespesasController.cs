using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Despesas;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DespesasController : ControllerBase
    {
        private readonly ILogger<DespesasController> _logger;
        private readonly IDespesaRepository _repoDespesas;
        private readonly IMapper _mapper;


        public DespesasController(ILogger<DespesasController> logger, IDespesaRepository repoDespesas, IMapper mapper)
        {
            _logger = logger;
            _repoDespesas = repoDespesas;
            _mapper = mapper;
        }

        [HttpPost("CreateExpense")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateExpense([FromBody] DespesaVM expense)
        {
            var location = GetControllerActionNames();
            try
            {
                var expenseToInsert = _mapper.Map<NovaDespesa>(expense);
                var createdId = await _repoDespesas.Insert(expenseToInsert);
                var createdExpense = await _repoDespesas.GetDespesa_ById(createdId);
                var result = CreatedAtAction(nameof(GetExpenseById), new { Id = createdId }, createdExpense);
                return result;
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpPut("UpdateExpense/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] DespesaVM expense)
        {
            var location = GetControllerActionNames();
            try
            {
                if (expense == null || id != expense.Id)
                    return BadRequest();
                if (_repoDespesas.GetDespesa_ById(id) == null)
                {
                    return NotFound();
                }
                var expenseToUpdate = _mapper.Map<AlteraDespesa>(expense);

                await _repoDespesas.Update(expenseToUpdate);
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpDelete("DeleteExpense/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    _logger.LogWarning($"{location}: Delete failed with bad data - id: {id}");
                    return BadRequest();
                }
                var despesaAApagar = await _repoDespesas.GetDespesa_ById(id);
                if (despesaAApagar == null)
                {
                    return NotFound($"Despesa com o Id {id} não encontrado");
                }

                await _repoDespesas.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpGet]
        [Route("GetDespesas")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetDespesas()
        {
            var location = GetControllerActionNames();
            try
            {
                var result = await _repoDespesas.GetAll();
                if (result.Any())
                {
                    return Ok(result);
                }

                return Ok(Enumerable.Empty<DespesaVM>());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetDespesas): {e.Message}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet]
        [Route("GetExpenseById/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetExpenseById(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }
                var result = await _repoDespesas.GetDespesa_ById(id);
                if (result != null)
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetExpenseById): {e.Message}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet]
        [Route("GetTipoDespesaByCategoriaDespesa/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetTipoDespesaByCategoriaDespesa(int id)
        {
            var location = GetControllerActionNames();
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }
                var result = await _repoDespesas.GetTipoDespesa_ByCategoriaDespesa(id);
                if (result != null)
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetExpenseById): {e.Message}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        //[HttpGet]
        //[Route("GetLastId")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //public async Task<IActionResult> GetLastId()
        //{
        //    var location = GetControllerActionNames();
        //    try
        //    {
        //        var expenseId = await _repoDespesas.GetLastId();
        //        if (expenseId > 0)
        //        {
        //            return Ok(expenseId);
        //        }

        //        return BadRequest();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, $"Erro no Api (GetLastStaffId): {e.Message}");
        //        return InternalError($"{location}: {e.Message} - {e.InnerException}");
        //    }
        //}

        //[HttpGet]
        //[Route("SubCategoryInUse/{id:int}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //public async Task<IActionResult> SubCategoryInUse(int id)
        //{
        //    var location = GetControllerActionNames();
        //    try
        //    {
        //        var InUse = await _repoDespesas.IsSubCategoryInUse(id);
        //        if (!InUse)
        //        {
        //            return Ok();
        //        }

        //        return BadRequest();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, $"Erro no Api (SubCategoryInUse): {e.Message}");
        //        return InternalError($"{location}: {e.Message} - {e.InnerException}");
        //    }
        //}



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
