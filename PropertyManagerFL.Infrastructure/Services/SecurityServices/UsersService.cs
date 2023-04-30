using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Services.Security;

namespace PropertyManagerFL.Infrastructure.Services.SecurityServices
{
    public class UsersService : IUsersService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UsersService> _logger;
        private readonly IConfiguration _env;
        private string apiErrorMessage = "Erro ao pesquisar API (UsersService)";

        private readonly string _Uri;

        public UsersService(HttpClient httpClient,
            ILogger<UsersService> logger,
            IConfiguration Env)
        {
            _httpClient = httpClient;
            _env = Env;
            _logger = logger;
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("ApiKey", _env["ApiKey"]);
            _Uri = $"{_env["BaseUrl"]}/users";
        }

        public async Task<string> GetRoleIdByName(string roleName)
        {
            string uri = $"{_Uri}/getroleidbyname/{roleName}";
            _logger.LogInformation($"Ler Id do Role por nome {roleName}");
            try
            {
                var _role = await _httpClient
                    .GetStringAsync(uri);
                return _role;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, apiErrorMessage);
                throw;
            }
        }

        public async Task<string> GetUserRoleId(string userId)
        {
            string uri = $"{_Uri}/getuserroleId/{userId}";
            _logger.LogInformation($"Ler Id do Role por User Id {userId}");
            try
            {
                string _role = await _httpClient.GetStringAsync(uri);
                return _role;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, apiErrorMessage);
                throw;
            }
        }

        public async Task<string> GetUserRoleName(string userId)
        {
            string uri = $"{_Uri}/getuserrolename/{userId}";
            _logger.LogInformation($"Ler nome do role por Id {userId}");
            try
            {
                var _role = await _httpClient.GetStringAsync(uri);
                return _role;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, apiErrorMessage);
                throw;
            }
        }

        public string GetUserRoleName_ByEmail(string userEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetUserRoleName_ByName(string roleName)
        {
            string uri = $"{_Uri}/GetUserRoleName_ByName/{roleName}";
            _logger.LogInformation($"Ler role por nome {roleName}");
            try
            {
                var _role = await _httpClient
                    .GetStringAsync(uri);
                return _role;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, apiErrorMessage);
                throw;
            }
        }

    }
}
