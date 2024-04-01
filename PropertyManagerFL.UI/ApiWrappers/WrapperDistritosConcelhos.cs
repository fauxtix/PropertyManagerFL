using Newtonsoft.Json;

using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using System.Text;

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
    public async Task<IEnumerable<DistritoConcelho>> GetConcelhos()
    {
        try
        {

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_apiUri}/Concelhos"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<IEnumerable<DistritoConcelho>>(data);
                    return output!;
                }

                return Enumerable.Empty<DistritoConcelho>();
            }
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (Concelhos/GetConcelhos)");
            return Enumerable.Empty<DistritoConcelho>();
        }
    }

    public async Task<IEnumerable<DistritoConcelho>> GetConcelhosByDistrito(int id)
    {
        try
        {
            var output = await _httpClient.GetFromJsonAsync<IEnumerable<DistritoConcelho>>($"{_apiUri}/ConcelhosByDistrito/{id}");
            if (output!.Any())
                return output!;

            return Enumerable.Empty<DistritoConcelho>();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Erro ao pesquisar API (ConcelhosByDistrito)");
            return Enumerable.Empty<DistritoConcelho>();
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

    public async Task<bool> UpdateCoeficienteIMI(int Id, float coeficienteIMI)
    {
        try
        {
            var content = new StringContent(coeficienteIMI.ToString(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"{_apiUri}/updatecoeficienteIMI/{Id}", content);

            if (response.IsSuccessStatusCode)
            {
                return (true);
            }
            else
            {
                return (false);
            }

        }
        catch (HttpRequestException httpEx)
        {
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
