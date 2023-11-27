using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ObjectsComparer;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
using PropertyManagerFLApplication.Utilities;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    /// <summary>
    /// Base class for tenants pages
    /// </summary>
    public class InquilinosBase : ComponentBase, IDisposable
    {
        /// <summary>
        /// tenants service
        /// </summary>
        [Inject] public IInquilinoService? inquilinoService { get; set; }
        [Inject] public IFracaoService? fracaoService { get; set; }
        [Inject] public IFiadorService? FiadorService { get; set; }
        [Inject] public IArrendamentoService? arrendamentosService { get; set; }

        [Inject] public IRecebimentoService? recebimentosService { get; set; }
        [Inject] public IArrendamentoService? arrendamentoService { get; set; }
        [Inject] public IWebHostEnvironment _env { get; set; }
        [Inject] public NavigationManager nav { get; set; }
        [Inject] protected IValidationService validatorService { get; set; }
        [Inject] protected IStringLocalizer<App> L { get; set; }
        [Inject] public IConfiguration _config { get; set; }
        [Inject] public ILogger<App> _logger { get; set; }

        protected IEnumerable<CC_InquilinoVM>? TenantPaymentsHistory { get; set; }
        protected IEnumerable<InquilinoVM>? Tenants { get; set; }
        protected IEnumerable<HistoricoAtualizacaoRendasVM>? TenantUpdatedRents { get; set; }
        protected IEnumerable<FiadorVM>? Guarantors { get; set; }

        protected CC_InquilinoVM? SelectedHistoryPayment { get; set; }
        protected InquilinoVM? SelectedTenant { get; set; }
        protected FiadorVM? SelectedGuarantor { get; set; }

        protected DocumentoInquilinoVM? TenantDocument { get; set; }
        protected InquilinoVM? OriginalTenantData { get; set; }
        protected FiadorVM? OriginalGuarantorData { get; set; }
        protected DocumentoInquilinoVM? OriginalTenantDocumentData { get; set; }
        protected OpcoesRegisto RecordMode { get; set; }
        protected int TenantId { get; set; }
        protected int FiadorId { get; set; }
        protected int TenantDocumentId { get; set; }
        protected string? NewCaption { get; set; }
        protected string? EditCaption { get; set; }
        protected string? DeleteCaption;
        protected string? WarningMessage { get; set; }

        protected bool AddEditTenantVisibility { get; set; }
        protected bool AddEditFiadorVisibility { get; set; }
        protected bool AddEditDocumentVisibility { get; set; }
        protected bool DeleteTenantVisibility { get; set; }
        protected bool DeleteGuarantorVisibility { get; set; }
        protected bool DeleteDocumentVisibility { get; set; }
        protected bool WarningVisibility { get; set; }
        protected bool ErrorVisibility { get; set; } = false;
        protected bool AlertVisibility { get; set; } = false;
        protected bool ShowFileVisibility { get; set; }
        protected bool ShowWordDocumentVisibility { get; set; }
        protected string? documentFilePath { get; set; }
        protected string? documentServiceUrl { get; set; } // Server pdfs

        protected decimal SaldoEsperado;

        protected decimal NewRentUpdated { get; set; }

        protected enum TenantFileTypes
        {
            PDF,
            WINWORD
        }


        public DialogEffect Effect = DialogEffect.Zoom;
        protected SfSpinner? SpinnerObj { get; set; }
        protected SfGrid<InquilinoVM>? tenantsGridObj { get; set; }
        protected SfToast? ToastObj { get; set; }

        protected string? ToastTitle;
        protected string? ToastMessage;
        protected string? ToastCss;
        protected string? ToastIcon;


        protected bool IsDirty = false;
        protected List<string> ValidationsMessages = new();

        protected Modules modulo { get; set; }
        protected AlertMessageType alertMessageType = AlertMessageType.Info;
        protected string? alertTitle = "";

        protected string? tenantName { get; set; }
        protected string? TituloTabDocumentos { get; set; }
        protected int MaxFileSize = 5 * 1024 * 1024; // 5 MB

        protected bool ShowToolbarDueRentLetter { get; set; }
        protected bool ShowToolbarContractRevocationLetter { get; set; }
        protected DocumentoEmitido SendingLetterType { get; set; }
        protected bool SendLetterDialogVisibility { get; set; } = false;
        protected bool ConfirmUpdateRentDialogVisibility { get; set; } = false;
        protected bool ConfirmManualUpdateRentDialogVisibility { get; set; } = false;
        protected bool ConfirmManualUpdateRentVisibility { get; set; } = false;
        protected ArrendamentoVM? Lease { get; set; }
        protected bool AutomaticRentAdjustment { get; set; }

        /// <summary>
        /// Startup Inquilinos
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            OriginalTenantData = new();
            OriginalGuarantorData = new();

            AddEditTenantVisibility = false;
            AddEditFiadorVisibility = false;

            DeleteTenantVisibility = false;
            DeleteGuarantorVisibility = false;
            DeleteDocumentVisibility = false;

            ErrorVisibility = false;
            AlertVisibility = false;

            DeleteCaption = "";

            IsDirty = false;

            tenantName = "";
            TituloTabDocumentos = L["TituloDocumento"] + "s";

            ToastTitle = "";
            ToastMessage = "";
            ToastCss = "";
            ToastIcon = "";

            documentFilePath = "";
            documentServiceUrl = "";

            WarningVisibility = false;
            WarningMessage = "";
            alertTitle = "";

            TenantId = 0;
            FiadorId = 0;
            TenantDocumentId = 0;

            var rentAdjustmentsetting = _config.GetSection("AppSettings:AutomaticRentAdjustment").Value;
            if (!string.IsNullOrEmpty(rentAdjustmentsetting))
            {
                AutomaticRentAdjustment = bool.Parse(rentAdjustmentsetting);
            }

            Tenants = await GetAllTenants();
            TenantUpdatedRents = GetTenantRentUpdates();
            //if (!Tenants.Any())
            //{
            //    WarningMessage = "Sem dados para mostrar";
            //    WarningVisibility = true;
            //}

        }

        /// <summary>
        /// Get all tenants
        /// </summary>
        /// <returns></returns>
        protected async Task<IEnumerable<InquilinoVM>> GetAllTenants()
        {
            IEnumerable<InquilinoVM>? tenantsList = await inquilinoService!.GetAll();
            // tenantsList.OrderByDescending(p => p.Id).ToList();
            return tenantsList.ToList();
        }

        protected IEnumerable<HistoricoAtualizacaoRendasVM> GetTenantRentUpdates()
        {
            IEnumerable<HistoricoAtualizacaoRendasVM> result = Task.Run(async () => await inquilinoService!.GetAllRentUpdates()).Result;
            return result;

        }

        protected IEnumerable<HistoricoAtualizacaoRendasVM> GetTenantRentUpdates(int tenantId)
        {
            IEnumerable<HistoricoAtualizacaoRendasVM> result = Task.Run(async () => await inquilinoService!.GetRentUpdates_ByTenantId(tenantId)).Result;
            return result;

        }

        protected IEnumerable<DocumentoInquilinoVM> GetTenantDocuments(int id)
        {

            IEnumerable<DocumentoInquilinoVM> documentsList = Task.Run(async () => await inquilinoService!.GetDocumentosInquilino(id)).Result;
            return documentsList;
        }

        protected IEnumerable<CC_InquilinoVM> GetTenantPaymentHistory(int id)
        {

            IEnumerable<CC_InquilinoVM> paymentsList = Task.Run(async () => await inquilinoService!.GetTenantPaymentsHistory(id)).Result;
            return paymentsList;
        }


        protected IEnumerable<FiadorVM> GetTenantGuarantors(int id)
        {
            IEnumerable<FiadorVM> fiadoresInquilino = Task.Run(async () => await inquilinoService!.GeFiadorInquilino_ById(id)).Result;
            return fiadoresInquilino;
        }

        protected async Task OnTenantDoubleClickHandler(RecordDoubleClickEventArgs<InquilinoVM> args)
        {
            TenantId = args.RowData.Id;
            modulo = Modules.Inquilinos;

            SelectedTenant = await inquilinoService!.GetInquilino_ById(TenantId);
            OriginalTenantData = await inquilinoService.GetInquilino_ById(TenantId); // TODO should use 'Clone/MemberWise'
            EditCaption = $"{L["EditMsg"]} {L["TituloInquilino"]}";
            RecordMode = OpcoesRegisto.Gravar;
            AddEditTenantVisibility = true;
        }


        public async Task OnTenantCommandClicked(CommandClickEventArgs<InquilinoVM> args)
        {
            modulo = Modules.Inquilinos;

            SelectedTenant = args.RowData;

            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                OriginalTenantData = await inquilinoService.GetInquilino_ById(SelectedTenant.Id); // TODO should use 'Clone/MemberWise'

                AddEditTenantVisibility = true;
                EditCaption = $"{L["EditMsg"]} {L["TituloInquilino"]}";
                RecordMode = OpcoesRegisto.Gravar;
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteTenantVisibility = true;
                DeleteCaption = SelectedTenant?.Nome;
            }
        }

        public async Task OnGuarantorCommandClicked(CommandClickEventArgs<FiadorVM> args)
        {
            FiadorId = args.RowData.Id;
            modulo = Modules.Fiadores;
            DeleteCaption = $"{L["DeleteMsg"]} {L["TituloFiador"]}";
            EditCaption = $"{L["EditMsg"]} {L["TituloFiador"]}";


            SelectedGuarantor = args.RowData;
            OriginalGuarantorData = await FiadorService.GetFiador_ById(FiadorId); // TODO should use 'Clone/MemberWise'
            DeleteCaption = SelectedGuarantor.Nome;


            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                AddEditFiadorVisibility = true;
                RecordMode = OpcoesRegisto.Gravar;
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteGuarantorVisibility = true;
                DeleteCaption = SelectedGuarantor.Nome;
            }
        }

        protected async Task OnGuarantorDoubleClickHandler(RecordDoubleClickEventArgs<FiadorVM> args)
        {
            FiadorId = args.RowData.Id;
            modulo = Modules.Fiadores;

            SelectedGuarantor = args.RowData;
            OriginalGuarantorData = await FiadorService.GetFiador_ById(FiadorId); // TODO should use 'Clone/MemberWise'
            EditCaption = $"{L["EditMsg"]} {L["TituloFiador"]}";
            RecordMode = OpcoesRegisto.Gravar;
            AddEditFiadorVisibility = true;
        }


        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "Tenants_Grid_pdfexport")  //Id is combination of Grid's ID and itemname
            {
                await tenantsGridObj!.ExportToPdfAsync();
                return;
            }

            if (args.Item.Text == "Expand all")
            {
                await tenantsGridObj.ExpandAllDetailRowAsync();
            }
            else if (args.Item.Text == "Collapse all")
            {
                await tenantsGridObj.CollapseAllDetailRowAsync();
            }

            switch (args.Item.Id)
            {
                case "DueRentLetter": // carta de renda em atraso

                    var hasActiveLease = await TenantHasLease();
                    if (hasActiveLease == false)
                    {
                        alertTitle = "Envio de carta de atraso no pagamento";
                        AlertVisibility = true;
                        WarningMessage = "Inquilino não tem contrato de arrendamento! Verifique, p.f";
                        await ToggleRow();
                        return;
                    }

                    var unpaidRents = await TenantHasUnpaidRents();
                    if (unpaidRents == false)
                    {
                        alertTitle = "Envio de carta de atraso no pagamento";
                        AlertVisibility = true;
                        WarningMessage = "Inquilino não tem rendas em atraso! Verifique, p.f";
                        await ToggleRow();
                        return;
                    }

                    SendingLetterType = DocumentoEmitido.RendasEmAtraso;
                    SendLetterDialogVisibility = true;
                    break;

                case "ContractOpposition": // carta de oposição à renovação do contrato
                    hasActiveLease = await TenantHasLease();
                    if (hasActiveLease == false)
                    {
                        alertTitle = "Envio de carta de revogação";
                        AlertVisibility = true;
                        WarningMessage = "Inquilino não tem contrato de arrendamento! Verifique, p.f";
                        await ToggleRow();
                        return;
                    }

                    SendingLetterType = DocumentoEmitido.OposicaoRenovacaoContrato;
                    SendLetterDialogVisibility = true;
                    break;

            }


        }

        public async Task OnContextMenuClick(ContextMenuClickEventArgs<InquilinoVM> args)
        {
            var hasActiveLease = await TenantHasLease();
            if (hasActiveLease == false)
            {
                alertTitle = "Envio de cartas ao inquilino";
                AlertVisibility = true;
                WarningMessage = "Inquilino não tem contrato de arrendamento! Verifique, p.f";
                await ToggleRow();
                return;
            }

            switch (args.Item.Id)
            {
                case "DueRentLetter": // carta de renda em atraso

                    var unpaidRents = await TenantHasUnpaidRents();
                    if (unpaidRents == false)
                    {
                        alertTitle = "Envio de carta de atraso no pagamento";
                        AlertVisibility = true;
                        WarningMessage = "Inquilino não tem rendas em atraso! Verifique, p.f";
                        await ToggleRow();
                        return;
                    }

                    SendingLetterType = DocumentoEmitido.RendasEmAtraso;
                    SendLetterDialogVisibility = true;
                    break;

                case "ContractOpposition": // carta de oposição à renovação do contrato
                    SendingLetterType = DocumentoEmitido.OposicaoRenovacaoContrato;
                    SendLetterDialogVisibility = true;
                    break;
                case "ManualRentUpdate": // carta de atualização de rendas (do inquilino
                    alertTitle = "Envio de cartas ao inquilino";
                    var tenantRentUpdates = await inquilinoService.GetRentUpdates_ByTenantId(TenantId);
                    if (tenantRentUpdates is null)
                    {
                        AlertVisibility = true;
                        WarningMessage = "Não foi feito qualquer aumento de renda para o Inquilino";
                        return;
                    }
                    var currentYearUpdateValues = tenantRentUpdates.FirstOrDefault(r => r.DateProcessed.Year == DateTime.Now.Year);
                    if (currentYearUpdateValues is null)
                    {
                        AlertVisibility = true;
                        WarningMessage = "Não foi feito qualquer aumento de renda para o ano corrente";
                        return;
                    }

                    SendingLetterType = DocumentoEmitido.AtualizacaoManualRenda;
                    SendLetterDialogVisibility = true;
                    break;
                case "RentIncrease": // aumento da renda, manual ou automático (ver flag em AppSettings.json)
                    if (AutomaticRentAdjustment)
                    {
                        ConfirmUpdateRentDialogVisibility = true;
                    }
                    else
                    {
                        var updateAlreadyMade = await inquilinoService.PriorRentUpdates_ThisYear(TenantId);
                        if (updateAlreadyMade)
                        {
                            alertTitle = "Atualizar valor de renda";
                            WarningMessage = "Já foi efetuado um aumento de renda para este inquilino. Verifique histórico, p.f.";
                            AlertVisibility = true;
                            return;
                        }

                        ConfirmManualUpdateRentDialogVisibility = true;
                    }
                    break;
            }
        }

        public async Task OnTenantDocumentCommandClicked(CommandClickEventArgs<DocumentoInquilinoVM> args)
        {
            TenantDocumentId = args.RowData.Id;
            DeleteCaption = "";

            TenantDocument = await inquilinoService!.GetDocumentoById(TenantDocumentId);
            OriginalTenantDocumentData = await inquilinoService.GetDocumentoById(TenantDocumentId);
            DeleteCaption = TenantDocument.DocumentPath;


            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                AddEditDocumentVisibility = true;
                EditCaption = $"{L["EditMsg"]} {L["TituloDocumento"]}";
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteDocumentVisibility = true;
                DeleteCaption = $"{L["DeleteMsg"]} {L["TituloDocumento"]}";
                StateHasChanged();
            }
            else if (args.CommandColumn.Type == CommandButtonType.None)
            {
                RecordMode = OpcoesRegisto.Info;
                var fileName = args.RowData.DocumentPath;
                string? fileExtension = Path.GetExtension(fileName);
                string? folderName = args.RowData.StorageFolder;
                char? storageType = args.RowData.StorageType;


                if (storageType == 'S')
                {
                    documentFilePath = await inquilinoService.GetServerPdfFileName(folderName, fileName);
                }
                else
                {
                    documentFilePath = Path.Combine(_env.WebRootPath, "uploads", folderName!,   fileName!);
                }

                if (fileExtension!.ToLower() == ".pdf")
                {
                    if (storageType == 'C')
                    {
                        if (File.Exists(documentFilePath))
                        {

                            ShowFileVisibility = true;
                        }
                        else
                        {
                            ShowFileVisibility = false;

                            ToastTitle = "Leitura de pdf";
                            ToastMessage = "Erro ao ler Api";

                            StateHasChanged();
                            await Task.Delay(100);
                            await ToastObj!.ShowAsync();
                        }
                    }
                    else
                        ShowFileVisibility = true;
                }
                else if (fileExtension.ToLower() == ".docx")
                {
                    if (storageType == 'S')
                    {
                        if (File.Exists(documentFilePath))
                        {
                            ShowWordDocumentVisibility = true;
                        }
                    }
                    else
                    {
                        ShowWordDocumentVisibility = false;
                    }

                }
                StateHasChanged();
            }

        }

        public async Task SaveTenantData()
        {
            IsDirty = false;
            ErrorVisibility = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationsMessages = validatorService.ValidateTenantEntries(SelectedTenant!);

            if (ValidationsMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    //CheckIfTenantData_Changed(); // 08/2022
                    //if (IsDirty) // registo alterado
                    //{
                    //}

                    ToastTitle = $"{L["btnSalvar"]} {L["TituloInquilino"]}";

                    var updateOk = await inquilinoService!.AtualizaInquilino(SelectedTenant!.Id, SelectedTenant);
                    if (!updateOk)
                    {
                        AlertVisibility = true;
                        alertTitle = "Atualização de dados do inquilino";
                        WarningMessage = $"{L["MSG_ApiError"]}";
                    }
                    else
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = $"{L["SuccessUpdate"]}";
                        ToastIcon = "fas fa-check";
                    }
                }
                else // !editMode (Insert)
                {
                    ToastTitle = $"{L["creationMsg"]} {L["TituloInquilino"]}";

                    var insertOk = await inquilinoService!.InsereInquilino(SelectedTenant!);
                    if (insertOk == false)
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = $"{L["MSG_ApiError"]}";
                        ToastIcon = "fas fa-times";
                    }
                    else
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = $"{L["SuccessInsert"]}";
                        ToastIcon = "fas fa-check";
                    }

                }
            }
            else
            {
                //AddEditTenantVisibility = true;
                ErrorVisibility = true;
                await Task.Delay(100);
                await SpinnerObj!.HideAsync();
            }

            AddEditTenantVisibility = false;
            Tenants = await GetAllTenants();
            StateHasChanged();

            await Task.Delay(100);
            await ToastObj!.ShowAsync();
        }

        public async Task SaveFiadorData()
        {
            IsDirty = false;
            ErrorVisibility = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationsMessages = validatorService.ValidateFiadorEntries(SelectedGuarantor!);

            if (ValidationsMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    ToastTitle = "Gravar dados do  Fiador ";

                    var updateOk = await FiadorService.AtualizaFiador(SelectedGuarantor!.Id, SelectedGuarantor);
                    if (!updateOk)
                    {
                        AlertVisibility = true;
                        alertTitle = "Atualização de dados do Fiador";
                        WarningMessage = $"{L["MSG_ApiError"]}";
                    }
                    else
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = $"{L["SuccessUpdate"]}";
                        ToastIcon = "fas fa-check";
                    }
                }
                else // !editMode (Insert)
                {
                    ToastTitle = "Criar Fiador";

                    var insertOk = await FiadorService!.InsereFiador(SelectedGuarantor!);
                    if (insertOk == false)
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = $"{L["MSG_ApiError"]}";
                        ToastIcon = "fas fa-times";
                    }
                    else
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = $"{L["SuccessInsert"]}";
                        ToastIcon = "fas fa-check";
                    }
                }
            }
            else
            {
                //AddEditTenantVisibility = true;
                ErrorVisibility = true;
                await Task.Delay(100);
                await SpinnerObj!.HideAsync();
            }

            AddEditFiadorVisibility = false;
            Guarantors = await FiadorService!.GetAll();

            StateHasChanged();

            await Task.Delay(100);
            await ToastObj!.ShowAsync();
        }

        public async Task SaveTenantDocument()
        {
            if (RecordMode == OpcoesRegisto.Gravar)
            {
                alertTitle = "Atualizar dados do documento";

                bool updateOk = await inquilinoService!.AtualizaDocumentoInquilino(TenantDocument!.Id, TenantDocument!);
                if (!updateOk)
                {
                    ToastCss = "e-toast-danger";
                    ToastMessage = $"{L["MSG_ApiError"]}";
                    ToastIcon = "fas fa-times";
                }
                else
                {
                    ToastTitle = "Gravar dados do Documento";
                    ToastCss = "e-toast-success";
                    ToastMessage = $"{L["SuccessUpdate"]}";
                    ToastIcon = "fas fa-check";
                }
            }
            else
            {
                ToastTitle = "Criação de documento";

                TenantDocumentId = await inquilinoService!.CriaDocumentoInquilino(TenantDocument!);
                if (TenantDocumentId < 1)
                {
                    ToastCss = "e-toast-danger";
                    ToastMessage = $"{L["MSG_ApiError"]}";
                    ToastIcon = "fas fa-times";
                }
                else
                {
                    ToastCss = "e-toast-success";
                    ToastMessage = $"{L["SuccessInsert"]}";
                    ToastIcon = "fas fa-check";
                }
            }

            AddEditDocumentVisibility = false;

            StateHasChanged();
            await Task.Delay(100);
            await ToastObj!.ShowAsync();
        }

        private void CheckIfTenantData_Changed()
        {
            var comparer = new ObjectsComparer.Comparer<InquilinoVM>();
            IEnumerable<Difference> differences;
            comparer.Compare(SelectedTenant!, OriginalTenantData!, out differences);
            IsDirty = differences.Any();
        }

        /// <summary>
        /// Inicializa dados do inquilino/fiador
        /// </summary>
        /// <param name="args"></param>
        public void onAddTenant(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            RecordMode = OpcoesRegisto.Inserir;
            NewCaption = $"{L["NewMsg"]} {L["TituloInquilino"]}";

            SelectedTenant = new InquilinoVM()
            {
                Ativo = true,
                Contacto1 = "",
                Contacto2 = "",
                DataNascimento = DateTime.Now,
                eMail = "",
                Identificacao = "",
                ID_EstadoCivil = 1,
                IRSAnual = 0,
                Morada = "",
                Naturalidade = "",
                NIF = "",
                Nome = "",
                Notas = "",
                SaldoCorrente = 0,
                Titular = true,
                ValidadeCC = DateTime.Now,
                Vencimento = 0
            };

            AddEditTenantVisibility = true;
        }



        public void onAddTenantDocument(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            RecordMode = OpcoesRegisto.Inserir;
            NewCaption = $"{L["NewMsg"]} {L["TituloDocumento"]}";
            TenantDocument = new DocumentoInquilinoVM()
            {
                Descricao = "",
                DocumentPath = "",
                TenantId = TenantId,
                CreationDate = DateTime.Now,
                ReferralDate = DateTime.Now,
                DocumentType = 6, // Arquivos de locatário
                StorageType = 'C',
                StorageFolder = "tenants"
            };

            AddEditDocumentVisibility = true;
        }

        /// <summary>
        /// Inicializa dados do Fiador/fiador
        /// </summary>
        /// <param name="args"></param>
        public void onAddGuarantor(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            RecordMode = OpcoesRegisto.Inserir;
            NewCaption = $"{L["NewMsg"]} {L["TituloFiador"]}";
            modulo = Modules.Fiadores;

            SelectedGuarantor = new FiadorVM()
            {
                Ativo = true,
                IdInquilino = TenantId,
                Contacto1 = "",
                Contacto2 = "",
                eMail = "",
                Identificacao = "",
                ID_EstadoCivil = 1,
                IRSAnual = 0,
                Morada = "",
                NIF = "",
                Nome = "",
                Notas = "",
                ValidadeCC = DateTime.Now,
                Vencimento = 0
            };

            AddEditFiadorVisibility = true;
        }


        protected void ContinueEditTenant()
        {
            IsDirty = false;
            AddEditTenantVisibility = true;
            StateHasChanged();
        }
        protected void ContinueEditGuarantor()
        {
            IsDirty = false;
            AddEditFiadorVisibility = true;
            StateHasChanged();
        }

        protected async Task UpdateRentYes()
        {
            alertTitle = "Atualização de renda";

            var updateResult = await inquilinoService.AtualizaRendaInquilino(TenantId);
            if (updateResult != null)
            {

                WarningMessage = updateResult;
            }
            else
            {
                WarningMessage = "Erro desconhecido (!) ao processar atualização";
            }

            ConfirmUpdateRentDialogVisibility = false;
            AlertVisibility = true;

        }
        protected void IgnoreTenantChangesAlert()
        {
            IsDirty = false;
            ErrorVisibility = false;
            AddEditTenantVisibility = false;
            StateHasChanged();
        }
        protected void IgnoreGuarantorChangesAlert()
        {
            IsDirty = false;
            ErrorVisibility = false;
            AddEditFiadorVisibility = false;
            StateHasChanged();
        }


        public async Task ConfirmDeleteYes()
        {
            WarningVisibility = false;
            WarningMessage = "";
            bool resultOk = true;

            try
            {
                switch (modulo)
                {
                    case Modules.Inquilinos:
                        ToastTitle = L["DeleteMsg"] + " " +  L["TituloInquilino"]; ;
                        alertTitle = L["DeleteMsg"] + " " + L["TituloInquilino"];
                        resultOk = await inquilinoService!.ApagaInquilino(SelectedTenant!.Id);
                        DeleteTenantVisibility = false;
                        break;
                    case Modules.Fiadores:
                        alertTitle = L["DeleteMsg"] + " " + L["TituloFiador"];
                        ToastTitle = L["DeleteMsg"] + " " + L["TituloFiador"]; ;
                        resultOk = await FiadorService!.ApagaFiador(SelectedGuarantor!.Id);
                        DeleteGuarantorVisibility = false;
                        break;
                }

                if (resultOk)
                {
                    ToastCss = "e-toast-success";
                    ToastMessage = L["SuccessDelete"];
                    ToastIcon = "fas fa-check";

                    Tenants = await GetAllTenants();

                    StateHasChanged();
                    await Task.Delay(100);
                    await ToastObj!.ShowAsync();
                }
                else
                {
                    switch (modulo)
                    {
                        case Modules.Inquilinos:
                            ToastTitle = L["DeleteMsg"] + " " + L["TituloInquilino"]; ;
                            alertTitle = L["DeleteMsg"] + " " + L["TituloInquilino"];
                            WarningMessage = "Inquilino tem contrato de arrendamento ativo! Verifique, p.f.";
                            DeleteCaption = SelectedTenant.Nome;

                            resultOk = await inquilinoService!.ApagaInquilino(SelectedTenant!.Id);

                            AddEditTenantVisibility = false;
                            break;
                        case Modules.Fiadores:
                            WarningMessage = "Erro ao apagar Fiador. Confirme log, p.f.";
                            alertTitle = L["DeleteMsg"] + " " + L["TituloFiador"];
                            ToastTitle = L["DeleteMsg"] + " " + L["TituloFiador"];
                            DeleteCaption = SelectedGuarantor?.Nome;

                            resultOk = await FiadorService!.ApagaFiador(SelectedGuarantor!.Id);

                            AddEditFiadorVisibility = false;
                            break;
                    }

                    AlertVisibility = true;
                }
            }
            catch (Exception exc)
            {
                AlertVisibility = true;
                alertTitle = modulo == Modules.Inquilinos ? L["DeleteMsg"] + " " + L["TituloInquilino"] : L["DeleteMsg"] + " " + L["TituloFiador"];
                WarningMessage = $"{L["FalhaAnulacaoRegisto"]}. {exc.Message}";
            }

            StateHasChanged();
        }

        public void ConfirmDeleteNo()
        {
            switch (modulo)
            {
                case Modules.Inquilinos:
                    DeleteTenantVisibility = false;
                    break;
                case Modules.Fiadores:
                    DeleteGuarantorVisibility = false;
                    break;
            }
        }

        public async Task ConfirmDeleteDocumentYes()
        {
            WarningVisibility = false;
            WarningMessage = "";
            try
            {
                ToastTitle = L["DeleteMsg"] + " " + L["TituloDocumento"];
                DeleteDocumentVisibility = false;
                var resultOk = await inquilinoService!.ApagaDocumentoInquilino(TenantDocument!.Id);
                if (resultOk)
                {
                    ToastCss = "e-toast-success";
                    ToastMessage = L["SuccessDelete"];
                    ToastIcon = "fas fa-check";

                    await Task.Delay(100);
                    await ToastObj.ShowAsync();
                }
                else
                {
                    DeleteDocumentVisibility = false;
                    AlertVisibility = true;
                    alertTitle = L["DeleteMsg"] + " " + L["TituloDocumento"];
                    DeleteCaption = TenantDocument.Descricao;
                    WarningMessage = "Inquilino tem contrato de arrendamento ativo! Verifique, p.f.";
                }
            }
            catch (Exception exc)
            {
                DeleteDocumentVisibility = false;
                AlertVisibility = true;
                alertTitle = L["FalhaAnulacaoRegisto"];
                WarningMessage = $"Não foi possível concluir operação. {exc.Message}";
            }

        }

        public void ConfirmDeleteDocumentNo()
        {
            DeleteDocumentVisibility = false;
        }

        public void CloseValidationErrorBox()
        {
            switch (modulo)
            {
                case Modules.Inquilinos:
                    AddEditTenantVisibility = true;
                    break;
                case Modules.Fiadores:
                    AddEditFiadorVisibility = true;
                    break;
            }
            ErrorVisibility = false;
        }

        protected void CloseEditDialog()
        {
            IsDirty = false;
            ErrorVisibility = false;

            if (modulo == Modules.Inquilinos)
            {
                var comparer_I = new ObjectsComparer.Comparer<InquilinoVM>();
                var currentData_I = SelectedTenant;
                var originalData_I = OriginalTenantData;
                IEnumerable<Difference> differences_I;
                var isEqual_I = comparer_I.Compare(currentData_I!, originalData_I!, out differences_I);
                if (!isEqual_I)
                {
                    IsDirty = true;
                }
                else
                {
                    AddEditTenantVisibility = false;
                }

            }
            else if (modulo == Modules.Fiadores)
            {
                var comparer_f = new ObjectsComparer.Comparer<FiadorVM>();
                var currentData_f = SelectedGuarantor;
                var originalData_f = OriginalGuarantorData;
                IEnumerable<Difference> differences_f;
                var isEqual_f = comparer_f.Compare(currentData_f!, originalData_f!, out differences_f);
                if (!isEqual_f)
                {
                    IsDirty = true;
                }
                else
                {
                    AddEditFiadorVisibility = false;
                }
            }
            StateHasChanged();
        }

        protected async Task EditFiador(int idFiador)
        {
            SelectedGuarantor = await FiadorService!.GetFiador_ById(idFiador);
            AddEditFiadorVisibility = true;
            modulo = Modules.Fiadores;
        }

        protected async Task HideToast()
        {
            await ToastObj!.HideAsync();
        }

        protected async Task<bool> TenantHasUnpaidRents()
        {
            // Verificar se inquilino tem rendas em atraso
            var _payments = await recebimentosService.GetAll();
            var _tenantId = SelectedTenant.Id;
            var tenantHaveOwedPayments = _payments.Where(p => p.Estado == 3 && p.ID_Inquilino == _tenantId).ToList();
            return tenantHaveOwedPayments.Any();
        }

        protected async Task<bool> TenantHasLease()
        {
            // Verificar se inquilino tem arrendamento ativo
            var tenantsWithNoleases = (await inquilinoService.GetInquilinos_SemContrato()).ToList();
            if (tenantsWithNoleases.Any())
            {
                var output = tenantsWithNoleases.Where(t => t.Id == SelectedTenant?.Id).SingleOrDefault();
                if (output is null)
                {
                    return true;
                }

                return false;
            }
            return true;
        }


        protected async Task HandleIssuedLetterConfirmation(DocumentoEmitido letterType)
        {
            switch (letterType)
            {
                case DocumentoEmitido.OposicaoRenovacaoContrato:
                    await IssueContractOppositionLetter();
                    break;
                case DocumentoEmitido.AtualizacaoManualRenda:
                    await IssueRentUpdateLetter();
                    break;
                case DocumentoEmitido.RendasEmAtraso:
                    await IssueLateRentLetter();
                    break;
                default:
                    break;
            }
        }

        protected async Task IssueContractOppositionLetter()
        {

            // TODO - implementar procedimento para 'resposta do inquilino'
            ToastTitle = L["TituloCartaRevogacao"];

            // Verificar se carta já foi enviada
            Lease = (await arrendamentosService
                .GetAll())
                .FirstOrDefault(l => l.ID_Inquilino == TenantId);

            if (Lease?.Id == 0)
            {
                SendLetterDialogVisibility = false;
                alertTitle = ToastTitle; ;
                WarningMessage = "Erro no processo (Id não existe)... Verifique, p.f.";
                AlertVisibility = true;
                return;
            }

            var alreadySent = await arrendamentosService.VerificaSeExisteCartaRevogacao(Lease.Id);

            if (alreadySent)
            {
                SendLetterDialogVisibility = false;
                alertTitle = ToastTitle; ;
                WarningMessage = "Carta já foi enviada... Verifique, p.f.";

                var dateSent = Lease.DataEnvioCartaRevogacao;
                var answerDateExpected = dateSent.AddDays(10); // estão a ser assumidos 10 dias para prazo de resposta. Configurar?

                // Verificar se carta enviada já foi respondida pelo inquilino
                var letterAnswered = await arrendamentosService.VerificaSeExisteRespostaCartaRevogacao(Lease.Id);
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
            var oppositionLetterData = await arrendamentosService!.GetDadosCartaOposicaoRenovacaoContrato(Lease!);
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

                    var registerOk = await arrendamentosService.RegistaCartaOposicao(Lease.Id, documentoARegistar);
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

        protected async Task IssueLateRentLetter()
        {
            DateTime? referralDate = null;
            string tentativa = "";
            ToastTitle = "Carta de aviso - rendas em atraso";

            // TODO - testar se data da emissão está dentro do prazo (3 pagamentos - 90 dias) ??
            // alertar user em conformidade

            // verifica se inquilino tem valores em divida
            try
            {
                var tenantRentPayments = (await recebimentosService.GetAll()).ToList();
                var tenantDuePayments = tenantRentPayments.Where(p => p.ID_Inquilino == TenantId && p.ValorEmFalta > 0);
                if (tenantDuePayments.Any() == false)
                {
                    alertTitle = "Envio de carta ao inquilino";
                    WarningMessage = "Inquilino não tem pagamentos em atraso!. Verifique, p.f.";
                    AlertVisibility = true;
                    return;
                }

                List<DateTime> dueLettersSent = new();

                var dueMonths = tenantDuePayments.Count();


                DocumentoInquilinoVM? lettersSent = new();

                foreach (var duePayment in tenantDuePayments)
                {

                    var tenantDocuments = await inquilinoService.GetDocumentosInquilino(TenantId);
                    lettersSent = tenantDocuments
                        .FirstOrDefault(l => l.ReferralDate.Month == duePayment.DataMovimento.Month &&
                        l.ReferralDate.Year == duePayment.DataMovimento.Year);

                    if (lettersSent is not null)
                    {
                        dueLettersSent.Add(duePayment.DataMovimento);
                    }
                }

                var countLettersSent = dueLettersSent.Count;
                if (countLettersSent > 0 && countLettersSent == dueMonths)
                {
                    var monthAsString = dueLettersSent.Select(ds => ds.Date.ToString("MMMM").ToTitleCase());
                    var yearAsString = dueLettersSent.Select(ds => ds.Date.ToString("yyyy"));
                    alertTitle = "Envio de carta ao inquilino";
                    WarningMessage = $"Carta de alerta para pagamento em atraso (mês {monthAsString}  de {yearAsString}) já foi enviada!. Verifique, p.f.";
                    AlertVisibility = true;
                    return;
                }

                if (dueMonths > 2 && dueLettersSent.Count < dueMonths - 1)
                {
                    alertTitle = "Envio de carta ao inquilino";
                    WarningMessage = "Há cartas de aviso que não foram enviadas!. Verifique, p.f.";
                    AlertVisibility = true;
                    return;

                }

                var dueLettersSentCount = dueLettersSent.Count;
                if (dueLettersSentCount > 0)
                {
                    var dueLetter = dueLettersSent.FirstOrDefault();
                    var dueLettersSentDateMonth = dueLetter.Date.Month;
                    var dueLettersSentDateYear = dueLetter.Date.Year;
                    var referralEntry = tenantDuePayments
                        .FirstOrDefault(p => p.DataMovimento.Month != dueLettersSentDateMonth &&
                                p.DataMovimento.Year == dueLettersSentDateYear);

                    if (referralEntry != null)
                    {
                        tentativa = dueLettersSentCount == 1 ? "2ª tentiva" : "3ª tentativa";
                        referralDate = referralEntry?.DataMovimento;
                    }
                }
                else // 1ª carta a enviar
                {
                    tentativa = "1ª tentiva";
                    referralDate = tenantDuePayments?.FirstOrDefault().DataMovimento;
                }


                Lease = (await arrendamentosService
                    .GetAll())
                    .FirstOrDefault(l => l.ID_Inquilino == TenantId);

                var leaseId = Lease.Id;

                // Verificar se foi enviada alguma carta 
                // comentado, já não faz sentido validação, uma vez que podem ser enviadas 3 cartas

                //var letterAlreadySent = await arrendamentosService.VerificaEnvioCartaAtrasoEfetuado(leaseId);
                //if (letterAlreadySent)
                //{
                //    alertTitle = "Envio de carta ao inquilino";
                //    WarningMessage = "Carta já foi enviada!. Verifique, p.f.";
                //    AlertVisibility = true;
                //    return;
                //}

                var latePaymentLetterData = await arrendamentosService!.GetDadosCartaRendasAtraso(Lease);
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
                            await arrendamentosService.RegistaCartaAtrasoRendas(leaseId, referralDate, tentativa, documentoARegistar);
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.ToString());
                throw;
            }
        }

        protected async Task IssueRentUpdateLetter()
        {
            DateTime? referralDate = null;
            string tentativa = "";
            ToastTitle = "Carta de atualização de rendas para o inquilino";
            alertTitle = ToastTitle;

            try
            {
                Lease = (await arrendamentosService
                    .GetAll())
                    .FirstOrDefault(l => l.ID_Inquilino == TenantId);

                var leaseId = Lease.Id;
                var allrentUpdates = await inquilinoService.GetAllRentUpdates();

                if(allrentUpdates == null)
                {
                    AlertVisibility = true;
                    WarningMessage = "Não foi feito qualquer aumento de renda";
                    return;
                }

                var tenantRentUpdates = await inquilinoService.GetRentUpdates_ByTenantId(TenantId);
                if(tenantRentUpdates is null)
                {
                    AlertVisibility = true;
                    WarningMessage = "Não foi feito qualquer aumento de renda para o Inquilino";
                    return;
                }
                var currentYearUpdateValues = tenantRentUpdates.FirstOrDefault(r => r.DateProcessed.Year == DateTime.Now.Year);
                if (currentYearUpdateValues is null)
                {
                    AlertVisibility = true;
                    WarningMessage = "Não foi feito qualquer aumento de renda para o ano corrente";
                    return;
                }

                InquilinoVM DadosInquilino = await inquilinoService.GetInquilino_ById(TenantId);
                var currentYearAsString = DateTime.Now.Year.ToString();
                var ValorRenda = currentYearUpdateValues!.PriorValue;
                var NovoValorRenda = currentYearUpdateValues.UpdatedValue;


                var rentUpdateData = await inquilinoService!.GetDadosCartaAtualizacaoInquilino(Lease);
                if (rentUpdateData is not null)
                {
                    rentUpdateData.ValorRenda = ValorRenda;
                    rentUpdateData.NomeInquilino = DadosInquilino.Nome;
                    rentUpdateData.NovoValorRenda = NovoValorRenda;

                    var docGerado = await inquilinoService.EmiteCartaAtualizacaoInquilino(rentUpdateData);
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
                            var creationOk = await inquilinoService.CriaCartaAtualizacaoInquilinoDocumentosInquilino(TenantId, documentoARegistar);
                            if (creationOk)
                            {
                                ToastMessage = L["TituloOperacaoOk"];
                                ToastCss = "e-toast-success";
                            }
                            else
                            {
                                ToastMessage = L["TituloOperacaoComErro"];
                                ToastCss = "e-toast-danger";
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Erro ao registar envio de carta na BD ({ex.Message}). Processo terminou com erro! Contacte Administrador, p.f.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex.ToString());
                throw;
            }
        }

        protected async Task HandleIssuedLetterCancelation()
        {
            SendLetterDialogVisibility = false;
            await tenantsGridObj!.ClearSelectionAsync();
        }


        protected async Task RowSelectHandler(RowSelectEventArgs<InquilinoVM> args)
        {
            TenantId = args.Data.Id;
            SelectedTenant = await inquilinoService!.GetInquilino_ById(TenantId);

            ShowToolbar_LetterOptions();
        }

        protected void RowDeselectHandler(RowDeselectEventArgs<InquilinoVM> args)
        {
            HideToolbar_LetterOptions();
        }

        protected void HandleRentChange(decimal updatedRent)
        {
            NewRentUpdated = updatedRent;
            ConfirmManualUpdateRentDialogVisibility = false;
            ConfirmManualUpdateRentVisibility = updatedRent > 0;
        }

        protected async Task UpdateTenantRent_Manually()
        {
            ConfirmManualUpdateRentVisibility = false;
            AlertVisibility = true;
            alertMessageType = AlertMessageType.Warning;

            var oldRentValue = await inquilinoService.GetTenantRent(TenantId);
            if (NewRentUpdated == 0)
            {
                alertTitle = "Atualizar valor de renda";
                WarningMessage = "Novo valor inválido. Verifique, p.f.";
                return;
            }

            var updateAlreadyMade = await inquilinoService.PriorRentUpdates_ThisYear(TenantId);
            if (updateAlreadyMade)
            {
                alertTitle = "Atualizar valor de renda";
                WarningMessage = "Já foi efetuado um aumento de renda para este ano. Verifique, p.f.";
                return;
            }

            var oldRentValueAsString = oldRentValue.ToString("#,###.00");
            var newRentValueAsString = NewRentUpdated.ToString("#,###.00");
            var resultAsAString = await inquilinoService.AtualizaRendaInquilino_Manual(TenantId, oldRentValueAsString, newRentValueAsString);
            if (string.IsNullOrEmpty(resultAsAString)) // sucesso; se preenchido, devolve erro
            {
                alertTitle = "Atualizar valor de renda";
                WarningMessage = "Operação terminou com sucesso.";
                alertMessageType = AlertMessageType.Info;
            }
            else
            {
                alertTitle = "Atualizar valor de renda";
                WarningMessage = $"Operação terminou com erro ({resultAsAString})";
                alertMessageType = AlertMessageType.Error;
            }

            StateHasChanged();

        }
        private void HideToolbar_LetterOptions()
        {
            ShowToolbarDueRentLetter = false;
            ShowToolbarContractRevocationLetter = false;
        }
        private void ShowToolbar_LetterOptions()
        {
            ShowToolbarDueRentLetter = true;
            ShowToolbarContractRevocationLetter = true;
        }

        public async Task ToggleRow()
        {
            var selRows = tenantsGridObj.GetSelectedRowIndexesAsync();
            await tenantsGridObj!.SelectRowAsync(1, true);
        }

        private async Task ShowToastMessage()
        {
            StateHasChanged();
            await Task.Delay(100);
            await ToastObj!.ShowAsync();
        }


        public void Dispose()
        {
            SpinnerObj?.Dispose();
            tenantsGridObj?.Dispose();
            ToastObj?.Dispose();
        }
    }
}