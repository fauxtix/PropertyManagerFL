using AutoMapper;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.MailMerge;
using PropertyManagerFL.Application.ViewModels.Proprietarios;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFLApplication.Utilities;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.ApiWrappers
{
    /// <summary>
    /// Wrapper de Inquilinos
    /// </summary>
    public class WrapperInquilinos : IInquilinoService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperInquilinos> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        readonly IProprietarioService _svcProprietarios;
        readonly IImovelService _svcImoveis;
        readonly IFracaoService _svcFracoes;
        readonly IMailMergeService _MailMergeSvc;

        private ArrendamentoVM? _arrendamento;

        /// <summary>
        /// Tenant's service constructor
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        /// <param name="mapper"></param>
        /// <param name="svcProprietarios"></param>
        /// <param name="svcImoveis"></param>
        /// <param name="svcFracoes"></param>
        /// <param name="mailMergeSvc"></param>
        /// <param name="svcInquilinos"></param>
        public WrapperInquilinos(IConfiguration env,
                                 ILogger<WrapperInquilinos> logger,
                                 HttpClient httpClient,
                                 IMapper mapper,
                                 IProprietarioService svcProprietarios,
                                 IImovelService svcImoveis,
                                 IFracaoService svcFracoes,
                                 IMailMergeService mailMergeSvc)
        {
            _httpClient = httpClient;
            _env = env;
            _logger = logger;
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Inquilinos";
            _mapper = mapper;
            _svcProprietarios = svcProprietarios;
            _svcImoveis = svcImoveis;
            _svcFracoes = svcFracoes;
            _MailMergeSvc = mailMergeSvc;
        }

        /// <summary>
        /// Insert new tenant
        /// </summary>
        /// <param name="inquilino"></param>
        /// <returns></returns>
        public async Task<bool> InsereInquilino(InquilinoVM inquilino)
        {
            _logger.LogInformation("Criação de Inquilino no API");
            try
            {
                var tenantToInsert = _mapper.Map<NovoInquilino>(inquilino);
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereInquilino", tenantToInsert))
                {
                    return result.IsSuccessStatusCode;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Inquilino {exc.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update tenant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inquilino"></param>
        /// <returns></returns>
        public async Task<bool> AtualizaInquilino(int id, InquilinoVM inquilino)
        {
            try
            {
                var tenantToUpdate = _mapper.Map<AlteraInquilino>(inquilino);
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraInquilino/{id}", tenantToUpdate))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.Message, "Erro ao atualizar Inquilino)");
                return false;
            }
        }

        /// <summary>
        /// Delete tenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ApagaInquilino(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaInquilino/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Inquilino)");
                return false;
            }
        }

        /// <summary>
        /// Delete tenant
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ApagaDocumentoInquilino(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaDocumentoInquilino/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar documento do Inquilino)");
                return false;
            }
        }

        public async Task AtualizaSaldo(int IdInquilino, decimal decSaldoCorrente)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.GetAsync($"{_uri}/AtualizaSaldo/{IdInquilino}/{decSaldoCorrente}"))
                {
                    var success = result.IsSuccessStatusCode;
                    //return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Inquilino)");
                //return false;
            }
        }

        /// <summary>
        /// Get all inquilinos
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<InquilinoVM>> GetAll()
        {
            try
            {
                var tenants = await _httpClient.GetFromJsonAsync<IEnumerable<InquilinoVM>>($"{_uri}/GetInquilinos");
                return tenants!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return Enumerable.Empty<InquilinoVM>();
            }
        }
        /// <summary>
        /// Get tenant from api
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<InquilinoVM?> Query_ById(int id)
        {
            try
            {
                var tenant = await _httpClient.GetFromJsonAsync<InquilinoVM>($"{_uri}/GetInquilinoById/{id}");
                return tenant;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }


        public int GetFirstIdInquilino()
        {
            throw new NotImplementedException();
        }

        public int GetFirstId_Inquilino()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetInquilinoFracao(int ID_Fracao)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LookupTableVM>> GetInquilinosDisponiveis()
        {
            try
            {
                var output = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetInquilinosDisponiveis");
                return output!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetInquilinosAsLookup()
        {
            try
            {
                var output = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetInquilinosAsLookup");
                return output!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }


        public async Task<IEnumerable<LookupTableVM>> GetInquilinos()
        {
            try
            {
                var output = await _httpClient.GetFromJsonAsync<IEnumerable<LookupTableVM>>($"{_uri}/GetInquilinos");
                return output!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }

        }

        public string GetNomeFracao(int IdInquilino, bool bTitular)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetNomeInquilino(int Id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetNomeInquilino/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result;
                }

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "";
            }

        }

        /// <summary>
        /// Última renda paga pelo Inquilino
        /// </summary>
        /// <param name="ID_Inquilino"></param>
        /// <returns></returns>
        public async Task<string?> GetUltimoMesPago_Inquilino(int ID_Inquilino)
        {
            _logger.LogInformation("Último mês pago pelo inquilino");
            try
            {
                var ultimoMesPago = await _httpClient.GetStringAsync($"{_uri}/GetUltimoMesPago_Inquilino/{ID_Inquilino}");
                var result = ultimoMesPago.Substring(1, ultimoMesPago.Length - 1).Replace("\\\\", "\\");
                result = result.Substring(0, result.Length - 1);

                return result;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return "";
            }
        }

        public IEnumerable<Inquilino> Query(string where = "")
        {
            throw new NotImplementedException();
        }

        public string RegistoComErros(Inquilino inquilino)
        {
            throw new NotImplementedException();
        }

        public bool TableHasData()
        {
            throw new NotImplementedException();
        }

        public string UltimoMesPago(int IdInquilino)
        {
            throw new NotImplementedException();
        }


        public async Task<InquilinoVM> GetInquilino_ById(int id)
        {
            try
            {
                var tenant = await _httpClient.GetFromJsonAsync<InquilinoVM>($"{_uri}/GetInquilinoById/{id}");
                return tenant;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        public async Task<int> CriaDocumentoInquilino(DocumentoInquilinoVM documento)
        {
            try
            {
                var documentToInsert = _mapper.Map<NovoDocumentoInquilino>(documento);
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereDocumento", documentToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return success ? 1 : -1;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar documento {exc.Message}");
                return -1;
            }
        }

        public async Task<DocumentoInquilinoVM> GetDocumentoById(int id)
        {
            try
            {
                var document = await _httpClient.GetFromJsonAsync<DocumentoInquilinoVM>($"{_uri}/GetDocumentoInquilinoById/{id}");
                return document!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return new(); // Enumerable.Empty<DocumentoInquilinoVM>();
            }
        }

        public async Task<IEnumerable<DocumentoInquilinoVM>> GetDocumentos()
        {
            try
            {
                var tenants = await _httpClient.GetFromJsonAsync<IEnumerable<DocumentoInquilinoVM>>($"{_uri}/GetDocumentosInquilinos");
                return tenants!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return Enumerable.Empty<DocumentoInquilinoVM>();
            }
        }
        public async Task<IEnumerable<DocumentoInquilinoVM>> GetDocumentosInquilino(int id)
        {
            try
            {
                var tenants = await _httpClient.GetFromJsonAsync<IEnumerable<DocumentoInquilinoVM>>($"{_uri}/GetDocumentosInquilino/{id}");
                return tenants!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (GetDocumentosInquilino)");
                return Enumerable.Empty<DocumentoInquilinoVM>();
            }
        }

        public async Task<bool> AtualizaDocumentoInquilino(int id, DocumentoInquilinoVM document)
        {
            try
            {
                var tenantDocumentToUpdate = _mapper.Map<AlteraDocumentoInquilino>(document);
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraDocumentoInquilino/{id}", tenantDocumentToUpdate))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar documento do Inquilino)");
                return false;
            }
        }

        public async Task<string> GetServerPdfFileName(string foldername, string filename)
        {
            var fileUrl = $"{_env["BaseUrl"]}/serverpdf/GetServerPdfName/{foldername}/{filename}";
            try
            {

                HttpResponseMessage response = await _httpClient.GetAsync(fileUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    result = result.Substring(1, result.Length - 1).Replace("\\\\", "\\");
                    result = result.Substring(0, result.Length - 1);
                    return result;
                }

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "";
            }
        }
        public async Task<string> GetPdfFromServer(string foldername, string filename)
        {
            var fileUrl = $"{_env["BaseUrl"]}/serverpdf/download/{foldername}/{filename}";
            try
            {

                HttpResponseMessage response = await _httpClient.GetAsync(fileUrl);
                if (response.IsSuccessStatusCode)
                {
                    var requestUri = response.RequestMessage?.RequestUri;
                    return requestUri!.AbsoluteUri; // requestUri.AbsolutePath ??
                }

                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return "";
            }
        }

        public Task<IEnumerable<LookupTableVM>> GetInquilinos_Inquilinos(bool titular)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FiadorVM>> GeFiadorInquilino_ById(int idInquilino)
        {
            {
                try
                {
                    var fiadores = await _httpClient.GetFromJsonAsync<IEnumerable<FiadorVM>>($"{_uri}/GetFiadorInquilinoById/{idInquilino}");
                    return fiadores;
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc, "Erro ao pesquisar API");
                    return null;
                }
            }
        }

        public async Task<IEnumerable<CC_InquilinoVM>> GetTenantPaymentsHistory(int idInquilino)
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/TenantPaymentsHistory/{idInquilino}"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return new List<CC_InquilinoVM>();
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var tenantPaymentsHistory = JsonConvert.DeserializeObject<IEnumerable<CC_InquilinoVM>>(jsonData);
                        return tenantPaymentsHistory?.ToList() ?? new List<CC_InquilinoVM>();
                    }
                    else
                    {
                        _logger.LogError("Erro ao ler dados (TenantPaymentsHistory)");
                        return new List<CC_InquilinoVM>();
                    }
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return new List<CC_InquilinoVM>();
            }

        }

        public async Task<IEnumerable<LookupTableVM>> GetInquilinos_SemContrato()
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetInquilinosSemContrato"))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return new List<LookupTableVM>();
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var tenantsWithNoLease = JsonConvert.DeserializeObject<IEnumerable<LookupTableVM>>(jsonData);
                        return tenantsWithNoLease?.ToList() ?? new List<LookupTableVM>();
                    }
                    else
                    {
                        _logger.LogError("Erro ao ler dados (TenantPaymentsHistory)");
                        return new List<LookupTableVM>();
                    }
                }

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return new List<LookupTableVM>();
            }
        }

        public async Task<string> AtualizaRendaInquilino(int Id)
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/UpdateTenantRent/{Id}"))
                {
                    var jsonData = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return "Renda atualizada com sucesso";
                    else
                        return jsonData.Trim('"');
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return "Erro ao pesquisar API"; ;
            }
        }

        public async Task<string> AtualizaRendaInquilino_Manual(int Id, string oldValue, string newValue)
        {
            try
            {

                oldValue = oldValue.ToString().Replace('.', ',');
                newValue = newValue.ToString().Replace('.', ',');

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/UpdateTenantRent_Manually/{Id}/{oldValue}/{newValue}"))
                {
                    var jsonData = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                        return "";
                    else
                        return jsonData.Trim('"');
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API ({exc.ToString()})");
                return "Erro ao pesquisar API"; ;
            }
        }

        public async Task<decimal> GetTenantRent(int Id)
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetTenantRent/{Id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var tenantRent = JsonConvert.DeserializeObject<decimal>(jsonData);
                        return tenantRent;
                    }
                    else
                        return -1;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (GetTenantRent");
                return -1;
            }
        }

        public async Task<bool> PriorRentUpdates_ThisYear(int tenantId)
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/CheckForPriorRentUpdates_ThisYear/{tenantId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var updateAlreadyMade = JsonConvert.DeserializeObject<bool>(jsonData);
                        return updateAlreadyMade;
                    }
                    else
                        return false;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API (PriorRentUpdates_ThisYear ({exc.Message})");
                return false;
            }
        }

        public async Task<IEnumerable<HistoricoAtualizacaoRendasVM>> GetAllRentUpdates()
        {
            try
            {

                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/RentUpdates"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var rentUpdates = JsonConvert.DeserializeObject<IEnumerable<HistoricoAtualizacaoRendasVM>>(jsonData);
                        return rentUpdates;
                    }
                    else
                        return null;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API (RentUpdates) ({exc.Message})");
                return null;
            }
        }

        public async Task<IEnumerable<HistoricoAtualizacaoRendasVM>> GetRentUpdates_ByTenantId(int tenantId)
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/RentUpdates/{tenantId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var tenantRentUpdates = JsonConvert.DeserializeObject<IEnumerable<HistoricoAtualizacaoRendasVM>>(jsonData);
                        return tenantRentUpdates;
                    }
                    else
                        return null;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API (RentUpdates_ByTenantId) ({exc.Message})");
                return null;
            }
        }

        public async Task<IEnumerable<RentAdjustmentsVM>?> GetRentAdjustments()
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/GetRentAdjustments"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var tenantsRentAdjustments = JsonConvert.DeserializeObject<IEnumerable<RentAdjustmentsVM>>(jsonData);
                        if (tenantsRentAdjustments != null)
                            return tenantsRentAdjustments;
                        else
                            return Enumerable.Empty<RentAdjustmentsVM>();
                    }
                    else
                        return Enumerable.Empty<RentAdjustmentsVM>();
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API (GetRentAdjustments) ({exc.Message})");
                return Enumerable.Empty<RentAdjustmentsVM>();
            }
        }

        public async Task<IEnumerable<LatePaymentLettersVM>> GetLatePaymentLetters()
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/LatePaymentLetters"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var tenantsLatePaymentLetters = JsonConvert.DeserializeObject<IEnumerable<LatePaymentLettersVM>>(jsonData);
                        if (tenantsLatePaymentLetters != null)
                        {
                            return tenantsLatePaymentLetters;
                        }
                        else
                        {
                            return Enumerable.Empty<LatePaymentLettersVM>();
                        }
                    }
                    else
                        return Enumerable.Empty<LatePaymentLettersVM>();
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API (GetLatePaymentLetters) ({exc.Message})");
                return Enumerable.Empty<LatePaymentLettersVM>();
            }
        }

        public async Task<CartaAtualizacao> GetDadosCartaAtualizacaoInquilino(ArrendamentoVM DadosArrendamento, CoeficienteAtualizacaoRendas coefficientData)
        {
            _arrendamento = DadosArrendamento;

            try
            {
                int IdProprietario = await _svcProprietarios.GetFirstId(); // nesta versão da aplicação, só existe um proprietário...
                ProprietarioVM DadosProprietario = await _svcProprietarios.GetProprietario_ById(IdProprietario);
                InquilinoVM DadosInquilino = await GetInquilino_ById(DadosArrendamento.ID_Inquilino);
                FracaoVM DadosFracao = await _svcFracoes.GetFracao_ById(DadosArrendamento.ID_Fracao);
                ImovelVM DadosImovel = await _svcImoveis.GetImovel_ById(DadosFracao.Id_Imovel);

                var moradaImovel = $"{DadosImovel.Morada}, {DadosImovel.Numero}  {DadosFracao.Andar}  {DadosFracao.Lado} {DadosImovel.CodPst} {DadosImovel.CodPstEx} {DadosImovel.FreguesiaImovel}";
                var moradaFracao = $"{DadosImovel.Morada}, {DadosImovel.Numero}  {DadosFracao.Andar}  {DadosFracao.Lado}";
                var moradaInquilino = DadosInquilino.Morada;
                var DiaAPartirDe = "01";
                var MesAPartirDe = DadosArrendamento.Data_Inicio.AddMonths(1).ToString("MMMM").ToTitleCase();
                var AnoAtualizacao = DateTime.Now.Year.ToString();
                var diplomaLegal = coefficientData?.DiplomaLegal?.Trim();
                var percAumento = $"{coefficientData?.Coeficiente * 100 - 100:F2}%";
                CartaAtualizacao dadosAtualizacaoRenda = new CartaAtualizacao()
                {
                    Id = DadosArrendamento.Id,
                    LocalEmissao = DadosImovel.ConcelhoImovel,
                    DataEmissao = DateTime.Now,
                    AnoAPartirDe = AnoAtualizacao,
                    MesAPartirDe = MesAPartirDe,
                    DiaAPartirDe = DiaAPartirDe,
                    AnoAtualizacao = AnoAtualizacao,
                    Coeficiente = percAumento,
                    MatrizPredial = DadosFracao.Matriz,
                    MoradaInquilino =moradaInquilino, // moradaImovel,                   
                    Naturalidade = DadosInquilino.Naturalidade,
                    MoradaFracao = moradaFracao,
                    Nome = DadosProprietario.Nome,
                    Morada = DadosProprietario.Morada,
                    CodigoPostal = DadosProprietario.CodPostal,
                    Lei = diplomaLegal,
                    DataPublicacao = coefficientData?.DataPublicacao

                };

                return dadosAtualizacaoRenda;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<string> EmiteCartaAtualizacaoInquilino(CartaAtualizacao DadosAtualizacao)
        {
            string[] aCampos = new string[] {
                "LocalEmissao", "DataEmissao", 
                "NomeSenhorio", "MoradaSenhorio", "CodigoPostalSenhorio",
                "NomeInquilino", "MoradaInquilino", "CodigoPostalInquilino",
                "MoradaFracao",
                "Coeficiente", "Lei", "DataPublicacao",
                "MatrizPredial",
                "ValorRenda", "NovoValorRenda", "ExtensoNovoValor",
                "DiaAPartirDe", "MesAPartirDe",  "AnoAtualizacao",
                "AnoPublicacao"
            };

            string[] aDados = new string[]
            {
                DadosAtualizacao.LocalEmissao,
                DadosAtualizacao.DataEmissao.ToLongDateString().ToTitleCase(),
                DadosAtualizacao.Nome,
                DadosAtualizacao.Morada,
                DadosAtualizacao.CodigoPostal,
                DadosAtualizacao.NomeInquilino,
                DadosAtualizacao.MoradaInquilino,
                DadosAtualizacao.Naturalidade, // Código postal inquilino
                DadosAtualizacao.MoradaFracao,
                DadosAtualizacao.Coeficiente,
                DadosAtualizacao.Lei,
                DadosAtualizacao.DataPublicacao,
                DadosAtualizacao.MatrizPredial,
                DadosAtualizacao.ValorRenda.ToString("C2"),
                DadosAtualizacao.NovoValorRenda.ToString("C2"),
                DadosAtualizacao.NovoValorExtenso = Utilitarios.ValorPorExtenso(DadosAtualizacao.NovoValorRenda),
                DadosAtualizacao.DiaAPartirDe,
                DadosAtualizacao.MesAPartirDe,
                DadosAtualizacao.AnoAPartirDe,
                DateTime.Now.Year.ToString()
            };

            var mergeModel = new MailMergeModel()
            {
                CodContrato = _arrendamento.Id,
                TipoDocumentoEmitido = DocumentoEmitido.AtualizacaoRendas,
                DocumentHeader = "",
                MergeFields = aCampos,
                ValuesFields = aDados,
                WordDocument = "CartaAtualizacaoRendasManual.dotx",
                SaveFile = true,
                Referral = true
            };

            var docGerado = await _MailMergeSvc.MailMergeLetter(mergeModel);
            docGerado = Path.GetFileName(docGerado).Replace("/", "");

            return docGerado;

        }

        public async Task<bool> CriaCartaAtualizacaoInquilinoDocumentosInquilino(int tenantId, string docGerado)
        {
            var endpoint = $"{_uri}/CreateUpdateLetterDocument/{tenantId}?docGerado={docGerado}";
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync(endpoint))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var creationOk = JsonConvert.DeserializeObject<bool>(jsonData);
                        return creationOk;
                    }
                    else
                        return false;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API (RentUpdates_ByTenantId) ({exc.Message})");
                return false;
            }
        }

        public async Task<IEnumerable<LatePaymentLettersVM>> GetRentUpdateLetters()
        {
            // RentUpdateLetters
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/RentUpdateLetters"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var rentUpdateLetters = JsonConvert.DeserializeObject<IEnumerable<LatePaymentLettersVM>>(jsonData);
                        if (rentUpdateLetters != null)
                        {
                            return rentUpdateLetters;
                        }
                        else
                        {
                            return Enumerable.Empty<LatePaymentLettersVM>();
                        }
                    }
                    else
                        return Enumerable.Empty<LatePaymentLettersVM>();
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao pesquisar API (GetRentUpdateLetters) ({exc.Message})");
                return Enumerable.Empty<LatePaymentLettersVM>();
            }
        }

        public async Task<IEnumerable<RecebimentoVM>> GetTenantPayments(int tenantId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/TenantPayments/{tenantId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var output = JsonConvert.DeserializeObject<IEnumerable<RecebimentoVM>>(data);
                    return output;
                }

                return null;

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API Recebimentos / GetTenantPayments");
                return null;
            }
        }

    }
}
