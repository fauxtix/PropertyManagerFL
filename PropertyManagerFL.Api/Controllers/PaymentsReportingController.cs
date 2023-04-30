using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsReportingController : ControllerBase
    {

        private readonly IPaymentsReporting _repoPaymentsReporting;
        private readonly IPaymentsByMonthReporting _repoPaymentsByMonthReporting;
        private readonly ILogger<PaymentsReportingController> _logger;

        /// <summary>
        /// Pagamentos - Reporting
        /// </summary>
        /// <param name="repoPaymentsReporting"></param>
        /// <param name="logger"></param>
        public PaymentsReportingController(IPaymentsReporting repoPaymentsReporting,
                                           ILogger<PaymentsReportingController> logger,
                                           IPaymentsByMonthReporting repoPaymentsByMonthReporting)
        {
            _repoPaymentsReporting = repoPaymentsReporting;
            _repoPaymentsByMonthReporting = repoPaymentsByMonthReporting;
            _logger = logger;
        }

        /// <summary>
        ///  Caixa do dia
        /// </summary>
        /// <param name="paymentDate">data do pagamento</param>
        /// <returns></returns>
        [HttpGet]
        [Route("CaixaDoDia/{paymentDate}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult CaixaDoDia(DateTime paymentDate)
        {
            var location = GetControllerActionNames();

            try
            {
                _repoPaymentsReporting.GenerateReport_CaixaDia(paymentDate);
                return Ok();

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GenerateReport_CaixaDia): {e.Message}");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Caixa mês ano
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CaixaMesAno/{month:int}/{year:int}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult CaixaMesAno(int month, int year)
        {
            var location = GetControllerActionNames();

            try
            {
                _repoPaymentsByMonthReporting.GenerateReport_MesAno(DateTime.Now);
                return Ok();

            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Erro no Api (GenerateReport_CaixaDia): {e.Message}");
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
