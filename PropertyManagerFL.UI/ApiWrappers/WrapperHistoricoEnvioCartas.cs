using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Shared.Enums;
using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.UI.Services.ClientApi;

namespace PropertyManagerFL.UI.ApiWrappers;

public class WrapperHistoricoEnvioCartas : IHistoricoEnvioCartasService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _env;
    private readonly ILogger<WrapperHistoricoEnvioCartas> _logger;
    private readonly HttpClientConfigurationService _httpClientConfigService;


    private readonly string? _url;


    public WrapperHistoricoEnvioCartas(HttpClient httpClient,
        ILogger<WrapperHistoricoEnvioCartas> logger, IConfiguration env, HttpClientConfigurationService httpClientConfigService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _env = env;
        _url = $"{_env["BaseUrl"]}/historicoenviocartas";
        _httpClientConfigService = httpClientConfigService;
        _httpClientConfigService.ConfigureHttpClient(_httpClient);
    }

    public async Task<IEnumerable<HistoricoEnvioCartasVM>> GetLettersSent()
    {
        try
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync(_url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<IEnumerable<HistoricoEnvioCartasVM>>(data);
                    if (output != null)
                    {
                        return output;
                    }
                    else
                        return Enumerable.Empty<HistoricoEnvioCartasVM>();
                }
                else
                {
                    return Enumerable.Empty<HistoricoEnvioCartasVM>();
                }
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (HistoricoEnvioCartas/GetLettersSent)");
            return Enumerable.Empty<HistoricoEnvioCartasVM>();
        }
    }

    public async Task<bool> InsertLetterSent(HistoricoEnvioCartasVM letterSent, AppDefinitions.DocumentoEmitido letterType)
    {
        try
        {
            letterSent.IdTipoCarta = (int)letterSent.IdTipoCarta;

            using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync(_url, letterSent))
            {
                var success = result.IsSuccessStatusCode;
                return success;
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"Erro ao criar documento {exc.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateLetterAnsweredDate(int Id, DateTime answerDate)
    {
        throw new NotImplementedException();
    }
}
