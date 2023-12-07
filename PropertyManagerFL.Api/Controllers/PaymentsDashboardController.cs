using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;

namespace PropertyManagerFL.Api.Controllers
{
    /// <summary>
    /// Controler de recebimentos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsDashboardController : ControllerBase
    {
        private readonly IStatsRepository _repoStats;
        private readonly ILogger<PaymentsDashboardController> _logger;

        /// <summary>
        /// Sumário de recebimentos
        /// </summary>
        /// <param name="repoStats"></param>
        /// <param name="logger"></param>
        public PaymentsDashboardController(IStatsRepository repoStats, ILogger<PaymentsDashboardController> logger)
        {
            _repoStats = repoStats;
            _logger = logger;
        }
    }
}
