using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.MailMerge;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperMailMerge : IMailMergeService
    {

        private readonly IConfiguration _env;
        private readonly ILogger<WrapperMailMerge> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;

        public WrapperMailMerge(IConfiguration env, ILogger<WrapperMailMerge> logger, HttpClient httpClient)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/MailMerge";

            _logger = logger;
            _httpClient = httpClient;
        }
        public async Task<string> MailMergeLetter(MailMergeModel model)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_uri}/MailMergeDocument", model);
                var resultString = await response.Content.ReadAsStringAsync();
                resultString = resultString.Replace("\"", "").Replace("\\\\", "\\");
                return resultString;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return "";
            }
        }
    }
}
