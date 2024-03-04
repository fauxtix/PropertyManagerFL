using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppSettingsController : ControllerBase
{
    private readonly IAppSettingsRepository _appSettingsRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AppSettingsController>? _logger;
    private readonly IWebHostEnvironment _environment;


    public AppSettingsController(IAppSettingsRepository appSettingsRepository,
                                 IMapper mapper,
                                 IWebHostEnvironment environment)
    {
        _appSettingsRepository = appSettingsRepository;
        _mapper = mapper;
        _environment = environment;
    }

    /// <summary>
    /// Lê settings do e-mail (outlook)
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    [HttpGet("emailsettings")]
    public async Task<IActionResult> GetSettingsAsync()
    {
        var location = GetControllerActionNames();

        try
        {
            var settings = await _appSettingsRepository.GetSettingsAsync();
            if (settings is not null)
            {
                var clientSettings = _mapper.Map<ApplicationSettingsVM>(settings);
                return Ok(clientSettings);
            }
            else
            {
                return NotFound();
            }

        }
        catch (Exception e)
        {
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }

    /// <summary>
    /// Altera settings do e-mail (outlook)
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    [HttpPut("emailsettings")]
    public async Task<IActionResult> UpdateSettingsAsync(ApplicationSettingsVM settings)
    {
        var location = GetControllerActionNames();
        try
        {
            if (settings == null)
            {
                string msg = "As definições passadas são incorretas.";
                _logger?.LogWarning("As definições passadas são incorretas.");
                return BadRequest(msg);
            }

            var entitySettings = _mapper.Map<ApplicationSettings>(settings);

            await _appSettingsRepository.UpdateSettingsAsync(entitySettings);
            return NoContent();

        }
        catch (Exception e)
        {
            _logger?.LogError($"{location}: {e.Message} - {e.InnerException}");
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }

    /// <summary>
    /// Altera settings (outros)
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    [HttpPut("othersettings")]
    public async Task<IActionResult> UpdateOtherSettingsAsync(ApplicationSettingsVM settings)
    {
        var location = GetControllerActionNames();

        try
        {
            if (settings == null)
            {
                string msg = "As definições passadas são incorretas.";
                _logger?.LogWarning("As definições passadas são incorretas.");
                return BadRequest(msg);
            }

            var emtitySettings = _mapper.Map<ApplicationSettings>(settings);

            await _appSettingsRepository.UpdateOtherSettingsAsync(emtitySettings);
            return NoContent();

        }
        catch (Exception e)
        {
            _logger?.LogError($"{location}: {e.Message} - {e.InnerException}");
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }

    /// <summary>
    /// Inicializa tabelas de pagamentos
    /// </summary>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    [HttpGet("initialize")]
    public async Task<IActionResult> Initialize()
    {
        var location = GetControllerActionNames();

        try
        {
            await _appSettingsRepository.InitializeRentProcessingTables();

            string docsAtualizacaoRendasPath = Path.Combine(_environment.ContentRootPath, "Reports", "Docs", "AtualizacaoRendas");
            string docsContratosPath = Path.Combine(_environment.ContentRootPath, "Reports", "Docs", "Contratos");
            string docsOposicaoRenovacaoPath = Path.Combine(_environment.ContentRootPath, "Reports", "Docs", "OposicaoRenovacaoContrato");
            string docsAtrasoRendasPath = Path.Combine(_environment.ContentRootPath, "Reports", "Docs", "RendasAtraso");

            DeleteFiles(docsAtualizacaoRendasPath);
            DeleteFiles(docsContratosPath);
            DeleteFiles(docsOposicaoRenovacaoPath);
            DeleteFiles(docsAtrasoRendasPath);

            return Ok($"{location} - Tabelas inicializadas com sucesso");
        }
        catch (Exception e)
        {
            _logger?.LogError($"{location}: {e.Message} - {e.InnerException}");
            return InternalError($"{location}: {e.Message} - {e.InnerException}");
        }
    }


    private void DeleteFiles(string path)
    {
        var files = Directory.GetFiles(path);
        if (files.Length == 0) return;

        Array.ForEach(files, System.IO.File.Delete);

    }
    private string GetControllerActionNames()
    {
        var controller = ControllerContext.ActionDescriptor.ControllerName;
        var action = ControllerContext.ActionDescriptor.ActionName;

        return $"{controller} - {action}";
    }

    private ObjectResult InternalError(string message)
    {
        _logger?.LogError(message);
        return StatusCode(500, "Algo de errado ocorreu. Contacte o Administrador");
    }

}
