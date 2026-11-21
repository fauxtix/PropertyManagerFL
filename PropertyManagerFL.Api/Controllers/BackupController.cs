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
        public async Task<ActionResult> BackupDatabase()
        {
            var location = GetControllerActionNames();
            try
            {
                var backupResult = await _repoBackup.BackupDatabase();
                return Ok(backupResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{Location}: falha ao efetuar backup da base de dados", location);
                return InternalError(location);
            }
        }
        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string location)
        {
            return StatusCode(500, $"Algo de errado ocorreu ({location}). Contacte o Administrador");
        }


    }
}
