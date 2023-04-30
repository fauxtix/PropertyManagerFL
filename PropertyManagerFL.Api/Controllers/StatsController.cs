using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;

namespace PropertyManagerFL.Api.Controllers
{

    /// <summary>
    /// Estatísticas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IStatsRepository _statsRepo;
        private readonly ILogger<StatsController> _logger;

        /// <summary>
        /// Controlador (geral) de estatísticas
        /// </summary>
        /// <param name="statsRepo"></param>
        /// <param name="logger"></param>
        public StatsController(IStatsRepository statsRepo, ILogger<StatsController> logger)
        {
            _statsRepo = statsRepo;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetTotalExpenses/{year:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetTotalExpenses(int year)
        {
            var location = GetControllerActionNames();
            try
            {
                var result = await _statsRepo.GetTotalExpenses(year);
                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetTotalExpenses): {e.Message}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet]
        [Route("GetTotalExpenses_ByYear/{year:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetTotalExpenses_ByYear(int year)
        {
            var location = GetControllerActionNames();
            try
            {
                var result = await _statsRepo.GetTotalExpenses_ByYear(year);
                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetTotalExpenses_ByYear): {e.Message}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }


        [HttpGet]
        [Route("GetExpensesCategoriesWithMoreSpending")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetExpensesCategoriesWithMoreSpending()
        {
            var location = GetControllerActionNames();
            try
            {
                var result = await _statsRepo.GetExpensesCategoriesWithMoreSpending();
                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetExpensesCategoriesWithMoreSpending): {e.Message}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        [HttpGet]
        [Route("GetExpensesCategoriesWithMoreSpendings_ByYear/{year:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetExpensesCategoriesWithMoreSpendings_ByYear(int year)
        {
            var location = GetControllerActionNames();
            try
            {
                var result = await _statsRepo.GetExpensesCategoriesWithMoreSpendings_ByYear(year);
                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetExpensesCategoriesWithMoreSpendings_ByYear): {e.Message}");
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
