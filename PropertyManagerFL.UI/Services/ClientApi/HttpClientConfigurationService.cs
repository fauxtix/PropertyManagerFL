using System.Net.Http.Headers;

namespace PropertyManagerFL.UI.Services.ClientApi;

public class HttpClientConfigurationService
{
    private readonly IConfiguration _configuration;

    public HttpClientConfigurationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureHttpClient(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("ApiKey", _configuration["ApiKey"]);
    }
}
