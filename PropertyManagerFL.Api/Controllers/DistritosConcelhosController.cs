using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Formatting;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using System.Globalization;

namespace PropertyManagerFL.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DistritosConcelhosController : ControllerBase
{
    private readonly ILogger<DistritosConcelhosController> _logger;
    private readonly IDistritosConcelhosRepository _repo;

    public DistritosConcelhosController(ILogger<DistritosConcelhosController> logger,
                                        IDistritosConcelhosRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    [HttpGet]
    [Route("Distritos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetDistritos()
    {
        var location = GetControllerActionNames();
        try
        {
            var result = await _repo.GetDistritos();
            if (result.Any())
            {
                return Ok(result);
            }

            return Ok(Enumerable.Empty<LookupTableVM>());
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro no Api (Distritos): {e.Message}");
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }

    [HttpGet]
    [Route("Concelhos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetConcelhos()
    {
        var location = GetControllerActionNames();
        try
        {
            var result = await _repo.GetConcelhos();
            if (result.Any())
            {
                return Ok(result);
            }

            return Ok(Enumerable.Empty<LookupTableVM>());
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro no Api (Concelhos): {e.Message}");
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }

    [HttpGet]
    [Route("ConcelhosByDistrito/{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> GetConcelhosByDistrito(int id)
    {
        var location = GetControllerActionNames();
        try
        {
            if (id < 1)
            {
                return BadRequest();
            }
            var result = await _repo.GetConcelhosByDistrito(id);
            if (result.Any())
            {
                return Ok(result);
            }

            return NotFound($"Sem resultados para o Id {id}");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Erro no Api (ConcelhosByDistrito): {e.Message}");
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }


    /// <summary>
    /// Altera coeficiente IMI
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    [HttpPatch("updatecoeficienteIMI/{Id:Int}")]
    public async Task<IActionResult> UpdateCoeficienteIMI(int Id, [FromBody] string coeficienteToUpdate)
    {
        var location = GetControllerActionNames();
        float coef;
        try
        {
            coef = float.Parse(coeficienteToUpdate, CultureInfo.InvariantCulture.NumberFormat);

            if (!DataFormat.IsNumeric(coef))
            {
                string msg = "Coeficiente com formato inválido (Api DistritosConcelhos)";
                _logger.LogWarning(msg);
                return BadRequest(msg);
            }

            var concelho = await _repo.GetConcelho_ById(Id);
            if (concelho == null)
                return NotFound();

            await _repo.UpdateCoeficienteIMI(Id, coef);
            return NoContent();
        }

        catch (Exception e)
        {
            _logger?.LogError($"{location}: {e.Message} - {e.InnerException}");
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
