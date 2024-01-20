using PropertyManagerFL.Application.ViewModels;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public interface ICodigosPostais
    {
        Task<List<AddressVM>> GetAddresses(int codPst, int subCodPst);
    }

    public class WrapperCodigosPostais : ICodigosPostais
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperCodigosPostais> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;

        public WrapperCodigosPostais(IConfiguration env,
                                    ILogger<WrapperCodigosPostais> logger,
                                    HttpClient httpClient)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/AddressesFromPostalCode";
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<List<AddressVM>> GetAddresses(int codPst, int subCodPst)
        {
            try
            {
                var properties = await _httpClient.GetFromJsonAsync<List<AddressVM>>($"{_uri}/GetAddresses/{codPst}/{subCodPst}");
                return properties!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return new List<AddressVM>();
            }

        }

    }
}
