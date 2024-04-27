using PropertyManagerFL.UI.Services.ClientApi;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public interface IWrapperImagens
    {
        Task<string> UploadImage(MultipartFormDataContent content);
    }

    /// <summary>
    /// wrapper de imagens
    /// </summary>
    public class WrapperImagens : IWrapperImagens
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperImagens> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly HttpClientConfigurationService _httpClientConfigService;


        /// <summary>
        /// images wrapper constructor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        public WrapperImagens(IConfiguration env, ILogger<WrapperImagens> logger, HttpClient httpClient, HttpClientConfigurationService httpClientConfigService)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Images";

            _logger = logger;
            _httpClient = httpClient;
            _httpClientConfigService = httpClientConfigService;
            _httpClientConfigService.ConfigureHttpClient(_httpClient);
        }

        /// <summary>
        /// Upload image
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> UploadImage(MultipartFormDataContent content)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/Save", content))
                {
                    var success = result;
                    return success.ToString();
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao carregar imagem {exc.Message}");
                return "";
            }
        }



    }
}
