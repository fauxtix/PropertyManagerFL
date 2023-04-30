using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Services.Stats;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesDashboardController : ControllerBase
    {
        private readonly IStatsService statsService;
        private readonly ILogger<ExpensesDashboardController> _logger;

        /// <summary>
        /// Sumário de Categoria de despesas
        /// </summary>
        /// <param name="statsService"></param>
        /// <param name="logger"></param>
        public ExpensesDashboardController(IStatsService statsService, ILogger<ExpensesDashboardController> logger)
        {
            this.statsService = statsService;
            _logger = logger;
        }

        /// <summary>
        /// Sumário de despesas
        /// </summary>
        /// <param name="year"></param>
        /// <returns>List of ExpenseSummaryData</returns>
        [HttpGet]
        [Route("GetCategoriesTotalAmount/{year:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetCategoriesSummary(int year)
        {
            var location = GetControllerActionNames();
            try
            {
                var result = await statsService.GetTotalExpenses(year);
                if (result.Any())
                {
                    return Ok(result);
                }

                return Ok(Enumerable.Empty<ExpensesSummaryData>());
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GetExpenses): {e.Message}");
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
