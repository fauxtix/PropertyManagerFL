using AutoMapper;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.MailMerge;
using PropertyManagerFL.Application.ViewModels.Proprietarios;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFLApplication.Utilities;
using System.Text;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.ApiWrappers
{
    /// <summary>
    /// Wrapper do Api de Arrendamento s
    /// </summary>
    public class WrapperArrendamentos : IArrendamentoService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperArrendamentos> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;


        readonly IProprietarioService _svcProprietarios;
        readonly IInquilinoService _svcInquilinos;
        readonly IImovelService _svcImoveis;
        readonly IFracaoService _svcFracoes;
        readonly IMailMergeService _MailMergeSvc;
        readonly IRecebimentoService _svcRecebimentos;

        private ArrendamentoVM? _arrendamento;


        /// <summary>
        /// Wrapper para Api de arrendamentos
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        /// <param name="mapper"></param>
        /// <param name="svcProprietarios"></param>
        /// <param name="svcInquilinos"></param>
        /// <param name="svcImoveis"></param>
        /// <param name="svcFracoes"></param>
        /// <param name="mailMergeSvc"></param>
        /// <param name="svcRecebimentos"></param>
        /// <param name="environment"></param>
        public WrapperArrendamentos(IConfiguration env,
                                    ILogger<WrapperArrendamentos> logger,
                                    HttpClient httpClient,
                                    IMapper mapper,
                                    IProprietarioService svcProprietarios,
                                    IInquilinoService svcInquilinos,
                                    IImovelService svcImoveis,
                                    IFracaoService svcFracoes,
                                    IMailMergeService mailMergeSvc,
                                    IRecebimentoService svcRecebimentos,
                                    IWebHostEnvironment environment)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Arrendamentos";

            _logger = logger;
            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("ApiKey", _env["ApiKey"]);


            _mapper = mapper;
            _svcProprietarios = svcProprietarios;
            _svcInquilinos = svcInquilinos;
            _svcImoveis = svcImoveis;
            _svcFracoes = svcFracoes;
            _MailMergeSvc = mailMergeSvc;
            _svcRecebimentos = svcRecebimentos;
            _environment = environment;
        }

        /// <summary>
        /// Cria arrendamento
        /// </summary>
        /// <param name="arrendamento"></param>
        /// <returns></returns>
        public async Task<bool> InsertArrendamento(ArrendamentoVM arrendamento)
        {
            if (DateTime.Now.Day < 8)
            {
                arrendamento.Data_Pagamento = arrendamento.Data_Pagamento.AddDays(-1);
            }

            try
            {

                arrendamento.EstadoPagamento = "Pago";

                var leaseToInsert = _mapper.Map<NovoArrendamento>(arrendamento);

                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsertArrendamento", leaseToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Arrendamento {exc.Message}");
                return false;
            }
        }

        /// <summary>
        /// Atualiza arrendamento
        /// </summary>
        /// <param name="id"></param>
        /// <param name="arrendamento"></param>
        /// <returns></returns>
        public async Task<bool> UpdateArrendamento(int id, ArrendamentoVM arrendamento)
        {
            try
            {
                var leaseToUpdate = _mapper.Map<AlteraArrendamento>(arrendamento);

                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/UpdateArrendamento/{id}", leaseToUpdate))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Arrendamento");
                return false;
            }
        }

        /// <summary>
        /// Apaga arrendamento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteArrendamento(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/DeleteArrendamento/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Arrendamento)");
                return false;
            }
        }


        /// <summary>
        /// Existe arrendamento?
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>

        public async Task<bool> ArrendamentoExiste(int unitId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_uri}/ArrendamentoExiste/{unitId}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var leaseExist = JsonConvert.DeserializeObject<bool>(data);
                    return leaseExist;
                }

                return true;

            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return true;
            }
        }

        public async Task CheckNewRents()
        {
            using (HttpResponseMessage result = await _httpClient.GetAsync($"{_uri}/CheckNewRents"))
            {
                var success = Results.NoContent();
            }
        }

        public async Task<bool> ChildrenExists(int IdFracao)
        {
            try
            {
                bool hasPayments = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/ChildrenExists/{IdFracao}");
                return hasPayments;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return true;
            }

        }

        public async Task<bool> ContratoEmitido(int id)
        {
            try
            {
                bool ctrEmitido = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/ContratoEmitido/{id}");
                return ctrEmitido;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return true;
            }

        }


        public void CriaRegistoHistorico(Arrendamento arrendamento)
        {
            throw new NotImplementedException();
        }


        public async Task GeraMovimentos(ArrendamentoVM arrendamento, int IdFracao)
        {
            try
            {
                arrendamento.Data_Pagamento = arrendamento.Data_Inicio.AddMonths(1);
                arrendamento.EstadoPagamento = "Pendente";

                var leaseToInsert = _mapper.Map<NovoArrendamento>(arrendamento);

                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/GeraMovimentos/{IdFracao}", leaseToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    //return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Arrendamento {exc.Message}");
                //return false;
            }
        }

        public async Task<string> GetDocumentoGerado(int Id)
        {
            var docGerado = await _httpClient.GetStringAsync($"{_uri}/GetDocumentoGerado/{Id}");
            return docGerado;
        }

        public async Task<string> GetNomeInquilino(int Id)
        {
            var nomeInquilino = await _httpClient.GetStringAsync($"{_uri}/GetNomeInquilino/{Id}");
            return nomeInquilino;
        }

        public async Task<int> GetIdInquilino_ByUnitId(int unitId)
        {
            var response = await _httpClient.GetAsync($"{_uri}/GetIdInquilino/{unitId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<int>(data);
                return output;
            }

            return -1;

        }

        public async Task<List<ArrendamentoVM>> GetResumedData()
        {
            throw new NotImplementedException();
        }

        public async Task MarcaContratoComoEmitido(int Id, string docGerado)
        {
            try
            {
                var fileName = Path.GetFileName(docGerado);
                var endpoint = $"{_uri}/MarcaContratoComoEmitido/{Id}/{fileName}";
                using (HttpResponseMessage result = await _httpClient.GetAsync(endpoint))
                {
                    var success = result.IsSuccessStatusCode;
                    //return success;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task MarcaContratoComoNaoEmitido(int Id)
        {
            using (HttpResponseMessage result = await _httpClient.GetAsync($"{_uri}/MarcaContratoComoNaoEmitido/{Id}"))
            {
                var success = result.IsSuccessStatusCode;
                //return success;
            }
        }

        public async Task<bool> RenovacaoAutomatica(int Id)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"{_uri}/RenovacaoAutomatica/{Id}");

        }

        public async Task<decimal> TotalRendas()
        {
            return await _httpClient.GetFromJsonAsync<decimal>($"{_uri}/TotalRendas");
        }

        /// <summary>
        /// Get arrendamento by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ArrendamentoVM> GetArrendamento_ById(int id)
        {
            try
            {
                var contact = await _httpClient.GetFromJsonAsync<ArrendamentoVM>($"{_uri}/GetArrendamento_ById/{id}");
                return contact!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return new ArrendamentoVM();
            }
        }

        public async Task<IEnumerable<ArrendamentoVM>> GetAll()
        {
            try
            {
                var leases = await _httpClient.GetFromJsonAsync<IEnumerable<ArrendamentoVM>>($"{_uri}/GetAll");
                return leases!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos / GetAll())");
                return Enumerable.Empty<ArrendamentoVM>();
            }
        }

        public async Task<int> GetLastId()
        {
            try
            {
                var Id = await _httpClient.GetFromJsonAsync<int>($"{_uri}/GetLastId");
                return Convert.ToInt32(Id);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return 0;
            }
        }

        public async Task<string> GetPdfFilename(string? filename)
        {
            try
            {
                //string fullPath = Path.Combine(_environment.ContentRootPath, "Reports", "Docs", "Contratos", filename!);
                //var fileName = fullPath.Replace("docx", "pdf");
                //if(File.Exists(fileName))
                //{
                //    return fileName;
                //}

                var response = await _httpClient.GetAsync($"{_uri}/GetPdfFilename/{filename}");
                if (response.IsSuccessStatusCode)
                {
                    var resultString = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(resultString) == false)
                    {
                        resultString = resultString.Replace("\"", "").Replace("\\\\", "\\");

                    }
                    return resultString;
                }

                return "";
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return "";
            }
        }

        public async Task<bool> UpdateLastPaymentDate(int id, DateTime date)
        {
            try
            {
                var result = await _httpClient.GetAsync($"{_uri}/UpdateLastPaymentDate/{id}/{date}");
                return result.IsSuccessStatusCode;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return false;
            }
        }

        public async Task<DateTime> GetLastPaymentDate(int unitId)
        {
            var response = await _httpClient.GetAsync($"{_uri}/GetLastPaymentDate/{unitId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<DateTime>(data);
                if (output != DateTime.MinValue)
                    return output;
                else
                    return DateTime.MinValue;
            }

            return DateTime.MinValue;
        }

        public async Task<float> GetCurrentRentCoefficient(string? ano)
        {
            var response = await _httpClient.GetAsync($"{_uri}/GetCurrentRentCoefficient/{ano}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<float>(data);
                if (output != -1)
                    return output;
                else
                    return -1;
            }

            return -1;
        }

        public async Task<CartaAtualizacao> GetDadosCartaAtualizacao(ArrendamentoVM DadosArrendamento)
        {
            _arrendamento = DadosArrendamento;

            try
            {
                int IdProprietario = await _svcProprietarios.GetFirstId(); // nesta versão da aplicação, só existe um proprietário...
                ProprietarioVM DadosProprietario = await _svcProprietarios.GetProprietario_ById(IdProprietario);

                InquilinoVM DadosInquilino = await _svcInquilinos.GetInquilino_ById(_arrendamento.ID_Inquilino);

                FracaoVM DadosFracao = await _svcFracoes.GetFracao_ById(_arrendamento.ID_Fracao);
                ImovelVM DadosImovel = await _svcImoveis.GetImovel_ById(DadosFracao.Id_Imovel);

                var moradaImovel = $"{DadosImovel.Morada}, {DadosImovel.Numero}  {DadosFracao.Andar}  {DadosFracao.Lado} {DadosImovel.CodPst} {DadosImovel.CodPstEx} {DadosImovel.FreguesiaImovel}";
                var moradaFracao = $"{DadosImovel.Morada}, {DadosImovel.Numero}  {DadosFracao.Andar}  {DadosFracao.Lado}";

                var currentYearAsString = DateTime.Now.Year.ToString();
                var coeffRecord = await GetRentCoefficient_ByYear(DateTime.Now.Year);

                var LeiVigente = coeffRecord.Lei;
                var DataPublicacao = coeffRecord.DataPublicacao;
                float ValorCoeficiente = coeffRecord.Coeficiente;

                var ValorRenda = DadosFracao.ValorRenda;
                var NovoValorRenda = ValorRenda * ((decimal)ValorCoeficiente);
                var DiaAPartirDe = "01";
                var MesAPartirDe = DadosArrendamento.Data_Inicio.ToString("MMMM").ToTitleCase();
                var AnoAtualizacao = DateTime.Now.Year.ToString();


                CartaAtualizacao dadosAtualizacaoRenda = new CartaAtualizacao()
                {
                    Id = _arrendamento.Id,
                    LocalEmissao = DadosImovel.ConcelhoImovel,
                    DataEmissao = DateTime.Now,
                    AnoAPartirDe = AnoAtualizacao,
                    MesAPartirDe = MesAPartirDe,
                    DiaAPartirDe = DiaAPartirDe,
                    ValorRenda = ValorRenda,
                    NovoValorRenda = NovoValorRenda,
                    AnoAtualizacao = AnoAtualizacao,
                    Coeficiente = ValorCoeficiente.ToString(),
                    MatrizPredial = DadosFracao.Matriz,
                    NomeInquilino = DadosInquilino.Nome,
                    MoradaInquilino = moradaImovel,
                    MoradaFracao = moradaFracao,
                    Nome = DadosProprietario.Nome,
                    Morada = DadosProprietario.Morada,
                    Lei = LeiVigente,
                    DataPublicacao = DataPublicacao,

                };

                return dadosAtualizacaoRenda;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }


        public async Task<string> EmiteCartaAtualizacao(CartaAtualizacao DadosAtualizacao)
        {
            string[] aCampos = new string[] {
                "LocalEmissao", "DataEmissao", "NomeSenhorio", "MoradaSenhorio",
                "NomeInquilino", "MoradaInquilino",
                "MoradaFracao",
                "Coeficiente", "Lei", "DataPublicacao",
                "MatrizPredial",
                "ValorRenda", "NovoValorRenda",
                "DiaAPartirDe", "MesAPartirDe",  "AnoAtualizacao",
            };

            string[] aDados = new string[]
            {
                DadosAtualizacao.LocalEmissao,
                DadosAtualizacao.DataEmissao.ToLongDateString().ToTitleCase(),
                DadosAtualizacao.Nome,
                DadosAtualizacao.Morada,
                DadosAtualizacao.NomeInquilino,
                DadosAtualizacao.MoradaInquilino,
                DadosAtualizacao.MoradaFracao,
                DadosAtualizacao.Coeficiente,
                DadosAtualizacao.Lei,
                DadosAtualizacao.DataPublicacao,
                DadosAtualizacao.MatrizPredial,
                DadosAtualizacao.ValorRenda.ToString("C2"),
                DadosAtualizacao.NovoValorRenda.ToString("C2"),
                DadosAtualizacao.DiaAPartirDe,
                DadosAtualizacao.MesAPartirDe,
                DadosAtualizacao.AnoAPartirDe
            };

            var mergeModel = new MailMergeModel()
            {
                CodContrato = _arrendamento.Id,
                TipoDocumentoEmitido = DocumentoEmitido.AtualizacaoRendas,
                DocumentHeader = "",
                MergeFields = aCampos,
                ValuesFields = aDados,
                WordDocument = "CartaAtualizacaoRendas.dotx",
                SaveFile = true,
                Referral = true
            };

            string docGerado = await _MailMergeSvc.MailMergeLetter(mergeModel);


            return docGerado;

        }

        public async Task<CartaOposicaoRenovacaoContrato> GetDadosCartaOposicaoRenovacaoContrato(ArrendamentoVM DadosArrendamento)
        {
            _arrendamento = DadosArrendamento;

            try
            {
                int IdProprietario = await _svcProprietarios.GetFirstId(); // nesta versão da aplicação, só existe um proprietário...
                ProprietarioVM DadosProprietario = await _svcProprietarios.GetProprietario_ById(IdProprietario);

                InquilinoVM DadosInquilino = await _svcInquilinos.GetInquilino_ById(_arrendamento.ID_Inquilino);

                FracaoVM DadosFracao = await _svcFracoes.GetFracao_ById(_arrendamento.ID_Fracao);
                ImovelVM DadosImovel = await _svcImoveis.GetImovel_ById(DadosFracao.Id_Imovel);

                var moradaFracao = $"{DadosImovel.Morada}, {DadosImovel.Numero}  {DadosFracao.Andar}  {DadosFracao.Lado}";

                CartaOposicaoRenovacaoContrato dadosCartaOposicao = new CartaOposicaoRenovacaoContrato()
                {
                    Id = _arrendamento.Id,
                    MoradaFracao = moradaFracao,
                    LocalEmissao = DadosImovel.ConcelhoImovel,
                    DataEmissao = DateTime.Now,
                    NomeInquilino = DadosInquilino.Nome,
                    MoradaInquilino = DadosInquilino.Morada,
                    Nome = DadosProprietario.Nome,
                    Morada = DadosProprietario.Morada,
                    InicioContrato = _arrendamento.Data_Inicio,
                    FimContrato = _arrendamento.Data_Saida
                };

                return dadosCartaOposicao;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<string> EmiteCartaOposicaoRenovacaoContrato(CartaOposicaoRenovacaoContrato DadosCartaOposicao)
        {
            string[] aCampos = new string[] {
                "LocalEmissao", "DataEmissao",
                "NomeSenhorio", "MoradaSenhorio",
                "NomeInquilino", "MoradaInquilino",
                "MoradaFracao",
                "InicioContrato", "FimContrato"
            };

            string[] aDados = new string[]
            {
                DadosCartaOposicao.LocalEmissao,
                DadosCartaOposicao.DataEmissao.ToLongDateString().ToTitleCase(),
                DadosCartaOposicao.Nome,
                DadosCartaOposicao.Morada,
                DadosCartaOposicao.NomeInquilino,
                DadosCartaOposicao.MoradaInquilino,
                DadosCartaOposicao.MoradaFracao,
                DadosCartaOposicao.InicioContrato.ToShortDateString(),
                DadosCartaOposicao.FimContrato.ToShortDateString(),
            };

            var mergeModel = new MailMergeModel()
            {
                CodContrato = _arrendamento.Id,
                TipoDocumentoEmitido = DocumentoEmitido.OposicaoRenovacaoContrato,
                DocumentHeader = "",
                MergeFields = aCampos,
                ValuesFields = aDados,
                WordDocument = "RevogacaoContrato.dotx",
                SaveFile = true,
                Referral = true
            };

            string docGerado = await _MailMergeSvc.MailMergeLetter(mergeModel);


            return docGerado;

        }


        /// <summary>
        /// Obtém dados para o preenchimento da carta
        /// </summary>
        /// <param name="DadosArrendamento">ArrendamentoVM</param>
        /// <returns></returns>
        public async Task<CartaRendasAtraso> GetDadosCartaRendasAtraso(ArrendamentoVM DadosArrendamento)
        {
            _arrendamento = DadosArrendamento;
            string dueRentsAsString = "";
            try
            {
                int IdProprietario = await _svcProprietarios.GetFirstId(); // nesta versão da aplicação, só existe um proprietário...
                ProprietarioVM DadosProprietario = await _svcProprietarios.GetProprietario_ById(IdProprietario);

                InquilinoVM DadosInquilino = await _svcInquilinos.GetInquilino_ById(_arrendamento.ID_Inquilino);

                FracaoVM DadosFracao = await _svcFracoes.GetFracao_ById(_arrendamento.ID_Fracao);
                ImovelVM DadosImovel = await _svcImoveis.GetImovel_ById(DadosFracao.Id_Imovel);

                var tenantCurrentBalance = Convert.ToInt32(  Math.Abs(DadosInquilino.SaldoCorrente - DadosInquilino.SaldoPrevisto));
                int monthsDued = Convert.ToInt16( tenantCurrentBalance / DadosFracao.ValorRenda);


                var moradaFracao = $"{DadosImovel.Morada}, {DadosImovel.Numero}  {DadosFracao.Andar}  {DadosFracao.Lado}";
                //var rentMonthsCollected = await _svcRecebimentos.GetMonthlyRentsProcessed(DateTime.Now.Year);
                //var lastMonthCollect = rentMonthsCollected.Max(r => r.DataProcessamento).Month;
                //var lastMonthPaid = _arrendamento.Data_Pagamento.Month;
                //var monthsDued = lastMonthCollect - lastMonthPaid;
                var lastPaymentDate = _arrendamento.Data_Pagamento;
                string months = "";

                if (tenantCurrentBalance > 0 && monthsDued == 0)
                    monthsDued = 1;

                if (monthsDued == 1)
                {
                    months = $"{_arrendamento.Data_Pagamento.AddMonths(1).ToString("MMMM").ToTitleCase()}";
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < monthsDued; i++)
                    {
                        months += $"{_arrendamento.Data_Pagamento.AddMonths(i).ToString("MMMM").ToTitleCase()}, ";
                    }

                    months = months.Substring(0, months.Length - 2);
                    var lastComma = months.LastIndexOf(',');
                    if (lastComma != -1)
                        months = months.Remove(lastComma, 1).Insert(lastComma, " e");
                }

                //lastPaymentDateAsString = $"{_arrendamento.Data_Pagamento.ToString("MMMM").ToTitleCase()} de {lastPaymentDate.ToString("yyyy")}";
                dueRentsAsString = $"{months} de {lastPaymentDate.ToString("yyyy")}";

                CartaRendasAtraso dadosCartaRendasAtraso = new CartaRendasAtraso()
                {
                    Id = _arrendamento.Id,
                    LocalEmissao = DadosImovel.ConcelhoImovel,
                    DataEmissao = DateTime.Now,
                    NomeInquilino = DadosInquilino.Nome,
                    MoradaInquilino = moradaFracao,
                    Nome = DadosProprietario.Nome,
                    Morada = DadosProprietario.Morada,
                    PrazoEmDias = "10", // TODO - adaptar pazo em dias para resolver situação de atraso no pagto. renda (appsettings?)
                    RendasEmAtraso = dueRentsAsString,
                    MontanteRendasAtraso = DadosFracao.ValorRenda * monthsDued
                };

                return dadosCartaRendasAtraso;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Emite carta
        /// </summary>
        /// <param name="dadosCartaAtraso">classe 'CartaRendasAtraso'</param>
        /// <returns></returns>
        public async Task<string> EmiteCartaRendasAtraso(CartaRendasAtraso dadosCartaAtraso)
        {
            string[] aCampos = new string[] {
                "LocalEmissao", "DataEmissao",
                "NomeSenhorio", "MoradaSenhorio",
                "NomeInquilino", "MoradaInquilino",
                "MontanteRendasEmAtraso", "RendasEmAtraso",
                "PrazoEmDias"
            };

            string[] aDados = new string[]
            {
                dadosCartaAtraso.LocalEmissao,
                dadosCartaAtraso.DataEmissao.ToLongDateString().ToTitleCase(),
                dadosCartaAtraso.Nome,
                dadosCartaAtraso.Morada,
                dadosCartaAtraso.NomeInquilino,
                dadosCartaAtraso.MoradaInquilino,
                dadosCartaAtraso.MontanteRendasAtraso.ToString("##,###.00"),
                dadosCartaAtraso.RendasEmAtraso,
                dadosCartaAtraso.PrazoEmDias
            };

            var mergeModel = new MailMergeModel()
            {
                CodContrato = _arrendamento.Id,
                TipoDocumentoEmitido = DocumentoEmitido.RendasEmAtraso,
                DocumentHeader = "",
                MergeFields = aCampos,
                ValuesFields = aDados,
                WordDocument = "AvisoRendasEmAtraso.dotx",
                SaveFile = true,
                Referral = true
            };

            string docGerado = await _MailMergeSvc.MailMergeLetter(mergeModel);
            //var output = docGerado.Substring(0, docGerado.Length - 1); // ended with 2 "", remove the last

            return docGerado;

        }


        public async Task MarcaCartaAtualizacaoComoEmitida(int Id, string docGerado)
        {
            try
            {
                var fileName = Path.GetFileName(docGerado);
                var endpoint = $"{_uri}/MarcaCartaAtualizacaoComoEmitida/{Id}/{fileName}";
                using (HttpResponseMessage result = await _httpClient.GetAsync(endpoint))
                {
                    var success = result.IsSuccessStatusCode;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<bool> RegistaCartaOposicao(int Id, string docGerado)
        {
            try
            {
                var fileName = Path.GetFileName(docGerado);
                var endpoint = $"{_uri}/RegistaCartaOposicao/{Id}/{fileName}";
                using (HttpResponseMessage response = await _httpClient.GetAsync(endpoint))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var registerOk = JsonConvert.DeserializeObject<bool>(result);
                        return registerOk;
                    }
                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
        public async Task RegistaCartaAtrasoRendas(int Id, DateTime? referralDate, string tentiva, string docGerado)
        {
            try
            {
                var fileName = Path.GetFileName(docGerado);
                var referralDateAsString = referralDate!.Value.ToString("yyyy-MM-dd");
                var endpoint = $"{_uri}/RegistaCartaAtraso/{Id}/{referralDateAsString}/{tentiva}/{fileName}";
                using (HttpResponseMessage result = await _httpClient.GetAsync(endpoint))
                {
                    var success = result.IsSuccessStatusCode;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }


        public async Task<IEnumerable<CoeficienteAtualizacaoRendas>> GetRentUpdatingCoefficients()
        {
            try
            {
                var coefficients = await _httpClient.GetFromJsonAsync<IEnumerable<CoeficienteAtualizacaoRendas>>($"{_uri}/Get_RentCoefficients");
                return coefficients!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }

        public async Task<CoeficienteAtualizacaoRendas> GetRentUpdatingCoefficient_ById(int id)
        {
            try
            {
                var rentCoefficient = await _httpClient.GetFromJsonAsync<CoeficienteAtualizacaoRendas>($"{_uri}/Get_RentCoefficient_ById/{id}");
                return rentCoefficient!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return null;
            }
        }
        public async Task<CoeficienteAtualizacaoRendas> GetRentCoefficient_ByYear(int year)
        {
            try
            {
                var CoefficientRecord = await _httpClient.GetFromJsonAsync<CoeficienteAtualizacaoRendas>($"{_uri}/Get_RentCoefficient_ByYear/{year}");
                return CoefficientRecord!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (coeficientes");
                return null;
            }
        }

        public async Task<bool> InsertRentCoefficient(CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsertRentCoefficient", coeficienteAtualizacaoRendas))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Coeficiente {exc.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateRentCoefficient(int id, CoeficienteAtualizacaoRendas coeficienteAtualizacaoRendas)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/UpdateRentCoefficient/{id}", coeficienteAtualizacaoRendas))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Arrendamento");
                return false;
            }
        }

        public async Task<bool> CartaAtualizacaoRendasEmitida(int ano)
        {
            try
            {
                bool ctrEmitido = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/CartaAtualizacaoEmitida/{ano}");
                return ctrEmitido;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return true;
            }
        }

        public async Task<bool> VerificaSeExisteCartaRevogacao(int id)
        {
            try
            {
                bool wasLetterAlreadySent = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/VerificaSeExisteCartaRevogacao/{id}");
                return wasLetterAlreadySent;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return true;
            }
        }

        public async Task<bool> VerificaSeExisteRespostaCartaRevogacao(int id)
        {
            try
            {
                bool wasLetterAnswered = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/VerificaSeExisteRespostaCartaRevogacao/{id}");
                return wasLetterAnswered;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return false;
            }
        }

        public async Task<bool> RegistaProcessamentoAtualizacaoRendas()
        {
            try
            {
                var endpoint = $"{_uri}/RegistaCartaAtualizacaoRendas";
                using (HttpResponseMessage result = await _httpClient.GetAsync(endpoint))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> RequirementsMet()
        {
            try
            {
                var endpoint = $"{_uri}/RequirementsMet";
                using (HttpResponseMessage result = await _httpClient.GetAsync(endpoint))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task<bool> VerificaEnvioCartaAtrasoEfetuado(int id)
        {
            try
            {
                bool wasLetterAlreadySent = await _httpClient.GetFromJsonAsync<bool>($"{_uri}/VerificaEnvioCartaAtrasoRenda/{id}");
                return wasLetterAlreadySent;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API (Arrendamentos)");
                return false;
            }
        }

        public async Task<bool> RegistaCartaAtraso(int id, string docGerado)
        {
            try
            {
                var fileName = Path.GetFileName(docGerado);
                var endpoint = $"{_uri}/RegistaCartaAtraso/{id}/{fileName}";
                using (HttpResponseMessage response = await _httpClient.GetAsync(endpoint))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var registerOk = JsonConvert.DeserializeObject<bool>(result);
                        return registerOk;
                    }
                    else
                        return false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }

        public async Task MarcaCartaAtrasoRendaComoEmitida(int Id, string docGerado)
        {
            try
            {
                var fileName = Path.GetFileName(docGerado);
                var endpoint = $"{_uri}/MarcaCartaAtrasoRendaComoEmitida/{Id}/{fileName}";
                using (HttpResponseMessage result = await _httpClient.GetAsync(endpoint))
                {
                    var success = result.IsSuccessStatusCode;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao pesquisar API (Arrendamentos)");
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task<IEnumerable<LookupTableVM>> GetApplicableLaws()
        {
            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync($"{_uri}/ApplicableLaws"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonData = await response.Content.ReadAsStringAsync();
                        var output = JsonConvert.DeserializeObject<IEnumerable<LookupTableVM>>(jsonData);
                        return output?.ToList() ?? new List<LookupTableVM>();
                    }
                    else
                    {
                        _logger.LogError("Erro ao ler dados (GetApplicableLaws)");
                        return new List<LookupTableVM>();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new List<LookupTableVM>();
            }
        }

        public async Task<bool> ExtendLeaseTerm(int Id)
        {
            try
            {
                var endpoint = $"{_uri}/ExtendLeaseTerm/{Id}";
                using (HttpResponseMessage result = await _httpClient.GetAsync(endpoint))
                {
                    var success = result.IsSuccessStatusCode;
                    var tenantName = await _svcInquilinos.GetNomeInquilino(Id);
                    if (success)
                    {
                        _logger.LogInformation($"Estendido prazo do contrato para o Inquilino {tenantName}");
                    }
                    else
                    {
                        _logger.LogError($"Erro ao atualizar prazo do contrato (Inquilino: {tenantName})");
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return false;
            }
        }
    }
}

