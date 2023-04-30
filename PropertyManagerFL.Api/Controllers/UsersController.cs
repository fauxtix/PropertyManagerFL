using MediaOrganizerApp.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


    public class UsersController : ControllerBase
    {

        private readonly ILogger<UsersController> _logger;
        private readonly IUsersRepository _repoUsers;

        public UsersController(ILogger<UsersController> logger, IUsersRepository repoUsers)
        {
            _logger = logger;
            _repoUsers = repoUsers;
        }

        [HttpGet("getroleidbyname/{rolename}")]
        public async Task<IActionResult> GetRoleIdByName(string roleName)
        {
            var location = GetControllerActionNames();
            try
            {
                var output = await _repoUsers.GetRoleIdByName(roleName);
                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no Api (GetRoleIdByName)");
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpGet("getuserroleId/{userid}")]
        public async Task<IActionResult> GetUserRoleId(string userid)
        {
            var location = GetControllerActionNames();
            try
            {
                string output = await _repoUsers.GetUserRoleId(userid);
                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no Api (GetUserRoleId)");
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpGet("getuserrolename/{userid}")]
        public async Task<IActionResult> GetUserRoleName(string userId)
        {
            var location = GetControllerActionNames();
            try
            {
                var output = await _repoUsers.GetUserRoleName(userId);
                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no Api (GetUserRoleName)");
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpGet("GetUserRoleName_ByName/{rolename}")]
        public async Task<IActionResult> GetUserRoleName_ByName(string roleName)
        {
            var location = GetControllerActionNames();
            try
            {
                var output = await _repoUsers.GetUserRoleName(roleName);
                return Ok(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no Api (GetUserRoleName_ByName)");
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
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
