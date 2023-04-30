using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAccessController : ControllerBase
    {
        private readonly ILogRepository _repoLoginsRepo;

        public LoginAccessController(ILogRepository repoLoginsRepo)
        {
            _repoLoginsRepo = repoLoginsRepo;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var logins = await _repoLoginsRepo.GetLogins();
            if (logins.Any())
            {
                return Ok(logins);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
