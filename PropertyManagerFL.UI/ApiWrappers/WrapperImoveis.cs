using AutoMapper;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.GeoApi.CodigosPostais;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.UI.ApiWrappers
{
    /// <summary>
    /// Wrapper de imóveis (Api calls)
    /// </summary>
    public class WrapperImoveis : IImovelService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperInquilinos> _logger;
        private readonly string? _uri;
        private readonly string? _uriGeoApi;

        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        /// <summary>
        /// API Wrapper de Imóveis
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        /// <param name="mapper"></param>
        public WrapperImoveis(IConfiguration env,
                              ILogger<WrapperInquilinos> logger,
                              HttpClient httpClient,
                              IMapper mapper)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Imoveis";
            _uriGeoApi = $"{_env["BaseUrl"]}/GeoApi";
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        /// <summary>
        /// Cria registo do imóvel
        /// </summary>
        /// <param name="imovel"></param>
        /// <returns></returns>
        public async Task<bool> InsereImovel(ImovelVM imovel)
        {
            _logger.LogInformation("Criação de Imóvel no API");
            try
            {
                var propertyToInsert = _mapper.Map<NovoImovel>(imovel);
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereImovel", propertyToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Imóvel {exc.Message}");
                return false;
            }
        }

        /// <summary>
        /// Atualiza um determinado imóvel
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imovel"></param>
        /// <returns></returns>
        public async Task<bool> AtualizaImovel(int id, ImovelVM imovel)
        {
            try
            {
                var propertyToUpdate = _mapper.Map<AlteraImovel>(imovel);
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraImovel/{id}", propertyToUpdate))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Imóvel)");
                return false;
            }
        }

        /// <summary>
        /// Apaga imóvel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ApagaImovel(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaImovel/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Imóvel)");
                return false;
            }
        }

        /// <summary>
        /// Lê todos os registos de imóveis
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ImovelVM>> GetAll()
        {
            // https://localhost:4000/api/Imoveis/GetImoveis
            var endpoint = $"{_uri}/GetImoveis";
            try
            {
                var properties = await _httpClient.GetFromJsonAsync<IEnumerable<ImovelVM>>($"{_uri}/GetImoveis");
                return properties!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API de imóveis");
                return null;
            }
        }

        public async Task<int> GetCodigo_Imovel(int Id)
        {
            try
            {
                var propertyId = await _httpClient.GetStringAsync($"{_uri}/GetCodigo_Imovel/{Id}");
                return Convert.ToInt32(propertyId);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return 0; ;
            }
        }

        public async Task<string> GetDescricao_Imovel(int Id)
        {
            try
            {
                var propertyName = await _httpClient.GetStringAsync($"{_uri}/GetDescricao_Imovel/{Id}");
                return propertyName!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return "";
            }
        }

        /// <summary>
        /// Pesquisa imóvel por Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ImovelVM> GetImovel_ById(int id)
        {
            try
            {
                var property = await _httpClient.GetFromJsonAsync<ImovelVM>($"{_uri}/GetImovelById/{id}");
                return property!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        public async Task<string> GetNumeroPorta(int idImovel)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LookupTableVM>> GetPropertiesAsLookupTables()
        {
            try
            {
                var propertiesLookup = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetPropertiesLookup");
                return propertiesLookup!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null!;
            }
        }

        public async Task<GeoApi_CP7> GetFreguesiaConcelho(string? pst, string? pstEx)
        {
            try
            {
                var _geoApiEndPoint = $"{_uriGeoApi}/GetDataByFullPostalCode/{pst}/{pstEx}";

                var geoApiPostalCodesData = await _httpClient.GetFromJsonAsync<GeoApi_CP7>(_geoApiEndPoint);
                return geoApiPostalCodesData!;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new GeoApi_CP7()
                {
                    Localidade = "Código postal não devolveu dados... Inválido?"
                };
            }
        }
    }
}
