using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Application.ViewModels.Logs;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.UI.ApiWrappers;
public class WrapperAppSettings : IAppSettingsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _env;
    private readonly ILogger<WrapperAppSettings> _logger;
    private readonly string? _url;

    public WrapperAppSettings(HttpClient httpClient, IConfiguration env,
                                    ILogger<WrapperAppSettings> logger)
    {
        _env = env;
        _url = $"{_env["BaseUrl"]}/AppSettings";
        _logger = logger;

        _httpClient = httpClient;
    }

    public async Task<ApplicationSettingsVM> GetSettingsAsync()
    {
        try
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync(_url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<ApplicationSettingsVM>(data);
                    if (output != null)
                        return output;
                    else
                        return new ApplicationSettingsVM();
                }
                else
                {
                    return new ApplicationSettingsVM();
                }
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (AppSettings)");
            return new ApplicationSettingsVM();
        }
    }

    public async Task UpdateSettingsAsync(ApplicationSettingsVM settings)
    {
        await _httpClient.PutAsJsonAsync(_url, settings);
    }
}