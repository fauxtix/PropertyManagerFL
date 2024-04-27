using AutoMapper;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Proprietarios;
using PropertyManagerFL.UI.Services.ClientApi;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperProprietario : IProprietarioService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperProprietario> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly HttpClientConfigurationService _httpClientConfigService;


        public WrapperProprietario(IConfiguration env, ILogger<WrapperProprietario> logger, HttpClient httpClient, IMapper mapper, HttpClientConfigurationService httpClientConfigService)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Proprietarios";
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
            _httpClientConfigService = httpClientConfigService;
            _httpClientConfigService.ConfigureHttpClient(_httpClient);
        }

        public async Task<int> Insert(ProprietarioVM proprietario)
        {

            try
            {
                var landlordToInsert = _mapper.Map<NovoProprietario>(proprietario);

                var insertedId = await _httpClient.PostAsJsonAsync($"{_uri}/InsereProprietario", landlordToInsert);
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereProprietario", landlordToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return success ? 1 : 0;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Proprietário {exc.Message}");
                return -1;
            }
        }

        public async Task<bool> Update(int id, ProprietarioVM proprietario)
        {
            try
            {
                var landlordToUpdate = _mapper.Map<AlteraProprietario>(proprietario);

                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraProprietario/{id}", landlordToUpdate))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Proprietário");
                return false;
            }
        }


        public async Task<bool> Delete(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaProprietario/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Proprietário)");
                return false;
            }
        }

        public async Task<int> GetFirstId()
        {
            try
            {
                var firstId = await _httpClient.GetFromJsonAsync<int>($"{_uri}/GetFirstId");
                return firstId;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Proprietários - GetFirstId)");
                return -1;
            }
        }

        public async Task<ProprietarioVM> GetProprietario_ById(int id)
        {
            try
            {
                var endpoint = $"{_uri}/GetProprietario_ById/{id}";

                var landlord = await _httpClient.GetFromJsonAsync<ProprietarioVM>(endpoint);
                return landlord!;
            }
            catch (Exception exc)
            {
                _logger.LogInformation(exc, "Sem registo de Proprietários, requere criação (GetProprietario_ById)");
                return new ProprietarioVM();
            }
        }


        public IEnumerable<Proprietario> Query(string where = "")
        {
            throw new NotImplementedException();
        }

        public Task<Proprietario> Query_ById(int Id)
        {
            throw new NotImplementedException();
        }

        public string RegistoComErros(Proprietario proprietario)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TableHasData()
        {

            using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/TableHasData"))
            {
                var data = await response.Content.ReadAsStringAsync();
                var landlordCreated = JsonConvert.DeserializeObject<bool>(data);
                return landlordCreated;
            }
        }
    }
}
