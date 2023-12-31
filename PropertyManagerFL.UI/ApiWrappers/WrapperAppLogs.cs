using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Logs;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperAppLogs : ILogService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperAppLogs> _logger;
        private readonly string? _appLogUri;
        private readonly HttpClient _httpClient;

        public WrapperAppLogs(IConfiguration env, ILogger<WrapperAppLogs> logger, HttpClient httpClient)
        {
            _env = env;
            _logger = logger;
            _appLogUri = $"{_env["BaseUrl"]}/Templates";
            _httpClient = httpClient;
        }

        public async Task DeleteAll()
        {
            try
            {
                await _httpClient.DeleteAsync($"{_appLogUri}");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao apagar logs (API AppLog)");
            }
        }
        public async Task DeleteFilteredRecords(IEnumerable<AppLogDto> filteredRecords)
        {
            try
            {
                foreach (var appLogDto in filteredRecords)
                {
                    await DeleteById(appLogDto.Id);
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao apagar logs por filtro (API AppLog)");
            }
        }

        public async Task DeleteById(int Id)
        {
            try
            {
                await _httpClient.DeleteAsync($"{_appLogUri}/ById/{Id}");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao apagar registo de log (API AppLog)");
            }
        }

        public async Task<IEnumerable<AppLogDto>> GetAppLogs()
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_appLogUri}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<AppLogDto>>(data);
                        return output?.ToList() ?? Enumerable.Empty<AppLogDto>();
                    }
                    else
                    {
                        return Enumerable.Empty<AppLogDto>();
                    }

                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
                return Enumerable.Empty<AppLogDto>();
            }
        }

        public async Task<AppLogDto> GetLog_ById(int id)
        {
            try
            {
                var appLog = await _httpClient.GetFromJsonAsync<AppLogDto>($"{_appLogUri}/{id}");
                return appLog;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
                return new AppLogDto();
            }
        }

        public async Task<IEnumerable<AppLogDto>> ViewLogins()
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_appLogUri}/Logins"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<AppLogDto>>(data);
                        if (output != null)
                            return output.ToList();
                        else
                            return Enumerable.Empty<AppLogDto>();
                    }
                    else
                    {
                        return Enumerable.Empty<AppLogDto>();
                    }

                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (AppLog)");
                return Enumerable.Empty<AppLogDto>();
            }
        }
    }
}
