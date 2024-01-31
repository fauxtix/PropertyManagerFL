using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.Documentos;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Inputs;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.Documentos;
public partial class AddEditDocumento
{

    [Inject] public ILookupTableService? LookupTablesService { get; set; }
    [Inject] public IDocumentosService? documentsService { get; set; }
    [Inject] public IStringLocalizer<App> L { get; set; }

    [Parameter] public DocumentoVM? Document { get; set; }
    [Parameter] public OpcoesRegisto EditMode { get; set; }
    [Parameter] public string? HeaderCaption { get; set; }

    protected int idxTipoDocumento = 0;
    protected int idxTipoCategoriaDocumento = 0;

    protected bool HideDocumentCategory = true;
    protected bool HideUploader = false;
    protected bool HideUploadedFile = false;

    protected bool ShowTipoDocumento;

    protected string? documentCategoryCaption;
    protected string? documentCategoryFolder;
    protected string? uploadedFile;
    protected string? documentTypeCaption;

    public IEnumerable<LookupTableVM>? DocumentCategories { get; set; }
    public IEnumerable<DocumentType>? DocumentTypes { get; set; }

    protected SfUploader? sfUploader;
    int MaxFileSize = 10 * 1024 * 1024; // 10 MB

    protected string controllerName_Save = "";
    protected string controllerName_Remove = "";
    protected string uploaderUrl_Save = string.Empty;
    protected string uploaderUrl_Remove = string.Empty;


    protected string PdfOrUrlCaption = "";

    protected bool ErrorVisibility { get; set; } = false;
    protected List<string> ValidationsMessages = new();



    protected Dictionary<string, object> NotesAttribute = new Dictionary<string, object>()
{
        {"rows", "3" }
};

    protected override async Task OnParametersSetAsync()
    {
        DocumentCategories = (await LookupTablesService!.GetLookupTableData("DocumentTypeCategories")).ToList();
        DocumentTypes = (await GetDocumentTypes());
        HideUploader = false;

        PdfOrUrlCaption = Document.URL.ToLower().EndsWith("pdf") ? "Pdf" : "Url do site";
        if (EditMode == OpcoesRegisto.Gravar)
        {

            idxTipoCategoriaDocumento = Document!.DocumentTypeId;

            idxTipoDocumento = DocumentTypes.FirstOrDefault(o => o.Id == idxTipoCategoriaDocumento).TypeCategoryId;
            documentTypeCaption = DocumentTypes?.FirstOrDefault(o => o.Id == idxTipoCategoriaDocumento)?.Name;
            documentCategoryCaption = DocumentCategories?.FirstOrDefault(c => c.Id == idxTipoDocumento)?.Descricao?.Trim();
            HideUploader = true;
            uploadedFile = Document.URL;
        }
        HideDocumentCategory = true;
        //        HideUploadedFile = true;

        ShowTipoDocumento = false;
    }

    protected async Task onChangeDocumentTypeCategories(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, LookupTableVM> args)
    {
        ShowTipoDocumento = true;
        idxTipoCategoriaDocumento = args.Value;
        Document!.DocumentCategoryId = idxTipoCategoriaDocumento;

        DocumentTypes = DocumentTypes?.Where(dt => dt.TypeCategoryId == idxTipoCategoriaDocumento);
        documentCategoryFolder = DocumentCategories?.SingleOrDefault(dc => args.Value == dc.Id).Descricao.Trim();

        StateHasChanged();
    }

    protected void onChangeDocumentType(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, DocumentType> args)
    {
        if (args == null || args.Value == 0) return;
        idxTipoDocumento = args.Value;
        Document.DocumentTypeId = idxTipoDocumento;
        HideDocumentCategory = false;
        if (EditMode != OpcoesRegisto.Inserir)
            return;

        HideUploader = false;
        // if (string.IsNullOrEmpty(documentCategoryFolder))
        // {
        //     documentCategoryFolder = DocumentCategories.SingleOrDefault(dc => args.Value == dc.Id).Descricao.Trim();
        // }

        // uploaderUrl_Save = $"api/Upload/SaveFile?folder={documentCategoryFolder}";
        // uploaderUrl_Remove = $"api/Upload/RemoveFile?folder={documentCategoryFolder}";


        switch (idxTipoCategoriaDocumento)
        {
            case 1:
                controllerName_Save = "api/uploadproperties/save";
                controllerName_Remove = "api/uploadproperties/remove";
                break;
            case 2:
                controllerName_Save = "api/uploadunits/save";
                controllerName_Remove = "api/uploadunits/remove";
                break;
            case 3:
            case 4:
            case 5:
            case 6:
                controllerName_Save = "api/uploadtenantdocuments/save";
                controllerName_Remove = "api/uploadtenantdocuments/remove";
                break;
        }
    }

    private async Task<IEnumerable<DocumentType>> GetDocumentTypes()
    {
        return (await documentsService!.GetAll_DocumentTypes()).ToList();
    }

    private void OnActionCompleteHandler(Syncfusion.Blazor.Inputs.ActionCompleteEventArgs args)
    {
        if (args.FileData.Count() == 0) return;

        if (idxTipoCategoriaDocumento < 1 || idxTipoDocumento < 1)
        {
            ValidationsMessages = new List<string>
        {  "Deve preencher Categoria e Tipo de documento"};
            sfUploader?.ClearAllAsync();
            ErrorVisibility = true;
            return;
        }


        uploadedFile = args.FileData.Select(p => p.Name).FirstOrDefault();
        HideUploadedFile = false;

        Document!.URL = uploadedFile;

        StateHasChanged();
    }

    protected void HandleDocumentSource(ChangeArgs<bool> args)
    {
        var localUpload = args.Value;
        HideUploadedFile = !localUpload;
        StateHasChanged();
    }

}