using AutoMapper;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperTipoDespesa : ITipoDespesaService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperTipoDespesa> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public WrapperTipoDespesa(IConfiguration env,
                                  ILogger<WrapperTipoDespesa> logger,
                                  HttpClient httpClient,
                                  IMapper mapper)
        {
            _env = env;
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
            _uri = $"{_env["BaseUrl"]}/TipoDespesas";

        }

        public async Task<int> Insert(TipoDespesaVM tipoDespesa)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/CriaTipoDespesa", tipoDespesa))
                {
                    var success = result.IsSuccessStatusCode;
                    return success ? 1 :-1;
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, $"Erro ao criar Tipo de Despesa {exc.Message}");
                return -1;
            }
        }

        public async Task<bool> Update(int id, TipoDespesaVM tipoDespesa)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AtualizaTipoDespesa/{id}", tipoDespesa))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, $"Erro ao atualizar Tipo de  Despesa)");
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaTipoDespesa/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Despesa)");
                return false;
            }
        }

        public async Task<IEnumerable<TipoDespesaVM>> GetAll()
        {
            try
            {
                var expenses = await _httpClient.GetFromJsonAsync<IEnumerable<TipoDespesaVM>>($"{_uri}/GetAllTipoDespesas");
                return expenses!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (TipoDespesa/GetAll)");
                return Enumerable.Empty<TipoDespesaVM>();
            }
        }

        public Task<int> GetFirstId()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetID_ByDescription(string Descricao)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TipoDespesaVM>> GetTipoDespesa_ByCategoria(int categoria)
        {
            throw new NotImplementedException();
        }

        public async Task<TipoDespesaVM> Get_ById(int id)
        {
            try
            {
                var expense = await _httpClient.GetFromJsonAsync<TipoDespesaVM>($"{_uri}/GetTipoDespesaById/{id}");
                return expense!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (TipoDespesa/Get_ById)");
                return new();
            }
        }


        public Task<IEnumerable<TipoDespesaVM>> Query(string where = "")
        {
            throw new NotImplementedException();
        }

        public string RegistoComErros(TipoDespesa tipoDespesa)
        {
            throw new NotImplementedException();
        }

        public bool TableHasData()
        {
            throw new NotImplementedException();
        }

    }
}
