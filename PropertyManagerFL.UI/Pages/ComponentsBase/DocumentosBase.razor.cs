using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using ObjectsComparer;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Documentos;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;
using PdfFontFamily = Syncfusion.Pdf.Graphics.PdfFontFamily;
using PdfNumberStyle = Syncfusion.Pdf.PdfNumberStyle;
using PdfPageNumberField = Syncfusion.Pdf.PdfPageNumberField;
using PdfStandardFont = Syncfusion.Pdf.Graphics.PdfStandardFont;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class DocumentosBase : ComponentBase, IDisposable
    {

        /// <summary>
        /// Documents service
        /// </summary>
        [Inject] public IDocumentosService? DocumentsService { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }
        [Inject] protected IStringLocalizer<App>? L { get; set; }
        [Inject] protected NavigationManager? NavigationManager { get; set; }
        [Inject] protected IJSRuntime? JSRuntime { get; set; }
        [Inject] protected ILogger<App>? _logger{ get; set; }


        /// <summary>
        /// list of Documents
        /// </summary>
        protected IEnumerable<DocumentoVM>? Documents { get; set; }
        public DocumentoVM? SelectedDocument { get; set; }
        protected DocumentoVM? OriginalDocumentData { get; set; }
        protected OpcoesRegisto RecordMode { get; set; }
        protected int DocumentId { get; set; }
        protected string? NewCaption { get; set; }
        protected string? EditCaption { get; set; }
        protected string? DeleteCaption;

        protected bool AddEditVisibility { get; set; }
        protected bool DeleteVisibility { get; set; }
        protected bool WarningVisibility { get; set; }
        protected string? WarningMessage { get; set; }
        public bool ErrorVisibility { get; set; } = false;
        protected bool ShowPdfVisibility { get; set; }
        protected string? PdfFilePath { get; set; }

        public DialogEffect Effect = DialogEffect.Zoom;
        protected SfSpinner? SpinnerObj { get; set; }
        protected SfToast? ToastObj { get; set; }
        protected SfGrid<DocumentoVM>? DocumentsGridObj { get; set; }

        protected bool IsDirty = false;
        protected List<string>? ValidationMessages = new();
        protected string? WarningTitle;
        protected AlertMessageType alertMessageType = AlertMessageType.Info;

        protected string? ToastTitle;
        protected string? ToastMessage;
        protected string? ToastCss;
        protected string? ToastIcon;

        protected string? documentName;
        protected string? documentCategory;
        protected string? documentDescription;
        protected string? documentsFolder = string.Empty;

        protected bool localUpload = true;
        protected string DeleteMsg { get; set; } = string.Empty;
        protected string DirtyMsg { get; set; } = string.Empty;

        protected int primaryIndex;

        /// <summary>
        /// Documentos
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            ToastTitle = "";
            ToastMessage = "";
            ToastCss = "";
            ToastIcon = "";
            documentName = "";
            documentDescription = string.Empty;
            documentsFolder = "";
            OriginalDocumentData = new();

            DeleteMsg = L["DeleteMsg"] + " " + L["TituloDocumento"];
            DirtyMsg = L["TituloDadosAlterados"] + " " + L["MSG_ConfirmarOperacao"];

            AddEditVisibility = false;
            DeleteVisibility = false;
            WarningVisibility = false;
            WarningMessage = "";
            DocumentId = 0;
            IsDirty = false;
            localUpload = true;
            Documents = await GetAllDocuments();
            if (!Documents.Any())
            {
                WarningMessage = "Sem dados para mostrar";
                WarningVisibility = true;
            }

        }

        /// <summary>
        /// Get all Documents
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DocumentoVM>> GetAllDocuments()
        {
            IEnumerable<DocumentoVM>? DocumentsList = await DocumentsService.GetAll();
            DocumentsList.OrderByDescending(p => p.Id).ToList();
            return DocumentsList;
        }

        public async void OnRowSelected(RowSelectEventArgs<DocumentoVM> args)
        {
            SelectedDocument = args.Data;
            primaryIndex = await DocumentsGridObj!.GetRowIndexByPrimaryKeyAsync(SelectedDocument.Id);
        }
        protected async Task OnDocumentDoubleClickHandler(RecordDoubleClickEventArgs<DocumentoVM> args)
        {
            DocumentId = args.RowData.Id;
            SelectedDocument = await DocumentsService!.GetDocument_ById(DocumentId);
            AddEditVisibility = true;
            EditCaption = L["editionMsg"] + " " + L["TituloDocumento"]; ;
            RecordMode = OpcoesRegisto.Gravar;
            SelectedDocument = await DocumentsService!.GetDocument_ById(DocumentId);
            OriginalDocumentData = await DocumentsService.GetDocument_ById(DocumentId); // TODO should use 'Clone/MemberWise'

            primaryIndex = await DocumentsGridObj!.GetRowIndexByPrimaryKeyAsync(SelectedDocument.Id);
            if (primaryIndex >= 0)
            {
                await DocumentsGridObj.SelectRowAsync(primaryIndex);
            }
        }

        /// <summary>
        /// Command handler
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task OnDocumentCommandClicked(CommandClickEventArgs<DocumentoVM> args)
        {
            var data = args.RowData;

            DocumentId = args.RowData.Id;
            SelectedDocument = await DocumentsService!.GetDocument_ById(DocumentId);
            var documentCategoryId = SelectedDocument.DocumentCategoryId;

            DeleteCaption = SelectedDocument?.Title;

            primaryIndex = await DocumentsGridObj!.GetRowIndexByPrimaryKeyAsync(data.Id);

            if (primaryIndex >= 0)
            {
                await DocumentsGridObj.SelectRowAsync(primaryIndex);
            }


            OriginalDocumentData = await DocumentsService.GetDocument_ById(DocumentId); // TODO should use 'Clone/MemberWise'
            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                AddEditVisibility = true;
                EditCaption = L["editionMsg"] + " " + L["TituloDocumento"]; ;
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteVisibility = true;
                StateHasChanged();
            }
            if (args.CommandColumn.Type == CommandButtonType.None)
            {

                documentName = SelectedDocument!.URL;
                documentDescription = SelectedDocument!.Title;
                //documentsFolder = SelectedDocument.DocumentCategory;

                switch (documentCategoryId)
                {
                    case 1:
                        documentsFolder = "properties";
                        break;
                    case 2:
                        documentsFolder = "unit_images";
                        break;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        documentsFolder = "tenants";
                        break;
                }

                RecordMode = OpcoesRegisto.Info;
                var fileName = args.RowData.URL;
                if (fileName!.ToLower().StartsWith("http"))
                {
                    try
                    {
                        await JSRuntime!.InvokeAsync<object>("open", fileName, "_blank");
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError($"Abertura de documento em nova página\n ({ex.Message})");
                    }
                }
                else
                {
                    PdfFilePath = DocumentsService.GetPdfFilename(documentsFolder!.Trim(), fileName!);
                    if (string.IsNullOrEmpty(PdfFilePath))
                    {
                        ShowPdfVisibility = false;

                        ToastTitle = "Leitura de pdf";
                        ToastMessage = "Ficheiro não existe no local indicado! Verifique, p.f.";
                        ToastCss = "e-toast-danger";
                        ShowPdfVisibility = false;
                        StateHasChanged();
                        await Task.Delay(100);
                        await ToastObj!.ShowAsync();
                    }
                    else
                    {
                        ShowPdfVisibility = true;
                    }
                }
            }
        }

        protected async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "C_Grid_pdfexport")  //Id is combination of Grid's ID and itemname
            {
                await PdfExport();
            }
        }

        PdfPageNumberField pageNumber = new PdfPageNumberField()
        {
            NumberStyle = PdfNumberStyle.Numeric,
            Font = new PdfStandardFont(PdfFontFamily.Helvetica, 7)
        };

        protected List<PdfHeaderFooterContent> FooterContent = new List<PdfHeaderFooterContent>
        {
                new PdfHeaderFooterContent()
                {
                    Type = ContentType.PageNumber, Value = $"Emissão: {DateTime.Now.ToShortDateString()}",
                    Position = new PdfPosition() { X = 250, Y = 20
                 },
                 Style = new PdfContentStyle()
                 {
                        TextBrushColor = "#C67878", FontSize = 14 },
                        PageNumberType = PdfPageNumberType.Numeric
                 },
        };

        protected List<PdfHeaderFooterContent> HeaderContent = new List<PdfHeaderFooterContent>
        {
            new PdfHeaderFooterContent()
            {
                Type = ContentType.Text,
                Value = "Lista de Documentos",
                Position = new PdfPosition()
                 {
                        X = 0,
                        Y = 0
                },
                Style = new PdfContentStyle()
                {
                    TextBrushColor = "#000000", FontSize = 13
                },
            },
            new PdfHeaderFooterContent()
            {
                Type = ContentType.Line,
                Points = new PdfPoints()
                {
                    X1 = 0,
                    Y1 = 14,
                    X2 = 685,
                    Y2 = 14
                },
                Style = new PdfContentStyle()
                {
                    FontSize= 10,
                    PenColor = "#000080",
                    DashStyle = Syncfusion.Blazor.Grids.PdfDashStyle.Solid,
                },
             }
        };

        protected async Task PdfExport()
        {
            PdfExportProperties ExportProperties = new PdfExportProperties();
            ExportProperties.Header = new PdfHeader()
            {
                FromTop = 0,
                Height = 60,
                Contents = HeaderContent
            };

            ExportProperties.Footer = new PdfFooter()
            {
                Height = 40,
                Contents = FooterContent,
                FromBottom = 160
            };

            ExportProperties.DisableAutoFitWidth = true;
            ExportProperties.FileName = $"AppDocument_{DateTime.Now.ToFileTime()}.pdf";

            //Below code is to customize the columns width for the pdf exported grid irrespective of the actual grid columns width

            ExportProperties.Columns = new List<GridColumn>()
            {
                new GridColumn(){ Field="Title", HeaderText="Título", Width="250"},
                new GridColumn(){ Field="Description", HeaderText="Descrição", Width="250"},
            };

            await DocumentsGridObj!.ExportToPdfAsync(ExportProperties);

        }

        /// <summary>
        /// Grava registo
        /// </summary>
        /// <returns></returns>
        public async Task SaveDocumentData()
        {
            IsDirty = false;
            ErrorVisibility = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationMessages = validatorService.ValidateDocumentEntries(SelectedDocument!);

            if (ValidationMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    ToastTitle = L["btnSalvar"] + " " + L["TituloDocumento"];

                    var updateOk = await DocumentsService!.UpdateDocument(SelectedDocument!.Id, SelectedDocument);
                    if (updateOk)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["MSG_ApiError"];
                        ToastIcon = "fas fa-exclamation";
                    }

                }

                else // !editMode (Insert)
                {
                    ToastTitle = L["NewMsg"] + " " + L["TituloDocumento"];

                    var insertOk = await DocumentsService!.InsertDocument(SelectedDocument!);
                    if (insertOk)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["MSG_ApiError"];
                        ToastIcon = "fas fa-exclamation";
                    }

                    //IsDirty = true;
                }

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

                AddEditVisibility = false;
                Documents = await GetAllDocuments();
                await DocumentsGridObj!.Refresh();
            }
            else
            {
                await Task.Delay(100);
                ErrorVisibility = true;
                await Task.Delay(100);
                await SpinnerObj!.HideAsync();
            }
        }

        private void CheckIfDocumentData_Changed()
        {
            var comparer = new ObjectsComparer.Comparer<DocumentoVM>();
            IEnumerable<Difference> differences;
            comparer.Compare(SelectedDocument!, OriginalDocumentData!, out differences);
            IsDirty = differences.Any();
        }

        /// <summary>
        /// Inicializa dados do Documento/fiador
        /// </summary>
        /// <param name="args"></param>
        public void onAddDocument(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            RecordMode = OpcoesRegisto.Inserir;
            NewCaption = L["NewMsg"] + " " + L["TituloDocumento"];

            SelectedDocument = new DocumentoVM()
            {
                CreatedBy = "Fausto",
                CreatedOn = DateTime.Now,
                LastModifiedBy = "Fausto",
                LastModifiedOn = DateTime.Now,
                Description = "",
                DocumentTypeId = 0,
                IsPublic = true,
                LocalUpload = true,
                Title = "",
                URL = "",
            };

            AddEditVisibility = true;
        }

        /// <summary>
        /// Fecha diálogo de criação / edição do Documento
        /// Verifica se registo foi alterado
        /// </summary>
        protected void CloseEditDialog()
        {
            IsDirty = false;

            //var comparer = new ObjectsComparer.Comparer<DocumentoVM>();
            //var currentData = SelectedDocument;
            //var originalData = OriginalDocumentData;
            //IEnumerable<Difference> differences;
            //var isEqual_P = comparer.Compare(currentData!, originalData!, out differences);
            //if (!isEqual_P)
            //{
            //    IsDirty = true;
            //}
            //else
            //{
            AddEditVisibility = false;
            //}
        }

        protected void ContinueEdit()
        {
            IsDirty = false;
            AddEditVisibility = true;
        }

        protected void IgnoreChangesAlert()
        {
            IsDirty = false;
            ErrorVisibility = false;
            AddEditVisibility = false;
        }

        public async Task ConfirmDeleteYes()
        {
            WarningVisibility = false;
            WarningMessage = "";
            ToastTitle = L["DeleteMsg"] + " " + L["TituloDocumento"];

            try
            {
                DeleteVisibility = false;
                var resultOk = await DocumentsService!.DeleteDocument(SelectedDocument!.Id);
                if (resultOk)
                {
                    Documents = await GetAllDocuments();
                    await DocumentsGridObj!.Refresh();
                    ToastCss = "e-toast-success";
                    ToastMessage = L["TituloOperacaoOk"];
                    ToastIcon = "fas fa-check";
                }
                else
                {
                    ToastCss = "e-toast-danger";
                    ToastMessage = L["MSG_ApiError"];
                    ToastIcon = "fas fa-exclamation";

                    //WarningVisibility = true;
                    //WarningMessage = $"Apagar Documento - não foi possível concluir operação...";
                }

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

            }
            catch (Exception exc)
            {

                ToastTitle = "Error";
                ToastCss = "e-toast-danger";
                ToastMessage = L["MSG_ApiError"];
                ToastIcon = "fas fa-exclamation";

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

                //WarningVisibility = true;
                //WarningMessage = $"Não foi possível concluir operação. {exc.Message}";
            }
        }

        public void ConfirmDeleteNo()
        {
            DeleteVisibility = false;
        }


        /// <summary>
        ///  Fecha diálogo de validação
        /// </summary>
        public void CloseValidationErrorBox()
        {
            ErrorVisibility = false;
            AddEditVisibility = true;
        }

        protected async Task HideToast()
        {
            await ToastObj!.HideAsync();
        }

        public void Dispose()
        {
            SpinnerObj?.Dispose();
            ToastObj?.Dispose();
            DocumentsGridObj?.Dispose();
        }
    }
}
