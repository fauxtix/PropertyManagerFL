using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ObjectsComparer;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Contract;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Core.Entities;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class ArrendamentosBase : ComponentBase, IDisposable
    {
        [Inject] IWebHostEnvironment ?_env { get; set; }
        [Inject] public IArrendamentoService? arrendamentosService { get; set; }
        [Inject] public ILogger<ArrendamentosBase>? _logger { get; set; }
        [Inject] public IContratoService? contratosService { get; set; }
        [Inject] public IInquilinoService? inquilinosService { get; set; }
        [Inject] public IRecebimentoService? recebimentosService { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }
        [Inject] protected IStringLocalizer<App>? L{ get; set; }

        protected IEnumerable<ArrendamentoVM>? leases { get; set; }
        protected IEnumerable<ArrendamentoVM>? activeLeases { get; set; }

        protected IEnumerable<CoeficienteAtualizacaoRendas>? rentCoefficients { get; set; }
        protected ArrendamentoVM? SelectedLease { get; set; }
        protected ArrendamentoVM? OriginalLeaseData { get; set; }
        protected CoeficienteAtualizacaoRendas? SelectedRentCoefficient { get; set; }
        protected CoeficienteAtualizacaoRendas? OriginalRentCoefficientData { get; set; }
        protected OpcoesRegisto RecordMode { get; set; }

        protected int LeaseId { get; set; }
        protected int CoefficientId { get; set; }
        protected string? NewCaption { get; set; }
        protected string? EditCaption { get; set; }

        protected string? DeleteLeaseCaption;
        protected bool AddEditLeaseVisibility { get; set; }
        protected bool AddEditRentCoefficientVisibility { get; set; }
        protected bool DeleteLeaseVisibility { get; set; }
        protected bool ShowPdfVisibility { get; set; }

        protected bool ShowToolbarDueRentLetter { get; set; }
        protected bool ShowToolbaContractRevocationLetter { get; set; }
        protected bool ShowToolbaContractLeaseTerm { get; set; }
        protected bool ShowToolbarViewContract { get; set; }

        protected bool IsDirty = false;
        protected List<string> ValidationMessages = new();

        protected string? alertTitle = "";
        protected bool AlertVisibility { get; set; } = false;
        protected string? WarningMessage { get; set; }

        protected bool ErrorVisibility { get; set; } = false;
        protected bool IssueRentalContractVisibility { get; set; } = false;
        public bool LeaseTermVisibility { get; set; } = false;
        protected string? LeaseTermCaption { get; set; } = "";
        protected string? LeaseTermCaptionNote { get; set; } = "";

        protected SfGrid<ArrendamentoVM>? leasesGridObj { get; set; }
        protected SfGrid<CoeficienteAtualizacaoRendas>? coefsGridObj { get; set; }
        public DialogEffect Effect = DialogEffect.Zoom;
        protected SfSpinner? SpinnerObj { get; set; }
        protected SfToast? ToastObj { get; set; }

        public decimal SaldoCorrenteInquilino { get; set; }

        protected Modules? modulo { get; set; }
        protected AlertMessageType alertMessageType = AlertMessageType.Warning;

        protected DocumentoEmitido SendingLetterType { get; set; }
        protected bool SendLetterDialogVisibility { get; set; } = false;

        protected string? PdfFilePath { get; set; }
        protected bool TableHasData { get; set; }
        protected string? ToastTitle;
        protected string? ToastMessage;
        protected string? ToastCss;

        protected DateTime? DataAtualizacaoRenda { get; set; }
        protected bool SpinnerVisibility { get; set; } = false;
        protected Dictionary<string, int> LeaseAlerts = new Dictionary<string, int>();
        protected override async Task OnInitializedAsync()
        {
            ToastTitle = "";
            ToastMessage = "";
            ToastCss = "e-toast-success";

            ShowPdfVisibility = false;
            PdfFilePath = "";

            HideToolbar_LetterOptions();

            OriginalLeaseData = new();
            AddEditLeaseVisibility = false;
            DeleteLeaseVisibility = false;
            LeaseId = 0;
            IsDirty = false;

            // Arrendamentos
            leases = await GetAllLeases();

            // Coeficientes de atualização
            rentCoefficients = await GetAllRentCoefficients();

            TableHasData = leases.Count() > 0 || leases is not null;

            // Verificar se há alertas para mostrar
            await CheckForAlerts();
        }

        private async Task CheckForAlerts()
        {
            LeaseAlerts.Clear();
            var tenantDocuments = await inquilinosService!.GetDocumentos();
            var updateLetterSentCurrentYear = tenantDocuments.Where(td => td.DocumentType == 16 && td.UploadDate.Year < DateTime.Now.Year);
            if(updateLetterSentCurrentYear.Any())
            {
                foreach (var document in tenantDocuments)
                {
                    LeaseAlerts.Add($"Necessário envio de carta de atualização ao inquilino {document.NomeInquilino}", 1);

                }
            }

            var leasesWhoNeedToUpdateRent =
                leases.Where(l => l.Data_Inicio.Month == DateTime.Now.Month + 1);
            if (leasesWhoNeedToUpdateRent.Any())
            {
                foreach (var item in leasesWhoNeedToUpdateRent)
                {
                    var alertMsg = $"Necessário atualizar renda do inquilino {item.NomeInquilino} ({item.Fracao})";
                    if (item.EnvioCartaAtualizacaoRenda == false)
                        alertMsg += " - não foi enviada carta de atualização!";

                    LeaseAlerts.Add(alertMsg, 1);
                }
            }

            foreach (var _leaseitem in leases)
            {
                var monthsToEnd = GetMonthDifference(DateTime.Now, _leaseitem.Data_Fim);
                if (monthsToEnd >= 0 && monthsToEnd <= 4)
                {
                    LeaseAlerts.Add($"Contrato do inquilino {_leaseitem.NomeInquilino} ({_leaseitem.Fracao}) está prestes a terminar. Renovar data-fim, ou enviar carta de revogação", 2);
                }
            }
            if (leases.Count() > 0)
            {
                var rentPayments = (await recebimentosService!.GetAll()).ToList().Count();
                if (rentPayments > 0)
                {
                    var UpdateLetterSent = await arrendamentosService!.CartaAtualizacaoRendasEmitida(DateTime.Now.Year);
                    if (UpdateLetterSent == false)
                    {
                        LeaseAlerts.Add("Cartas de atualização de rendas não foram emitidas para o ano corrente!! Diploma é publicado em Outubro; cartas deverão ser enviadas antes do fim do ano.", 3);
                    }
                }
            }
        }

        /// <summary>
        /// Get all leases
        /// </summary>
        /// <returns></returns>
        protected async Task<IEnumerable<ArrendamentoVM>> GetAllLeases()
        {
            try
            {
                IEnumerable<ArrendamentoVM> listOfleases = await arrendamentosService!.GetAll();
                if (listOfleases is not null && listOfleases.Any())
                {
                    listOfleases = listOfleases.OrderByDescending(l => l.Id).ToList();
                    return listOfleases;
                }
                else
                    return new List<ArrendamentoVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return null;

            }
        }
        protected async Task<IEnumerable<CoeficienteAtualizacaoRendas>?> GetAllRentCoefficients()
        {
            try
            {
                IEnumerable<CoeficienteAtualizacaoRendas> listOfCoefficients = await arrendamentosService.GetRentUpdatingCoefficients();
                if (listOfCoefficients is not null && listOfCoefficients.Any())
                {
                    listOfCoefficients.OrderByDescending(l => l.Id).ToList();
                    return listOfCoefficients!.ToList();
                }
                else
                    return new List<CoeficienteAtualizacaoRendas>();
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Novo arrendamento
        /// </summary>
        /// <param name="args"></param>
        protected void onAddLease(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            PrepareAndCreateNewLease();
        }

        protected void onAddExistentLease(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            PrepareAndCreateExistingLease();
        }

        // Grava dados do arrendamento
        protected async Task SaveLeaseData()
        {
            IsDirty = false;
            ErrorVisibility = false;

            // Verifica se há erros de validação
            ValidationMessages = validatorService!.ValidateLeasesEntries(SelectedLease!);

            if (ValidationMessages == null) // Dados ok, atualizada ou insere dados
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    var updateOk = await arrendamentosService!.UpdateArrendamento(SelectedLease!.Id, SelectedLease);
                    if (!updateOk)
                    {
                        ErrorVisibility = true;
                        ValidationMessages = new List<string> { "Erro na atualização de Arrendamento" };
                    }
                    else
                    {
                        ToastTitle = "Atualização de Arrendamento";
                        ToastMessage = "Registo atualizado com sucesso";
                        ToastCss = "e-toast-success";

                        StateHasChanged();
                        await Task.Delay(100);
                        await ToastObj!.ShowAsync();
                    }
                }
                else // Insert
                {
                    if (await arrendamentosService!.ArrendamentoExiste(SelectedLease!.ID_Fracao))
                    {
                        ErrorVisibility = true;
                        ValidationMessages = new List<string> { "Já existe arrendamento ativo para essa fração\", \"Verifique, p.f." };
                    }
                    else
                    {
                        var insertOk = await arrendamentosService!.InsertArrendamento(SelectedLease!);
                        if (insertOk == false)
                        {
                            ErrorVisibility = true;
                            ValidationMessages = new List<string> { "Erro na criação de Arrendamento" };
                        }
                        else
                        {
                            AddEditLeaseVisibility = false;
                            LeaseId = await arrendamentosService.GetLastId();

                            if (SelectedLease.ArrendamentoNovo == false)
                            {
                                await UpdateTenantBalance(SelectedLease.ID_Inquilino, SaldoCorrenteInquilino);
                            }
                            else
                            {
                                IssueRentalContractVisibility = true;
                            }

                            ToastTitle = "Criação de Arrendamento";
                            ToastMessage = "Registo criado com sucesso";
                            ToastCss = "e-toast-success";

                            StateHasChanged();
                            await Task.Delay(100);
                            await ToastObj!.ShowAsync();

                        }
                    }
                }

                AddEditLeaseVisibility = false;
                leases = await GetAllLeases();
                StateHasChanged();
            }
            else // Não passou validação... dados incorretos ou em falta => exibe diálogo ao utilizador
            {
                ErrorVisibility = true;
            }
        }

        protected async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            AlertVisibility = false;
            activeLeases = leases!.Where(l => l.Ativo == true);

            switch (args.Item.Id)
            {
                case "Leases_Grid_pdfexport":
                    await LeasingAgreements_Report();
                    break;
                case "DueRentLetter": // carta de renda em atraso
                    var unpaidRents = await TenantHasUnpaidRents();
                    if (unpaidRents == false) 
                    {
                        alertTitle = "Envio de carta ao inquilino";
                        AlertVisibility = true;
                        WarningMessage = "Inquilino não tem rendas em atraso! Verifique, p.f";
                        return;
                    }

                    SendingLetterType = DocumentoEmitido.RendasEmAtraso;
                    SendLetterDialogVisibility = true;
                    break;
                case "ContractOpposition": // carta de oposição à renovação do contrato
                    SendingLetterType = DocumentoEmitido.OposicaoRenovacaoContrato;
                    SendLetterDialogVisibility = true;
                    break;
                case "ContractLeaseTerm": // Estender prazo do contrato
                    LeaseTermCaption = "Confirma operação?";
                    alertMessageType = AlertMessageType.Info;
                    var monthsToEnd = GetMonthDifference(DateTime.Now, SelectedLease.Data_Fim);
                    if(monthsToEnd != 1)
                    {
                        LeaseTermCaptionNote = $"Contrato só termina dentro de {monthsToEnd} meses!";
                        alertMessageType = AlertMessageType.Warning;
                        StateHasChanged();
                    }
                    LeaseTermVisibility = true;
                    break;
                case "ViewLeaseContract": // Visualizar Pdf do contrato de arrendamento
                    await ViewLeaseContarct();
                    break;
                default:
                    break;
            }
        }

        protected async Task ToolbarClickHandler_Coef(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "Coefs_Grid_pdfexport")
            {
                await coefsGridObj!.ExportToPdfAsync();
            }
        }

        protected async Task OnLeaseDoubleClickHandler(RecordDoubleClickEventArgs<ArrendamentoVM> args)
        {
            HideToolbar_LetterOptions();

            LeaseId = args.RowData.Id;
            SelectedLease = await arrendamentosService!.GetArrendamento_ById(LeaseId);
            OriginalLeaseData = await arrendamentosService.GetArrendamento_ById(LeaseId); // TODO should use 'Clone/MemberWise'

            AddEditLeaseVisibility = true;
            EditCaption = L["EditMsg"] + " " + L["TituloArrendamento"];
            RecordMode = OpcoesRegisto.Gravar;
        }

        protected async Task OnLeaseCommandClicked(CommandClickEventArgs<ArrendamentoVM> args)
        {
            HideToolbar_LetterOptions();

            LeaseId = args.RowData.Id;
            var optionId = args.CommandColumn.ID;

            SelectedLease = await arrendamentosService!.GetArrendamento_ById(LeaseId);

            DeleteLeaseCaption = SelectedLease?.NomeInquilino;

            OriginalLeaseData = await arrendamentosService.GetArrendamento_ById(LeaseId); // TODO should use 'Clone/MemberWise'
            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                AddEditLeaseVisibility = true;
                EditCaption = L["EditMsg"] + " " + L["TituloArrendamento"];
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();
            }
            else if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                RecordMode = OpcoesRegisto.Apagar;
                DeleteLeaseVisibility = true;
                StateHasChanged();
            }
        }

        protected async Task OnCoefficientCommandClicked(CommandClickEventArgs<CoeficienteAtualizacaoRendas> args)
        {
            CoefficientId = args.RowData.Id;
            SelectedRentCoefficient = await arrendamentosService!.GetRentUpdatingCoefficient_ById(args.RowData.Id);
            OriginalRentCoefficientData = await arrendamentosService.GetRentUpdatingCoefficient_ById(args.RowData.Id); // TODO should use 'Clone/MemberWise'

            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                AddEditLeaseVisibility = true;
                EditCaption = L["EditMsg"] + " " + L["TituloMenuCoeficientesRendas"];
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();
            }
        }

        protected async Task OnCoefficientDoubleClickHandler(RecordDoubleClickEventArgs<CoeficienteAtualizacaoRendas> args)
        {

            CoefficientId = args.RowData.Id;
            SelectedRentCoefficient = await arrendamentosService!.GetRentUpdatingCoefficient_ById(args.RowData.Id);
            OriginalRentCoefficientData = await arrendamentosService.GetRentUpdatingCoefficient_ById(args.RowData.Id); // TODO should use 'Clone/MemberWise'

            AddEditRentCoefficientVisibility = true;
            EditCaption = L["EditMsg"] + " " + L["TituloMenuCoeficientesRendas"];
            RecordMode = OpcoesRegisto.Gravar;
        }

        private void CheckIfLeaseData_Changed()
        {
            var comparer = new ObjectsComparer.Comparer<ArrendamentoVM>();
            IEnumerable<Difference> differences;
            comparer.Compare(SelectedLease!, OriginalLeaseData!, out differences);
            IsDirty = differences.Any();
        }

        protected void CloseEditDialog()
        {
            IsDirty = false;

            var comparer = new ObjectsComparer.Comparer<ArrendamentoVM>();
            var currentData = SelectedLease;
            var originalData = OriginalLeaseData;
            IEnumerable<Difference> differences;
            var isEqual_P = comparer.Compare(currentData!, originalData!, out differences);
            if (!isEqual_P)
            {
                IsDirty = true;
            }
            else
            {
                AddEditLeaseVisibility = false;
            }
        }

        protected void ContinueEdit()
        {
            IsDirty = false;
            AddEditLeaseVisibility = true;
        }

        protected void IgnoreChangesAlert()
        {
            IsDirty = false;
            ErrorVisibility = false;
            AddEditLeaseVisibility = false;
        }

        protected async Task ConfirmDeleteYes()
        {
            try
            {
                HideToolbar_LetterOptions();

                DeleteLeaseVisibility = false;

                var leaseHasPayments = await arrendamentosService!.ChildrenExists(SelectedLease!.ID_Fracao);
                if (leaseHasPayments)
                {
                    ErrorVisibility = true;
                    ValidationMessages = new List<string> { "Arrendamento tem pagamentos associados! Operação cancelada." };
                }
                else
                {
                    DeleteLeaseVisibility = false;
                    var resultOk = await arrendamentosService!.DeleteArrendamento(SelectedLease!.Id);
                    if (resultOk)
                    {
                        ToastTitle = L["DeleteMsg"] + " " + L["TituloArrendamento"];

                        ToastMessage = L["SuccessDelete"];
                        ToastCss = "e-toast-success";

                        leases = await GetAllLeases();

                        await CheckForAlerts();

                        await ShowToastMessage();
                    }
                    else
                    {
                        ErrorVisibility = true;
                        ValidationMessages = new List<string> { "Erro ao remover Arrendamento" };
                    } // delete ok?
                } // are there payments in the leasing?
            }
            catch (Exception exc)
            {
                _logger.LogError(exc.ToString());

                ErrorVisibility = true;
                ValidationMessages = new List<string> { $"Não foi possível concluir operação. {exc.Message}" };
            }
        }

        protected void ConfirmDeleteNo()
        {
            DeleteLeaseVisibility = false;
        }

        protected async Task ExtendLeaseTerm(int Id)
        {
            var resultOk = await arrendamentosService!.ExtendLeaseTerm(Id);
            if (resultOk)
            {
                LeaseTermVisibility = false;
                ToastTitle = "Atualização do termo do contrato";
                ToastMessage = L["RegistoGravadoSucesso"];
                ToastCss = "e-toast-success";
                await InvokeAsync(StateHasChanged);
                await leasesGridObj.Refresh();
                leases = await GetAllLeases();
            }
            else {
                ErrorVisibility = true; 
                ValidationMessages = new List<string> {"Erro ao atualizar data-fim do contrato" };
            }
        }

        /// <summary>
        ///  Fecha diálogo de validação
        /// </summary>
        protected void CloseValidationErrorBox()
        {
            ErrorVisibility = false;
            switch (RecordMode)
            {
                case OpcoesRegisto.Gravar:
                case OpcoesRegisto.Inserir:
                    AddEditLeaseVisibility = true;
                    break;
                case OpcoesRegisto.Apagar:
                    DeleteLeaseVisibility = false;
                    break;
            }
        }

        private async Task UpdateUnitSituation(int id)
        {
            await contratosService.AtualizaSituacaoFracao(id); // fração
        }

        private async Task UpdateTenantBalance(int IdInquilino, decimal decSaldoCorrente)
        {
            await inquilinosService!.AtualizaSaldo(IdInquilino, decSaldoCorrente);
        }

        private async Task Create_InitialContractPayments(ArrendamentoVM arrendamento, int idFracao)
        {
            await arrendamentosService!.GeraMovimentos(arrendamento, idFracao);
        }


        protected async Task Emite_Contrato()
        {
            if (await arrendamentosService!.ContratoEmitido(LeaseId))
            {
                ErrorVisibility = true;
                IssueRentalContractVisibility = false;
                ValidationMessages = new List<string> { "Contrato já foi emitido.\r\n\r\nVerifique, p.f" };
            }
            else
            {
                try
                {
                    await ShowSpinner();
                    var _arrendamento = await arrendamentosService!.GetArrendamento_ById(LeaseId);
                    IssueRentalContractVisibility = false;

                    // Emite contrato
                    Contrato contrato = await contratosService!.GetDadosContrato(_arrendamento);
                    string docGerado = await contratosService.EmiteContrato(contrato);

                    // Verifica se contrato foi gerado (fração está livre?)
                    if (string.IsNullOrEmpty(docGerado))
                    {
                        await HideSpinner();
                        ErrorVisibility = true;
                        ValidationMessages = new List<string> { "Fração não está livre para arrendamento..." };
                    }
                    else
                    {
                        // Marca contrato de arrendamento como emitido

                        // Neste momento está a fazer a atualização automática da renda, aquando da emissão;
                        // Este processo deverá ser executado apenas aquando do processamento mensal de rendas
                        // criar registo de log para esta situação

                        await AtualizaSituacaoContrato(LeaseId, docGerado);

                        await HideSpinner();

                        ToastTitle = "Geração de contrato";
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastCss = "e-toast-success";

                        await ShowToastMessage();

                        leases = await GetAllLeases();
                        return;
                    }

                }
                catch (Exception ex)
                {
                    await HideSpinner();
                    _logger.LogError(ex.Message, ex);
                }
            }
        }

        private async Task AtualizaSituacaoContrato(int idArrendamento, string docGerado)
        {
            await arrendamentosService!.MarcaContratoComoEmitido(idArrendamento, docGerado);
            //blnContratoEmitido = true;
        }
        private async Task AtualizaSituacaoEnvioAtualizacaoRendas(int idArrendamento, string docGerado)
        {
            await arrendamentosService!.MarcaCartaAtualizacaoComoEmitida(idArrendamento, docGerado);
        }

        private async Task<bool> RegistaEnvioAtualizacaoRendas()
        {
            return await arrendamentosService!.RegistaProcessamentoAtualizacaoRendas();
        }

        protected async Task HideToast()
        {
            await ToastObj!.HideAsync();
        }

        protected void HandleTenantBalanceChange(decimal balance)
        {
            SaldoCorrenteInquilino = balance;
        }

        // check if lease has more than one year
        private bool CanUpdateRentsLetterBeSent(DateTime start)
        {

            var result = (DateTime.Now.Year - start.Year - 1) +
                (((DateTime.Now.Month > start.Month) ||
                ((DateTime.Now.Month == start.Month) && (DateTime.Now.Day >= start.Day)))
                ? 1
                : 0);
            return result >= 1;
        }

        protected async Task RentCoefficientActionBeginHandler(ActionEventArgs<CoeficienteAtualizacaoRendas> Args)
        {
            ToastTitle = L["TituloMenuCoeficientesRendas"];

            if (Args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
            {
                var rentCoefficient = Args.Data;

                if (Args.Action.ToLower() == "add")
                {
                    var insertOk = await arrendamentosService!.InsertRentCoefficient(rentCoefficient);
                    if (insertOk)
                    {
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastCss = "e-toast-success";
                    }
                    else
                    {
                        ToastMessage = L["FalhaCriacaoRegisto"];
                        ToastCss = "e-toast-danger";
                    }
                }
                else
                {
                    var updateOk = await arrendamentosService!.UpdateRentCoefficient(rentCoefficient.Id, rentCoefficient);
                    if (updateOk)
                    {
                        ToastMessage = L["RegistoGravadoSucesso"];
                        ToastCss = "e-toast-success";
                    }
                    else
                    {
                        ToastMessage = L["FalhaGravacaoRegisto"];
                        ToastCss = "e-toast-danger";
                    }
                }

                rentCoefficients = await GetAllRentCoefficients();

                await ShowToastMessage();
            }
        }
        protected async Task LeasingAgreements_Report()
        {
            if (activeLeases!.Count() == 0)
            {
                ToastTitle = "Listagem de arrendamentos";
                ToastMessage = $"Não há contratos ativos! Verifique, p.f.";
                ToastCss = "e-toast-warning";

                await ShowToastMessage();
                return;
            }

            await leasesGridObj!.ExportToPdfAsync();
        }

        protected async Task IssueLateRentLetter()
        {

            ToastTitle = "Carta de aviso - rendas em atraso";

            // TODO - testar se data da emissão está dentro do prazo (3 pagamentos - 90 dias)
            // alertar user em conformidade

            // Verificar se carta já foi enviada
            var letterAlreadySent = await arrendamentosService.VerificaEnvioCartaAtrasoEfetuado(LeaseId);
            if (letterAlreadySent)
            {
                alertTitle = "Envio de carta ao inquilino";
                WarningMessage = "Carta já foi enviada!. Verifique, p.f.";
                AlertVisibility = true;
                return;
            }

            var latePaymentLetterData = await arrendamentosService!.GetDadosCartaRendasAtraso(SelectedLease!);
            if (latePaymentLetterData is not null)
            {
                var docGerado = await arrendamentosService.EmiteCartaRendasAtraso(latePaymentLetterData);
                if (string.IsNullOrEmpty(docGerado))
                {
                    ToastMessage = "Erro na emissão de carta. Verifique log, p.f.";
                    ToastCss = "e-toast-danger";
                }
                else
                {
                    var documentoARegistar = docGerado.Replace(".docx", ".pdf");
                    try
                    {
                        await arrendamentosService.RegistaCartaAtrasoRendas(LeaseId, documentoARegistar);
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastCss = "e-toast-success";
                    }
                    catch (Exception ex)
                    {
                        ToastMessage = $"Erro ao registar envio de carta na BD ({ex.Message}). Processo terminou com erro! Contacte Administrador, p.f.";
                        ToastCss = "e-toast-danger";
                    }
                }
            }
            else
            {
                ToastMessage = "Erro ao obter dados para emissão de carta";
                ToastCss = "e-toast-danger";
            }

            SendLetterDialogVisibility = false;

            await ShowToastMessage();
        }

        protected async Task IssueContractOppositionLetter()
        {

            // TODO - implementar procedimento para 'resposta do inquilino'
            ToastTitle = L["TituloCartaRevogacao"];

            // Verificar se carta já foi enviada

            var alreadySent = await arrendamentosService.VerificaSeExisteCartaRevogacao(LeaseId);

            if (alreadySent)
            {
                SendLetterDialogVisibility = false;
                alertTitle = ToastTitle; ;
                WarningMessage = "Carta já foi enviada... Verifique, p.f.";

                var dateSent = SelectedLease.DataEnvioCartaRevogacao;
                var answerDateExpected = dateSent.AddDays(10); // estão a ser assumidos 10 dias para prazo de resposta. Configurar?

                // Verificar se carta enviada já foi respondida pelo inquilino
                var letterAnswered = await arrendamentosService.VerificaSeExisteRespostaCartaRevogacao(LeaseId);
                if (letterAnswered)
                {
                    // Verifica se a data-limite da resposta foi ultrapassada
                    if (DateTime.Now.Date < answerDateExpected.Date) // dentro do prazo
                    {
                        WarningMessage = "Processo de envio cancelado. Já foi enviada carta, e Inquilino respondeu no prazo definido... ";
                    }
                    else // prazo ultrapassado
                    {
                        WarningMessage = $"Processo de envio cancelado. Já foi enviada carta, e Inquilino respondeu fora do prazo definido (limite: {answerDateExpected.ToShortDateString()})... Verifique Log , p.f.";
                    }
                }
                else // Carta enviada ==> sem registo de resposta
                {
                    // Verifica se data-limite da resposta foi ultrapassada
                    if (DateTime.Now.Date < answerDateExpected.Date) // dentro do prazo
                    {
                        WarningMessage = "Processo de envio cancelado. Já foi enviada carta, Aguarda resposta do Inquilino";
                    }
                    else // Inquilino não respondeu no prazo definido como limite
                    {
                        WarningMessage = $"Processo de envio cancelado. Já foi enviada carta. Inquilino não respondeu no prazo (limite: {answerDateExpected.ToShortDateString()})... Verifique, p.f.";
                    }
                }

                AlertVisibility = true;

                StateHasChanged();
                return;
            }

            // Recolhe dados para envio da carta
            var oppositionLetterData = await arrendamentosService!.GetDadosCartaOposicaoRenovacaoContrato(SelectedLease!);
            if (oppositionLetterData is null)
            {
                ToastMessage = $"Processo de envio cancelado. Erro ao obter dados para a carta! Verifique log, p.f.";
                ToastCss = "e-toast-danger";

                await ShowToastMessage();
                return;
            }
            else
            {
                // gera documentos (pdf + docx)
                var docGerado = await arrendamentosService.EmiteCartaOposicaoRenovacaoContrato(oppositionLetterData);
                if (string.IsNullOrEmpty(docGerado))
                {
                    // erro ao gerar carta (pdf), alerta e sai 

                    ToastMessage = $"Processo de envio cancelado. Erro ao gerar pdf... Verifique log, p.f.";
                    ToastCss = "e-toast-danger";

                    await ShowToastMessage();
                    return;
                }
                else // carta gerada, regista ocorrência na BD
                {
                    ToastTitle = "Carta de oposição à renovação do contrato";

                    var documentoARegistar = docGerado.Replace(".docx", ".pdf");

                    var registerOk = await arrendamentosService.RegistaCartaOposicao(LeaseId, documentoARegistar);
                    // verifica se processo de registo (final, onde tabelas são alteradas) terminou com sucesso
                    if (registerOk)
                    {
                        ToastMessage = "Operação terminada com sucesso";
                        ToastCss = "e-toast-success";
                    }
                    else
                    {
                        // TODO - erro no processo atualização da BD; como os documentos já foram gerados (pdf + docx), haverá necessidade de os remover
                        ToastMessage = "Processo de envio cancelado. Erro ao registar carta. Verifique log, p.f.";
                        ToastCss = "e-toast-danger";
                    }

                    SendLetterDialogVisibility = false;

                    await ShowToastMessage();
                }
            }
        }

        private void PrepareAndCreateNewLease()
        {
            RecordMode = OpcoesRegisto.Inserir;
            NewCaption = L["NewMsg"] + " " + L["TituloArrendamento"];

            SelectedLease = new ArrendamentoVM()
            {
                ArrendamentoNovo = true,
                Ativo = true,
                ContratoEmitido = false,
                Data_Inicio = FirstDayOfNextMonth(DateTime.Now),
                Data_Fim = FirstDayOfNextMonth(DateTime.Now.AddYears(3)),
                Data_Pagamento = DateTime.Now,
                Data_Saida = DateTime.Now.AddYears(3),
                Prazo = 3,
                DocumentoGerado = "",
                Doc_IRS = true,
                Doc_Vencimento = true,
                Caucao = true,
                EstadoPagamento = "Pendente",
                Fiador = true,
                FormaPagamento = 2,
                ID_Fiador = 0,
                ID_Fracao = 0,
                ID_Inquilino = 0,
                Notas = "",
                Prazo_Meses = 36,
                RenovacaoAutomatica = true,
                Valor_Renda = 0,
                SaldoInicial = 0
            };

            AddEditLeaseVisibility = true;
            StateHasChanged();
        }

        private void PrepareAndCreateExistingLease()
        {
            RecordMode = OpcoesRegisto.Inserir;
            NewCaption =L["NewMsg"] + " " + L["TituloArrendamento"] + " - " + L["TituloContratoExistente"];

            SelectedLease = new ArrendamentoVM()
            {
                ArrendamentoNovo = false,
                Ativo = true,
                ContratoEmitido = false,
                Data_Inicio = FirstDayOfNextMonth(DateTime.Now),
                Data_Fim = FirstDayOfNextMonth(DateTime.Now.AddYears(3)),
                Data_Pagamento = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                Data_Saida = DateTime.Now.AddYears(3),
                DocumentoGerado = "",
                Doc_IRS = true,
                Doc_Vencimento = true,
                Caucao = true,
                EstadoPagamento = "Pendente",
                Fiador = true,
                FormaPagamento = 2,
                ID_Fiador = 0,
                ID_Fracao = 0,
                ID_Inquilino = 0,
                Notas = "",
                Prazo_Meses = 36,
                Prazo = 3,
                RenovacaoAutomatica = true,
                Valor_Renda = 0,
                SaldoInicial = 0
            };

            AddEditLeaseVisibility = true;
            StateHasChanged();
        }



        private async Task Process_Opposition_Letter()
        {
            // Verificar se carta já foi enviada
            var alreadySent = await arrendamentosService!.VerificaSeExisteCartaRevogacao(LeaseId);
            if (alreadySent)
            {
                ToastMessage = "Já foi enviada uma carta de oposição... Verifique, p.f.";
                ToastCss = "e-toast-warning";

                // Verificar se carta já foi respondida pelo inquilino
                var letterAnswered = await arrendamentosService.VerificaSeExisteRespostaCartaRevogacao(LeaseId);
                if (letterAnswered)
                {
                    ToastMessage = "Já foi enviada uma carta de oposição e Inquilino já respondeu... Verifique, p.f.";
                    ToastCss = "e-toast-warning";
                }
                else // inquilino não respondeu 
                {
                    // Verifica se data de resposta já foi ultrapassada
                    var dateSent = SelectedLease.DataEnvioCartaRevogacao;
                    var answerDateExpected = dateSent.AddDays(10); // estão a ser assumidos 10 dias para prazo de resposta. Configurar?

                    if (answerDateExpected < DateTime.Now)
                    {
                        ToastMessage = "Já foi enviada uma carta de oposição ==> prazo de resposta ultrapassado... Verifique, p.f.";
                        ToastCss = "e-toast-danger";
                    }
                    else // prazo ultrapassado
                    {
                        ToastMessage = $"Já foi enviada uma carta de oposição e Inquilino ainda não respondeu (limite: {answerDateExpected.ToShortDateString()})... Verifique, p.f.";
                        ToastCss = "e-toast-warning";
                    }
                }

                await ShowToastMessage();
                return;
            }


            var oppositionLetterData = await arrendamentosService!.GetDadosCartaOposicaoRenovacaoContrato(SelectedLease!);
            if (oppositionLetterData is not null)
            {
                var docGerado = await arrendamentosService.EmiteCartaOposicaoRenovacaoContrato(oppositionLetterData);
                if (string.IsNullOrEmpty(docGerado))
                {
                    // TODO erro no documento gerado... informar user
                }
                else
                {
                    var documentoARegistar = docGerado.Replace(".docx", ".pdf");
                    await arrendamentosService.RegistaCartaOposicao(LeaseId, documentoARegistar);
                    ToastTitle = "Carta de oposição à renovação do contrato";
                    ToastMessage = "Operação terminada com sucesso";
                    ToastCss = "e-toast-success";

                    await ShowToastMessage();
                }
            }

        }

        private async Task Process_LatePayment_Letter()
        {
            // TODO - testar se data da emissão está dentro do prazo (3 pagamentos - 90 dias)
            // alertar user em conformidade

            var latepaymentLetterData = await arrendamentosService!.GetDadosCartaRendasAtraso(SelectedLease!);
            if (latepaymentLetterData is not null)
            {
                var docGerado = await arrendamentosService.EmiteCartaRendasAtraso(latepaymentLetterData);
                if (string.IsNullOrEmpty(docGerado))
                {
                    // TODO erro no documento gerado... informar user
                }
                else
                {
                    var documentoARegistar = docGerado.Replace(".docx", ".pdf");
                    await arrendamentosService.RegistaCartaAtrasoRendas(LeaseId, documentoARegistar);
                    ToastTitle = "Carta de aviso de rendas em atraso";
                    ToastMessage = "Operação terminada com sucesso";
                    ToastCss = "e-toast-success";

                    await ShowToastMessage();
                }
            }

        }
        protected async Task RowSelectHandler(RowSelectEventArgs<ArrendamentoVM> args)
        {

            LeaseId = args.Data.Id;
            SelectedLease = await arrendamentosService!.GetArrendamento_ById(LeaseId);

            ShowToolbar_LetterOptions();

        }

        private async Task ViewLeaseContarct()
        {
            var agreementIssued = SelectedLease.ContratoEmitido;
            RecordMode = OpcoesRegisto.Info;
            if (agreementIssued)
            {
                var fileName = SelectedLease.DocumentoGerado;
                PdfFilePath = await arrendamentosService.GetPdfFilename(fileName);
                if (string.IsNullOrEmpty(PdfFilePath))
                {
                    ShowPdfVisibility = false;

                    ToastTitle = "Leitura de pdf";
                    ToastMessage = "Ficheiro não existe no local indicado!";
                    ToastCss = "e-toast-danger";
                    ShowPdfVisibility = false;

                    await ShowToastMessage();
                }
                else
                {
                    ShowPdfVisibility = true;
                }
            }

        }

        protected void RowDeselectHandler(RowDeselectEventArgs<ArrendamentoVM> args)
        {
            HideToolbar_LetterOptions();
        }

        // Após confirmação de emissão de carta pelo utilizador
        protected async Task HandleIssuedLetterConfirmation(DocumentoEmitido letterType)
        {
            switch (letterType)
            {
                case DocumentoEmitido.ContratoArrendamento:
                    break;
                case DocumentoEmitido.AtualizacaoRendas:
                    break;
                case DocumentoEmitido.OposicaoRenovacaoContrato:
                    await IssueContractOppositionLetter();
                    break;
                case DocumentoEmitido.RendasEmAtraso:
                    await IssueLateRentLetter();
                    break;
                default:
                    break;
            }
        }

        protected async Task<bool> TenantHasUnpaidRents()
        {
            // Verificar se inquilino tem rendas em atraso
            var _payments = await recebimentosService.GetAll();
            var _tenantId = SelectedLease.ID_Inquilino;
            var tenantHaveOwedPayments = _payments.Where(p => p.Estado == 3 && p.ID_Inquilino == _tenantId).ToList();
            return tenantHaveOwedPayments.Any();
        }
        protected async Task HandleIssuedLetterCancelation()
        {
            SendLetterDialogVisibility = false;
            await leasesGridObj!.ClearSelectionAsync();
            //ShowToolbaContractRevocationLetter = false;
            //ShowToolbarDueRentLetter = false;
        }
        protected int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            if (endDate.Date < startDate.Date)
            {
                return 0;
            }

            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        private DateTime FirstDayOfNextMonth(DateTime dt)
        {
            DateTime ss = new DateTime(dt.Year, dt.Month, 1);
            return ss.AddMonths(1);
        }
        private async Task ShowToastMessage()
        {
            StateHasChanged();
            await Task.Delay(100);
            await ToastObj!.ShowAsync();
        }

        private void HideToolbar_LetterOptions()
        {
            ShowToolbarDueRentLetter = false;
            ShowToolbaContractRevocationLetter = false;
            ShowToolbaContractLeaseTerm = false;
            ShowToolbarViewContract = false;
        }
        private void ShowToolbar_LetterOptions()
        {
            ShowToolbarDueRentLetter = true;
            ShowToolbaContractRevocationLetter = true;
            ShowToolbaContractLeaseTerm = true;
            if (SelectedLease.ContratoEmitido)
                ShowToolbarViewContract = true;
        }

        private async Task ShowSpinner()
        {
            await Task.Delay(100);
            SpinnerVisibility = true;
            StateHasChanged();

        }
        private async Task HideSpinner()
        {
            await Task.Delay(100);
            SpinnerVisibility = false;
            StateHasChanged();
        }

        public void Dispose()
        {
            leasesGridObj?.Dispose();
            coefsGridObj?.Dispose();
            SpinnerObj?.Dispose();
            ToastObj?.Dispose();
        }
    }
}
