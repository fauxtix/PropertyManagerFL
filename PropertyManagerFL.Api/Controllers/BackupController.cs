using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories.Data_Operations;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BackupController : ControllerBase
    {
        private readonly IBackupDBRepository _repoBackup;
        private readonly ILogger<BackupController> _logger;

        public BackupController(IBackupDBRepository repoBackup, ILogger<BackupController> logger)
        {
            _repoBackup = repoBackup;
            _logger = logger;
        }

        [HttpGet("BackupDatabase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult BackupDatabase()
        {
            var location = GetControllerActionNames();
            try
            {
                var backupResult = _repoBackup.BackupDatabase();
                return Ok(backupResult);
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
