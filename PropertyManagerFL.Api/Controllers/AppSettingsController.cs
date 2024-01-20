using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.Logs;
using PropertyManagerFL.Core.Entities;
using static PropertyManagerFL.Application.ViewModels.AppSettings.ApplicationSettingsVM;
using static System.Reflection.Metadata.BlobBuilder;

namespace PropertyManagerFL.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppSettingsController : ControllerBase
{
    private readonly IAppSettingsRepository _appSettingsRepository;
    private readonly IMapper _mapper;

    public AppSettingsController(IAppSettingsRepository appSettingsRepository, IMapper mapper)
    {
        _appSettingsRepository = appSettingsRepository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettingsAsync()
    {
        var settings = await _appSettingsRepository.GetSettingsAsync();
        var clientSettings = _mapper.Map<ApplicationSettingsVM>(settings);

        return Ok(clientSettings);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettingsAsync(ApplicationSettingsVM settings)
    {
        var emtitySettings = _mapper.Map<ApplicationSettings>(settings);

        await _appSettingsRepository.UpdateSettingsAsync(emtitySettings);
        return NoContent();
    }
}
