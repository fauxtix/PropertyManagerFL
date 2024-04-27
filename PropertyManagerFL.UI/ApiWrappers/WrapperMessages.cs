using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Messages;
using PropertyManagerFL.UI.Services.ClientApi;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperMessages : IMessagesService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperMessages> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly HttpClientConfigurationService _httpClientConfigService;


        public WrapperMessages(IConfiguration env, ILogger<WrapperMessages> logger, HttpClient httpClient, HttpClientConfigurationService httpClientConfigService)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Messages";
            _logger = logger;
            _httpClient = httpClient;
            _httpClientConfigService = httpClientConfigService;
            _httpClientConfigService.ConfigureHttpClient(_httpClient);
        }

        public async Task<bool> Add(ComposeMessageVM messageVM)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}", messageVM))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Mensagem {exc.Message}");
                return false;
            }
        }

        public async Task<bool> Delete(int Id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/{Id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Mensagem)");
                return false;
            }
        }

        public async Task<IEnumerable<ComposeMessageVM>> GetAllMessages()
        {
            try
            {
                var messages = await _httpClient.GetFromJsonAsync<IEnumerable<ComposeMessageVM>>($"{_uri}");
                return messages!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API de mensagens");
                return Enumerable.Empty<ComposeMessageVM>();
            }
        }

        public async Task<ComposeMessageVM> GetMessageById(int id)
        {
            try
            {
                var contact = await _httpClient.GetFromJsonAsync<ComposeMessageVM>($"{_uri}/{id}");
                return contact!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (GetMessageById)");
                return new ComposeMessageVM();
            }
        }

        public async Task<bool> Save(int Id, ComposeMessageVM messageVM)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/{Id}", messageVM))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Mensagem (Messages)");
                return false;
            }
        }
    }
}
