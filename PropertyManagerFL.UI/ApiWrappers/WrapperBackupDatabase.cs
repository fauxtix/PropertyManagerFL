using AutoMapper;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.Common;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperBackupDatabase : IBackupDatabaseService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperBackupDatabase> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;

        public WrapperBackupDatabase(ILogger<WrapperBackupDatabase> logger, HttpClient httpClient, IConfiguration env)
        {
            _logger = logger;
            _httpClient = httpClient;
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Backup";
        }

        public async Task<bool?> BackupDatabase()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/BackupDatabase");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Backup BD)");
                return true;
            }
        }

    }
}
