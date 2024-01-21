using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.AppSettings;

namespace PropertyManagerFL.UI.ApiWrappers;
public class WrapperAppSettings : IAppSettingsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _env;
    private readonly ILogger<WrapperAppSettings> _logger;
    private readonly string? _url;
    private readonly string? _otherSettingsUrl;

    public WrapperAppSettings(HttpClient httpClient, IConfiguration env,
                                    ILogger<WrapperAppSettings> logger)
    {
        _env = env;
        _url = $"{_env["BaseUrl"]}/appsettings/emailsettings";
        _otherSettingsUrl = $"{_env["BaseUrl"]}/appsettings/othersettings";
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
    public async Task UpdateOtherSettingsAsync(ApplicationSettingsVM settings)
    {
        await _httpClient.PutAsJsonAsync(_otherSettingsUrl, settings);
    }
}