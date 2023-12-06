using AutoMapper;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.SituacaoFracao;
using PropertyManagerFL.Application.ViewModels.Logs;

namespace PropertyManagerFL.UI.ApiWrappers
{
    /// <summary>
    /// Api Wrapper de Fracões
    /// </summary>
    public class WrapperFracoes : IFracaoService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperFracoes> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        /// <param name="mapper"></param>
        public WrapperFracoes(IConfiguration env,
                              ILogger<WrapperFracoes> logger,
                              HttpClient httpClient,
                              IMapper mapper)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Fracoes";

            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        /// <summary>
        /// New unit
        /// </summary>
        /// <param name="unit"></param>
        /// <returns>Success</returns>
        public async Task<bool> InsereFracao(FracaoVM unit)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereFracao", unit))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Fração {exc.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update unit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="unit"></param>
        /// <returns>Success</returns>
        public async Task<bool> AtualizaFracao(int id, FracaoVM unit)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraFracao/{id}", unit))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Fração)");
                return false;
            }
        }

        /// <summary>
        /// Delete unit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task ApagaFracao(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaFracao/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    //return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Fração)");
                //return false;
            }
        }

        /// <summary>
        /// Get all apartemtns
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<FracaoVM>> GetAll()
        {
            try
            {
                var units = await _httpClient.GetFromJsonAsync<IEnumerable<FracaoVM>>($"{_uri}/GetFracoes");
                return units!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return Enumerable.Empty<FracaoVM>();
            }
        }

        public int GetFirstId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search unit by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View Model</returns>
        public async Task<FracaoVM> GetFracao_ById(int id)
        {
            try
            {
                var unit = await _httpClient.GetFromJsonAsync<FracaoVM>($"{_uri}/GetFracaoById/{id}");
                return unit!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        /// <summary>
        /// Search unit by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Unit</returns>
        public async Task<FracaoVM> GetUnit_ById(int id)
        {
            try
            {
                var unit = await _httpClient.GetFromJsonAsync<FracaoVM>($"{_uri}/GetUnitById/{id}");
                return unit!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        /// <summary>
        /// Get property units
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<FracaoVM>> GetFracoes_Imovel(int id = 0)
        {
            try
            {
                var units = await _httpClient.GetFromJsonAsync<IEnumerable<FracaoVM>>($"{_uri}/GetFracoes_Imovel/{id}");
                return units!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return Enumerable.Empty<FracaoVM>();
            }
        }

        public async Task<bool> FracaoEstaLivre(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/IsUnitFreeToLease/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var okForTransaction = JsonConvert.DeserializeObject<bool>(data);
                    return okForTransaction;
                }

                return false;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API: {exc.Message}");
                return false;
            }
        }


        /// <summary>
        /// Get units (lookup combo)
        /// </summary>
        /// <param name="idImovel"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LookupTableVM>> GetFracoes(int idImovel = 0)
        {
            try
            {
                var units = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetFracoesLookup/{idImovel}");
                return units!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return Enumerable.Empty<LookupTableVM>();
            }
        }

        public async Task<bool> MarcaFracaoComoAlugada(int id)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/SetUnitAsRented/{id}");
                return result;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return false;
            }
        }

        public async Task<bool> MarcaFracaoComoLivre(int id)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/SetUnitAsRented/{id}");
                return result;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return false;
            }
        }


        public async Task<int> GetIDSituacao_ByDescription(string description)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/GetIDSituacao_ByDescription/{description}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<int>(data);
                    return output;
                }

                return -1;
                //var result = await _httpClient.GetFromJsonAsync<int>($"{_uri}/GetIDSituacao_ByDescription/{description}");
                //return result;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return -1;
            }
        }

        public Task<string> GetNomeFracao(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<FracaoVM>> GetResumedData()
        {
            throw new NotImplementedException();
        }

        public Task<List<SituacaoFracaoVM>> GetSituacaoFracao()
        {
            throw new NotImplementedException();
        }

        public string RegistoComErros(Fracao fracao)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<LookupTableVM>> GetFracoes_ComArrendamentoCriado()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LookupTableVM>> GetFracoes_Disponiveis(int idImovel = 0)
        {
            try
            {
                var unitsAvailable = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetFracoesDisponiveis/{idImovel}");
                return unitsAvailable!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }

        }

        public async Task<IEnumerable<ImagemFracao>> GetImages_ByUnitId(int unitId) // 
        {
            try
            {
                var units = await _httpClient.GetFromJsonAsync<IEnumerable<ImagemFracao>>($"{_uri}/GetImages_ByUnitId/{unitId}");
                return units!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        public async Task<int> InsereImagemFracao(NovaImagemFracao imagem)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereImagemFracao", imagem))
                {
                    var success = result.IsSuccessStatusCode;
                    var output = result.Content.ReadAsStringAsync();
                    return Convert.ToInt32(output); ;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Imagem da fração {exc.Message}");
                return -1;
            }
        }

        public async Task<bool> AtualizaImagemFracao(int id, AlteraImagemFracao imagem)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraImagemFracao/{id}", imagem))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Imagem)");
                return false;
            }
        }

        public async Task ApagaImagemFracao(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaImagemFracao/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    //return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Imagem)");
                //return false;
            }
        }

        public async Task<ImagemFracao> GetImage_ByUnitId(int id)
        {
            try
            {
                var image = await _httpClient.GetFromJsonAsync<ImagemFracao>($"{_uri}/GetImage_ByUnitId/{id}");
                return image!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetFracoes_SemContrato(int propertyId)
        {
            try
            {
                var units = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetFracoesSemContrato/{propertyId}");
                return units!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API de frações - GetFracoes_SemContrato");
                return Enumerable.Empty<LookupTableVM>();
            }

        }

        public async Task<IEnumerable<LookupTableVM>> GetFracoes_WithDuePayments()
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/UnitsWithDuePayments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<LookupTableVM>>(data);
                        if(output != null)
                            return output.ToList();
                        else
                            return Enumerable.Empty<LookupTableVM>();
                    }
                    else
                    {
                        return Enumerable.Empty<LookupTableVM>();
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API de frações: {exc.Message}");
                return Enumerable.Empty<LookupTableVM>();
            }
        }

        public async Task<bool> AtualizaValorRenda(int Id, decimal novoValorRenda)
        {
            try
            {
                var normalizeParValue = novoValorRenda.ToString().Replace('.', ',');
                var response = await _httpClient.GetAsync($"{_uri}/AcertaPagamentoRenda/{Id}/{normalizeParValue}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var updateOk = JsonConvert.DeserializeObject<bool>(data);
                    return updateOk;
                }

                return false;


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
