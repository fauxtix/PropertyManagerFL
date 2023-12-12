using AutoMapper;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.LookupTables;

namespace PropertyManagerFL.UI.ApiWrappers
{
    /// <summary>
    /// Wrapper de Fiadores
    /// </summary>
    public class WrapperFiadores : IFiadorService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperFiadores> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        /// <summary>
        /// wrapper Fiadores constructor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        /// <param name="mapper"></param>
        public WrapperFiadores(IConfiguration env,
                                 ILogger<WrapperFiadores> logger,
                                 HttpClient httpClient,
                                 IMapper mapper)
        {
            _httpClient = httpClient;
            _env = env;
            _logger = logger;
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Fiadores";
            _mapper = mapper;
        }

        /// <summary>
        /// Insert new guarantor
        /// </summary>
        /// <param name="Fiador"></param>
        /// <returns></returns>
        public async Task<bool> InsereFiador(FiadorVM Fiador)
        {
            _logger.LogInformation("Criação de Fiador no API");
            try
            {
                var fiadorToInsert = _mapper.Map<NovoFiador>(Fiador);
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereFiador", fiadorToInsert))
                {
                    return result.IsSuccessStatusCode;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Fiador {exc.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update tenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Fiador"></param>
        /// <returns></returns>
        public async Task<bool> AtualizaFiador(int id, FiadorVM Fiador)
        {
            try
            {
                var tenantToUpdate = _mapper.Map<AlteraFiador>(Fiador);
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraFiador/{id}", tenantToUpdate))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, "Erro ao atualizar Fiador)");
                return false;
            }
        }

        public async Task<bool> ApagaFiador(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaFiador/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Fiador)");
                return false;
            }
        }

        /// <summary>
        /// Get all Fiadores
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<FiadorVM>> GetAll()
        {
            try
            {
                var tenants = await _httpClient.GetFromJsonAsync<IEnumerable<FiadorVM>>($"{_uri}/GetFiadores");
                return tenants!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return Enumerable.Empty<FiadorVM>();
            }
        }
        /// <summary>
        /// Get tenant from api
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<FiadorVM?> Query_ById(int id)
        {
            try
            {
                var tenant = await _httpClient.GetFromJsonAsync<FiadorVM>($"{_uri}/GetFiadorById/{id}");
                return tenant;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }


        public int GetFirstIdFiador()
        {
            throw new NotImplementedException();
        }

        public int GetFirstId_Fiador()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetFiadorFracao(int ID_Fracao)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LookupTableVM>> GetFiadoresDisponiveis()
        {
            try
            {
                var output = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetFiadores");
                return output!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetFiadores_ForLookUp()
        {
            try
            {
                var output = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetFiadores_ForLookup");
                return output!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }

        }

        public async Task<FiadorVM> GetFiador_Inquilino(int id)
        {
            try
            {
                var output = await _httpClient.GetFromJsonAsync<FiadorVM>($"{_uri}/GetFiador_Inquilino/{id}");
                return output!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API - GetFiador_Inquilino({id})");
                return null;
            }

        }


        public Task<string> GetNomeFiador(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Fiador> Query(string where = "")
        {
            throw new NotImplementedException();
        }

        public string RegistoComErros(Fiador Fiador)
        {
            throw new NotImplementedException();
        }

        public bool TableHasData()
        {
            throw new NotImplementedException();
        }

        public async Task<FiadorVM> GetFiador_ById(int id)
        {
            try
            {
                var fiador = await _httpClient.GetFromJsonAsync<FiadorVM>($"{_uri}/GetFiadorById/{id}");
                return fiador;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }
    }
}
