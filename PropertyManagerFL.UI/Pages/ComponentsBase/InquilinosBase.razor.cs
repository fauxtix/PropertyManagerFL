using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ObjectsComparer;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Fiadores;
using PropertyManagerFL.Application.ViewModels.Inquilinos;
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
        [Inject] public IFiadorService? FiadorService { get; set; }
        [Inject] public IRecebimentoService? recebimentosService { get; set; }
        [Inject] public IArrendamentoService? arrendamentoService { get; set; }
        [Inject] public IWebHostEnvironment _env { get; set; }
        [Inject] public NavigationManager nav { get; set; }
        [Inject] protected IValidationService validatorService { get; set; }
        [Inject] protected IStringLocalizer<App> L { get; set; }


        /// <summary>
        /// list of tenants
        /// </summary>
        protected IEnumerable<CC_InquilinoVM>? TenantPaymentsHistory { get; set; }
        protected IEnumerable<InquilinoVM>? Tenants { get; set; }
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
        protected AlertMessageType? alertMessageType = AlertMessageType.Info;
        protected string? alertTitle = "";

        protected string? tenantName { get; set; }
        protected int MaxFileSize = 5 * 1024 * 1024; // 5 MB

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


            Tenants = await GetAllTenants();
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
        public async Task<IEnumerable<InquilinoVM>> GetAllTenants()
        {
            IEnumerable<InquilinoVM>? tenantsList = await inquilinoService!.GetAll();
            // tenantsList.OrderByDescending(p => p.Id).ToList();
            return tenantsList.ToList();
        }

        public IEnumerable<DocumentoInquilinoVM> GetTenantDocuments(int id)
        {

            IEnumerable<DocumentoInquilinoVM> documentsList = Task.Run(async () => await inquilinoService!.GetDocumentosInquilino(id)).Result;
            return documentsList;
        }

        public IEnumerable<CC_InquilinoVM> GetTenantPaymentHistory(int id)
        {

            IEnumerable<CC_InquilinoVM> paymentsList = Task.Run(async () => await inquilinoService!.GetTenantPaymentsHistory(id)).Result;
            return paymentsList;
        }


        public IEnumerable<FiadorVM> GetTenantGuarantors(int id)
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



            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                SelectedTenant = args.RowData;
                OriginalTenantData = await inquilinoService.GetInquilino_ById(TenantId); // TODO should use 'Clone/MemberWise'

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
            if (args.Item.Text == "Collapse all")
            {
                await tenantsGridObj.CollapseAllDetailRowAsync();
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
                EditCaption = $"{L["EditMsg"]} {L["TituloDocumento]"]}";
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteDocumentVisibility = true;
                DeleteCaption = $"{L["DeleteMsg"]} {L["TituloDocumento]"]}";
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
                    documentFilePath = Path.Combine(_env.WebRootPath, "uploads", folderName!, fileName!);
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
                UploadDate = DateTime.Now,
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
                        ToastTitle = L["DeleteMsg"] + L["TituloInquilino"]; ;
                        alertTitle = L["DeleteMsg"] + L["TituloInquilino"];
                        resultOk = await inquilinoService!.ApagaInquilino(SelectedTenant!.Id);
                        DeleteTenantVisibility = false;
                        break;
                    case Modules.Fiadores:
                        alertTitle = L["DeleteMsg"] + L["TituloFiador"];
                        ToastTitle = L["DeleteMsg"] + L["TituloFiador"]; ;
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
                            ToastTitle = L["DeleteMsg"] + L["TituloInquilino"]; ;
                            alertTitle = L["DeleteMsg"] + L["TituloInquilino"];
                            WarningMessage = "Inquilino tem contrato de arrendamento ativo! Verifique, p.f.";
                            DeleteCaption = SelectedTenant.Nome;

                            resultOk = await inquilinoService!.ApagaInquilino(SelectedTenant!.Id);

                            AddEditTenantVisibility = false;
                            break;
                        case Modules.Fiadores:
                            WarningMessage = "Erro ao apagar Fiador. Confirme log, p.f.";
                            alertTitle = L["DeleteMsg"] + L["TituloFiador"];
                            ToastTitle = L["DeleteMsg"] + L["TituloFiador"];
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
                alertTitle = modulo == Modules.Inquilinos ? L["DeleteMsg"] + L["TituloInquilino"]:  L["DeleteMsg"] + L["TituloFiador"];
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

        public void Dispose()
        {
            SpinnerObj?.Dispose();
            tenantsGridObj?.Dispose();
            ToastObj?.Dispose();
        }
    }
}