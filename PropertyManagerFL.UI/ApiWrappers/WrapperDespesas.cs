using AutoMapper;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Despesas;
using PropertyManagerFL.Application.ViewModels.TipoDespesa;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperDespesas : IDespesaService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperDespesas> _logger;
        private readonly string? _uri;
        private readonly string? _uriImoveis;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public WrapperDespesas(IConfiguration env, ILogger<WrapperDespesas> logger, HttpClient httpClient, IMapper mapper)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Despesas";
            _uriImoveis = $"{_env["BaseUrl"]}/Imoveis";
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        public async Task<int> Insert(DespesaVM expense)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/CreateExpense", expense))
                {
                    var success = result.IsSuccessStatusCode;
                    return success ? 1 : -1;
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, $"Erro ao criar Despesa {exc.Message}");
                return -1;
            }
        }

        public async Task<bool> Update(int id, DespesaVM expense)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/UpdateExpense/{id}", expense))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, $"Erro ao atualizar Despesa)");
                return false;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/DeleteExpense/{id}"))
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

        public async Task<IEnumerable<DespesaVM>> GetAll()
        {
            try
            {
                var expenses = await _httpClient.GetFromJsonAsync<IEnumerable<DespesaVM>>($"{_uri}/GetDespesas");
                return expenses!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Despesas/GetAll)");
                return Enumerable.Empty<DespesaVM>();
            }
        }

        public async Task<DespesaVM> GetDespesa_ById(int id)
        {
            try
            {
                var expense = await _httpClient.GetFromJsonAsync<DespesaVM>($"{_uri}/GetExpenseById/{id}");
                return expense;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Despesas/GetDespesa_ById)");
                return null;
            }
        }

        public List<DespesaVM> GetResumedData()
        {
            throw new NotImplementedException();
        }


        public async Task<bool>  AreThereProperties()
        {
            try
            {
                var OkToInsertExpense = await _httpClient.GetFromJsonAsync<bool>($"{_uriImoveis}/TableHasData");
                return OkToInsertExpense;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Despesas/AreThereProperties)");
                return false;
            }

        }

        public List<DespesaVM> Query_ByYear(string sAno)
        {
            throw new NotImplementedException();
        }

        public decimal TotalDespesas(int iTipoDespesa = 0)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TipoDespesaVM>> GetTipoDespesa_ByCategoriaDespesa(int id)
        {
            try
            {
                var expenses = await _httpClient.GetFromJsonAsync<IEnumerable<TipoDespesaVM>>($"{_uri}/GetTipoDespesaByCategoriaDespesa/{id}");
                if (expenses != null)
                {
                    expenses = expenses.OrderBy(e => e.Descricao);
                    return expenses!.ToList();
                }
                return null;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Despesas/GetTipoDespesa_ByCategoriaDespesa)");
                return Enumerable.Empty<TipoDespesaVM>();
            }
        }
    }
}
