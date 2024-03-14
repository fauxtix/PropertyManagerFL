using Newtonsoft.Json;

using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.UI.ApiWrappers;

public class WrapperDistritosConcelhos : IDistritosConcelhosService
{
    private readonly IConfiguration _env;
    private readonly ILogger<WrapperDistritosConcelhos> _logger;
    private readonly string? _apiUri;
    private readonly HttpClient _httpClient;

    public WrapperDistritosConcelhos(IConfiguration env,
                                     ILogger<WrapperDistritosConcelhos> logger,
                                     HttpClient httpClient)
    {
        _env = env;
        _logger = logger;
        _httpClient = httpClient;
        _apiUri = $"{_env["BaseUrl"]}/DistritosConcelhos";

    }
    public async Task<IEnumerable<Concelho>> GetConcelhos()
    {
        try
        {

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUri}/Concelhos"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<IEnumerable<Concelho>>(data);
                    return output!;
                }

                return Enumerable.Empty<Concelho>();
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (Concelhos/GetConcelhos)");
            return Enumerable.Empty<Concelho>();
        }
    }

    public async Task<IEnumerable<Concelho>> GetConcelhosByDistrito(int id)
    {
        try
        {
            var output = await _httpClient.GetFromJsonAsync<IEnumerable<Concelho>>($"{_apiUri}/ConcelhosByDistrito/{id}");
            if (output!.Any())
                return output!;

            return Enumerable.Empty<Concelho>();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (ConcelhosByDistrito)");
            return Enumerable.Empty<Concelho>();
        }
    }

    public async Task<IEnumerable<LookupTableVM>> GetDistritos()
    {
        try
        {

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUri}/Distritos"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<IEnumerable<LookupTableVM>>(data);
                    return output!;
                }

                return Enumerable.Empty<LookupTableVM>();
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (Distritos/GetDistritos)");
            return Enumerable.Empty<LookupTableVM>();
        }
    }
}
