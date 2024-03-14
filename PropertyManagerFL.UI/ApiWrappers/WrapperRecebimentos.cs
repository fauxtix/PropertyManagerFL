using AutoMapper;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.UI.ApiWrappers
{
    public class WrapperRecebimentos : IRecebimentoService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperRecebimentos> _logger;
        private readonly string? _uri;
        private readonly bool AutomaticRentAdjustment;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;


        public WrapperRecebimentos(IConfiguration env, ILogger<WrapperRecebimentos> logger, HttpClient httpClient, IMapper mapper)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Recebimentos";
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;

            AutomaticRentAdjustment = bool.Parse(_env.GetSection("AppSettings:AutomaticRentAdjustment").Value);
        }


        public async Task AtualizaSaldoInquilino(int IdFracao, decimal decValorRecebido)
        {
            throw new NotImplementedException();
        }

        public void CriaMovimento_CC_Inquilino(int IdPropriedade, decimal decValorRecebido, DateTime dtMovimento)
        {
            throw new NotImplementedException();
        }


        public int GetFirstId()
        {
            throw new NotImplementedException();
        }

        public int GetID_TipoRecebimento_ByDescription(string Descricao_TipoRec)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TipoRecebimento> GetLista_TipoRecebimento()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetNomeInquilino(int Id)
        {
            throw new NotImplementedException();
        }

        public List<Arrendamento> GetPendingContracts()
        {
            throw new NotImplementedException();
        }

        public List<RecebimentoVM> GetResumedData()
        {
            throw new NotImplementedException();
        }

        public decimal GetValorRenda(int IdFracao)
        {
            throw new NotImplementedException();
        }

        public DateTime Get_Data_Prox_Pagamento(int IdFracao)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Recebimento> Query(string where = "")
        {
            throw new NotImplementedException();
        }

        public Recebimento Query_ById(int id)
        {
            throw new NotImplementedException();
        }

        public bool RegistoArrendamentoCriado(int IdPropriedade)
        {
            throw new NotImplementedException();
        }

        public string RegistoComErros(Recebimento recebimento)
        {
            throw new NotImplementedException();
        }

        public bool TableHasData()
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> GetTotalRecebimentos_ByType(int id)
        {
            try
            {
                var recebimentos = await _httpClient.GetFromJsonAsync<decimal>($"{_uri}/GetTotalRecebimentos/{id}");
                return recebimentos;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/GetTotalRecebimentos)");
                return -1;
            }
        }

        public async Task<IEnumerable<RecebimentoVM>> GetAll()
        {
            try
            {
                var recebimentos = await _httpClient.GetFromJsonAsync<IEnumerable<RecebimentoVM>>($"{_uri}/GetAll");
                return recebimentos;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/GetAll)");
                return Enumerable.Empty<RecebimentoVM>();
            }
        }

        Task<List<RecebimentoVM>> IRecebimentoService.GetResumedData()
        {
            throw new NotImplementedException();
        }

        public Task<decimal> TotalRecebimentos(int iTipoMovimento = 0)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> TotalRecebimentos_Inquilino(int IdInquilin)
        {
            throw new NotImplementedException();
        }
        public async Task<decimal> TotalRecebimentosPrevisto_Inquilino(int idInquilino)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/GetTotalRecebimentosPrevistos_Inquilino/{idInquilino}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var leaseExist = JsonConvert.DeserializeObject<decimal>(data);
                    return leaseExist;
                }

                return -1;

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/TotalRecebimentosPrevisto_Inquilino)");
                return -1;
            }
        }

        public async Task<RecebimentoVM> GetRecebimento_ById(int id)
        {
            try
            {
                var recebimento = await _httpClient.GetFromJsonAsync<RecebimentoVM>($"{_uri}/GetRecebimento_ById/{id}");
                return recebimento!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/GetRecebimento_ById)");
                return null;
            }
        }

        public async Task<int> ProcessMonthlyRentPayments()
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/ProcessMonthlyRentPayments"))
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<int>(data);
                    return output; // 1 = Ok, -1 = Problem
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao processar pagamentos mensais: {exc.Message}");
                return -1;
            }
        }

        public async Task<int> InsertRecebimento(RecebimentoVM recebimento, bool isBatchProcessing = false)
        {
            try
            {
                var recebimentoToInsert = _mapper.Map<NovoRecebimento>(recebimento);

                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereRecebimento/{isBatchProcessing}", recebimentoToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return success ? 1 : 0;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Recebimento {exc.Message}");
                return -1;
            }
        }

        public async Task<int> InsertRecebimentoTemp(RecebimentoVM recebimento)
        {
            try
            {
                var recebimentoToInsert = _mapper.Map<NovoRecebimento>(recebimento);

                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereRecebimentoTemp", recebimentoToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return 1;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Recebimento (Temp) {exc.Message}");
                return -1;
            }
        }


        public async Task<bool> UpdateRecebimento(int id, RecebimentoVM recebimento)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraRecebimento/{id}", recebimento))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Recebimento ({exc.Message})");
                return false;
            }
        }
        public async Task<bool> UpdateRecebimentoTemp(int id, RecebimentoVM recebimento)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/ChangedTempRentAmount/{id}", recebimento))
                {
                    return result.IsSuccessStatusCode;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Recebimento (Temp) ({exc.Message})");
                return false;
            }
        }

        public async Task<bool> DeleteRecebimento(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaRecebimento/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Recebimento ({exc.Message}))");
                return false;
            }
        }

        public async Task<IEnumerable<RecebimentoVM>> GeneratePagamentoRendas(int month, int year, bool automaticRentAdjustment = false)
        {
            try
            {
                var recebimentosGerados = await _httpClient.GetFromJsonAsync<IEnumerable<RecebimentoVM>>($"{_uri}/GeneratePagamentoRendas/{month}/{year}/{automaticRentAdjustment}");
                return recebimentosGerados!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/GeneratePagamentoRendas)");
                return Enumerable.Empty<RecebimentoVM>();
            }
        }

        public async Task<decimal> GetValorUltimaRendaPaga(int id)
        {
            try
            {
                var recebimentos = await _httpClient.GetFromJsonAsync<decimal>($"{_uri}/GetTotalRecebimentos/{id}");
                return recebimentos;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/GetValorUltimaRendaPaga)");
                return -1;
            }
        }

        public async Task<bool> RentalProcessingPerformed(int month, int year)
        {
            try
            {
                var OkToProceed = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/CheckIfPagamentoRendasAlreadyPerformed/{month}/{year}");
                return OkToProceed;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/RentalProcessingPerformed)");
                return false;
            }
        }

        // duplicated ?
        public async Task<int> InsertRecebimentoTemp(NovoRecebimento recebimentoTemp)
        {
            try
            {
                var recebimentoToInsert = _mapper.Map<NovoRecebimento>(recebimentoTemp);

                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereRecebimentoTemp", recebimentoToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return 1;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Recebimento {exc.Message}");
                return -1;
            }
        }

        public async Task<RecebimentoVM> GetRecebimentoTemp_ById(int id)
        {
            try
            {
                var recebimento = await _httpClient.GetFromJsonAsync<RecebimentoVM>($"{_uri}/GetRecebimento_Temp_ById/{id}");
                return recebimento!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/GetRecebimentoTemp_ById)");
                return new();
            }
        }

        public async Task DeleteRecebimentosTemp()
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaRecebimentosTemp"))
                {
                    var success = result.IsSuccessStatusCode;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar pagamentos temporários");
            }
        }

        public async Task<bool> LogRentProcessingPerformed()
        {
            try
            {
                NovoProcessamentoRendas processamento = new()
                {
                    Mes = DateTime.Now.Month,
                    Ano = DateTime.Now.Year,
                    DataProcessamento = DateTime.Now
                };

                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/LogRentProcessingPerformed", processamento))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar registo de log (processamento de rendas)");
                return false;
            }
        }

        public async Task<IEnumerable<RecebimentoVM>> GetAllTemp()
        {
            try
            {
                var recebimentosTemp = await _httpClient.GetFromJsonAsync<IEnumerable<RecebimentoVM>>($"{_uri}/GetAllTemp");
                return recebimentosTemp!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/GetAllTemp)");
                return Enumerable.Empty<RecebimentoVM>();
            }
        }

        public async Task ProcessMonthlyRentTransactions()
        {

        }

        public async Task<decimal> GetMaxValueAllowed_ManualInput(int idInquilino)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/GetMaxValueAllowed_ManualInput/{idInquilino}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var leaseExist = JsonConvert.DeserializeObject<decimal>(data);
                    return leaseExist;
                }

                return -1;

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/GetMaxValueAllowed_ManualInput)");
                return -1;
            }
        }

        public async Task<bool> AcertaPagamentoRenda(int idRecebimento, int paymentState, decimal valorAcerto)
        {
            try
            {
                var parAcerto = valorAcerto.ToString().Replace('.', ',');
                var response = await _httpClient.GetAsync($"{_uri}/AcertaPagamentoRenda/{idRecebimento}/{paymentState}/{parAcerto}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var updateOk = JsonConvert.DeserializeObject<bool>(data);
                    return updateOk;
                }

                return false;

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos/AcertaPagamentoRenda)");
                return false;
            }
        }

        public async Task<IEnumerable<ProcessamentoRendasDTO>> GetMonthlyRentsProcessed(int year)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/GetMonthlyRentsProcessed/{year}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<IEnumerable<ProcessamentoRendasDTO>>(data);
                    return output;
                }

                return null;

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API Recebimentos / GetMonthlyRentsProcessed");
                return null;
            }
        }

        public async Task<ProcessamentoRendasDTO> GetLastPeriodProcessed()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/GetLastPeriodProcessed");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<ProcessamentoRendasDTO>(data);
                    return output;
                }

                return null;

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Recebimentos / GetMonthlyRentsProcessed)");
                return new();
            }
        }

    }
}