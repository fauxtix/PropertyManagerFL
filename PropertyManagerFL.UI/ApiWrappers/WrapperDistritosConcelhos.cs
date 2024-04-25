using Newtonsoft.Json;

using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using System.Globalization;
using System.Text;

namespace PropertyManagerFL.UI.ApiWrappers;

public class WrapperDistritosConcelhos : IDistritosConcelhosService
{
    private readonly IConfiguration _env;
    private readonly ILogger<WrapperDistritosConcelhos> _logger;
    private readonly string? _apiUri;
    private readonly IAppSettingsService _appSettings;
    private readonly HttpClient _httpClient;

    public WrapperDistritosConcelhos(IConfiguration env,
                                     ILogger<WrapperDistritosConcelhos> logger,
                                     HttpClient httpClient,
                                     IAppSettingsService appSettings)
    {
        _env = env;
        _logger = logger;
        _httpClient = httpClient;
        _apiUri = $"{_env["BaseUrl"]}/DistritosConcelhos";
        _appSettings = appSettings;
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

    public async Task<bool> UpdateCoeficienteIMI(int Id, decimal coeficienteIMI)
    {
        try
        {
            IFormatProvider culture;
            var coefIMI = coeficienteIMI.ToString();
            var app = await _appSettings.GetSettingsAsync();
            var appLanguage = app.DefaultLanguage;
            if ((!appLanguage.ToLower().Contains("en")))
            {
                coefIMI = ConvertToUSFormat(coefIMI);
            }

            var content = new StringContent(coefIMI, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"{_apiUri}/updatecoeficienteIMI/{Id}", content);

            if (response.IsSuccessStatusCode)
            {
                return (true);
            }
            else
            {
                _logger.LogError($"Erro na atualização do coeficiente IMI (Concelho: {Id} - Coeficiente: {coeficienteIMI})");
                return (false);
            }

        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError($"Erro na atualização do coeficiente IMI ({httpEx.Message})");

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro na atualização do coeficiente IMI ({ex.Message})");
            return false;
        }
    }

    static string ConvertToUSFormat(string decimalString)
    {
        // Define Portuguese culture
        CultureInfo portugueseCulture = CultureInfo.CreateSpecificCulture("pt-PT");

        // Parse the decimal string using Portuguese culture
        decimal decimalValue = decimal.Parse(decimalString, portugueseCulture);

        // Convert to string using en-US culture (to get the '.' as decimal separator)
        string usDecimalString = decimalValue.ToString(CultureInfo.InvariantCulture);

        // Parse back to decimal using en-US culture
        decimal convertedDecimal = decimal.Parse(usDecimalString, CultureInfo.InvariantCulture);

        return usDecimalString;
    }
}
