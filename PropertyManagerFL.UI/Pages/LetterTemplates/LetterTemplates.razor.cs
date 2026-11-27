using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Application.ViewModels.MailMerge;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Helpers;
using Syncfusion.Blazor.DocumentEditor;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.SfPdfViewer;
using System.Text.Json;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.LetterTemplates;
public partial class LetterTemplates
{
    [Inject] ILetterTemplatesService? documentsSevice { get; set; }
    [Inject] public IAppSettingsService? appSettingsService { get; set; }

    [Inject] public IStringLocalizer<App>? L { get; set; }
    [Inject] public ILogger<LetterTemplates>? Logger { get; set; }

    protected List<Template> Templates { get; set; } = new();

    SfDocumentEditorContainer? container;
    SfUploader? uploader;
    string fileName = string.Empty;
    string filePath = string.Empty;
    string ext = string.Empty;
    bool HideErrroMessage = true;
    ImportFormatType formatType;

    protected List<string> FileList { get; set; } = new();
    protected List<TemplateFileName>? FileNamesOnlyList { get; set; } = new();

    protected string? AlertTitle = "";
    protected bool AlertVisibility { get; set; } = false;
    protected string? WarningMessage { get; set; }

    AlertMessageType AlertMessageType = AlertMessageType.Warning;
    OpcoesRegisto OpcoesRegisto = OpcoesRegisto.Warning;
    private ApplicationSettingsVM? appSettings;

    protected int idxFileSelected;

    private string? selectedCulture = Thread.CurrentThread.CurrentCulture.Name;
    private string? currentTemplateName;
    private string? currentTemplateDisplayName;
    private bool showTemplateLanguage;
    private bool PreviewVisible { get; set; }
    private bool PreviewBusy { get; set; }
    private byte[]? PreviewPdfBytes { get; set; }
    private SfPdfViewer2? previewViewer;
    private bool previewLoaded;

    [Inject] public NavigationManager? NavManager { get; set; }

    protected bool IsFullScreen { get; private set; }
    protected bool FieldPanelVisible { get; set; }
    protected bool TemplatesLoaded { get; private set; }
    protected IReadOnlyList<MergeField> AvailableFields =>
        MergeFieldCatalog.GetFieldsForTemplate(currentTemplateName);


    protected List<Object> Items => new List<Object> { "New", new CustomToolbarItemModel(){ Id="save", Text=L?["Guardar"] ?? "Save", PrefixIcon="saveIcon"},
        new CustomToolbarItemModel(){ Id="preview", Text=L?["lt-preview"] ?? "Pré-visualizar", PrefixIcon="fa fa-eye"}, "Undo",
        "Redo", "Comments", "Image", "Table", "Hyperlink", "Bookmark", "Header",
        "Footer", "PageSetup", "PageNumber", "Find", "LocalClipboard", "RestrictEditing"
    };

#pragma warning disable BL0005, CA2000  // Component parameter should not be set outside of its component, Dispose objects before losing scope

    protected override async Task OnInitializedAsync() => await LoadTemplatesAsync();

    private async Task OnPreviewViewerCreated()
    {
        if (!PreviewVisible || previewLoaded || previewViewer is null || PreviewPdfBytes is null)
            return;

        previewLoaded = true;
        try
        {
            await previewViewer.LoadAsync(PreviewPdfBytes, null);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Could not load the letter template preview in the PDF viewer.");
            PreviewVisible = false;
            PreviewPdfBytes = null;
            DisplayResult(L?["lt-preview-title"] ?? "Preview",
                L?["lt-preview-error"] ?? "Could not generate the preview.", AlertMessageType.Error);
        }
    }

    protected async Task ReloadTemplates() => await LoadTemplatesAsync();

    private async Task LoadTemplatesAsync()
    {
        string relativeUrl = NavManager?.ToBaseRelativePath(NavManager.Uri) ?? string.Empty;
        IsFullScreen = relativeUrl.StartsWith("LetterTemplates", StringComparison.OrdinalIgnoreCase);

        formatType = ImportFormatType.Docx;
        idxFileSelected = 0;
        AlertVisibility = false;
        AlertTitle = "";
        WarningMessage = "";
        TemplatesLoaded = false;
        int counter = 0;

        appSettings = await GetSettings();
        selectedCulture = appSettings?.DefaultLanguage;
        if (string.IsNullOrWhiteSpace(selectedCulture))
            selectedCulture = Thread.CurrentThread.CurrentCulture.Name;

        FileList = (await documentsSevice!.GetTemplatesFilenamesFromServer(selectedCulture!)).ToList();
        showTemplateLanguage = TemplateDisplayCatalog.HasMixedLanguages(FileList);
        FileNamesOnlyList = new();
        foreach (var item in FileList)
        {
            var filename = Path.GetFileNameWithoutExtension(item);
            FileNamesOnlyList.Add(new TemplateFileName
            {
                Id = counter++,
                TemplateFile = GetTemplateDisplayName(filename),
                FilePath = item
            });
        }

        idxFileSelected = 0;
        filePath = string.Empty;
        currentTemplateName = string.Empty;
        currentTemplateDisplayName = string.Empty;
        TemplatesLoaded = true;

        if (FileList.Count > 0)
        {
            filePath = FileList[0];
            currentTemplateName = Path.GetFileNameWithoutExtension(filePath);
            currentTemplateDisplayName = GetTemplateDisplayName(currentTemplateName);
        }
        else
        {
            Logger?.LogWarning(
                "No letter templates found for culture '{Culture}'. Check the API connection and the .dotx files in the API's Reports/Docs folder.",
                selectedCulture);
        }

        StateHasChanged();
    }


    public async void OnSuccess(UploadingEventArgs action)
    {
        string? base64 = action.FileData.RawFile.ToString();
        fileName = action.FileData.Name;
        filePath = await documentsSevice!.GetTemplateFromServer(fileName);
        currentTemplateName = Path.GetFileNameWithoutExtension(fileName);
        currentTemplateDisplayName = GetTemplateDisplayName(currentTemplateName);
        ext = Path.GetExtension(fileName);
        if (ext.ToLower() == ".doc")
        {
            formatType = ImportFormatType.Doc;
        }
        string? data = base64?.Split(',')[1];
        byte[] bytes = Convert.FromBase64String(s: data);
        using (Stream? stream = new MemoryStream(bytes))
        {
            WordDocument? document = WordDocument.Load(stream, formatType);
            string? sfdtString = JsonSerializer.Serialize(document);
            document.Dispose();
            document = null;
            SfDocumentEditor editor = container.DocumentEditor;
            await editor.OpenAsync(sfdtString);
            sfdtString = null;
        }
        action.Cancel = true;
    }

    private void OnActionCompleteHandler(ActionCompleteEventArgs args)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            DisplayResult(Localized("lt-file-not-loaded-title"), Localized("lt-file-not-loaded"), AlertMessageType.Warning);
        }
        else
        {
            AlertVisibility = false;
            AlertTitle = "";
            WarningMessage = "";
        }
        _ = uploader.ClearAllAsync();
    }

    private void OnFailure(FailureEventArgs args)
    {
        DisplayResult(Localized("lt-upload-error-title"), args.Response.StatusText, AlertMessageType.Error);
    }

    private async void OnCreated()
    {
        var documentEditor = container.DocumentEditor;

        if (!string.IsNullOrEmpty(filePath) && documentEditor != null)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    DisplayResult(Localized("lt-file-not-loaded-title"),
                        string.Format(Localized("lt-file-not-found"), filePath), AlertMessageType.Error);
                    return;
                }

                using (FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read))
                {
                    WordDocument document = WordDocument.Load(fileStream, ImportFormatType.Docx);
                    await documentEditor.OpenAsync(JsonSerializer.Serialize(document));
                    document.Dispose();
                    document = null;
                }
            }
            catch (Exception ex)
            {
                DisplayResult(string.Format(Localized("lt-file-load-error"), filePath), $"{ex.Message}");
            }
        }
    }

    public async Task onItemClick(ClickEventArgs args)
    {
        switch (args.Item.Id)
        {
            case "save":
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        await SaveFile();
                    }
                    catch (Exception ex)
                    {
                        DisplayResult(Localized("lt-save-error"), ex.Message);
                    }
                }
                break;
            case "preview":
                await PreviewDocument();
                break;
        }
    }

    async Task SaveFile()
    {

        SfDocumentEditor editor = container!.DocumentEditor;
        string base64Data = await editor.SaveAsBlobAsync(FormatType.Dotx);
        byte[] data = Convert.FromBase64String(base64Data);
        base64Data = null;

        Stream stream = new MemoryStream(data);
        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            stream.CopyTo(fileStream);
            fileStream.Close();
        }

        stream.Close();
        stream = null;
        DisplayResult(Localized("lt-save-title"), Localized("lt-save-success"), AlertMessageType.Info);
    }



    async Task PreviewDocument()
    {
        if (container?.DocumentEditor is null)
            return;

        PreviewVisible = true;
        PreviewBusy = true;
        PreviewPdfBytes = null;
        previewLoaded = false;
        StateHasChanged();

        try
        {
            string dotxBase64 = await container.DocumentEditor.SaveAsBlobAsync(FormatType.Dotx);
            byte[] templateBytes = Convert.FromBase64String(dotxBase64);

            IReadOnlyList<MergeField> fields = AvailableFields;
            string[] fieldCodes = fields.Select(field => field.Code).ToArray();
            string[] sampleValues = fieldCodes.Select(MergeFieldSampleValues.For).ToArray();

            byte[]? pdf = await documentsSevice!.PreviewDocumentAsync(new MailMergePreviewModel
            {
                TemplateBase64 = Convert.ToBase64String(templateBytes),
                MergeFields = fieldCodes,
                ValuesFields = sampleValues
            });

            if (pdf is null || pdf.Length == 0)
            {
                PreviewVisible = false;
                DisplayResult(L?["lt-preview-title"] ?? "Preview",
                    L?["lt-preview-error"] ?? "Could not generate the preview.", AlertMessageType.Error);
                return;
            }

            PreviewPdfBytes = pdf;
        }
        catch (Exception ex)
        {
            PreviewVisible = false;
            Logger?.LogError(ex, "Letter template preview failed for {Template}.", currentTemplateName);
            DisplayResult(L?["lt-preview-title"] ?? "Preview", ex.Message, AlertMessageType.Error);
        }
        finally
        {
            PreviewBusy = false;
        }
    }

    void ClosePreview()
    {
        PreviewVisible = false;
        PreviewBusy = false;
        PreviewPdfBytes = null;
        previewLoaded = false;
        previewViewer = null;
    }

    async Task InsertMergeField(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName) || container?.DocumentEditor is null)
            return;

        string fieldCode = MergeFieldFormat.Code(fieldName);
        string fieldResult = MergeFieldFormat.Result(fieldName);
        await container.DocumentEditor.Editor.InsertFieldAsync(fieldCode, fieldResult);
    }

    void ToggleFieldPanel() => FieldPanelVisible = !FieldPanelVisible;

    void CloseFieldPanel() => FieldPanelVisible = false;

    void OpenFullScreen() => NavManager?.NavigateTo("/LetterTemplates");

    void BackToMenu() => NavManager?.NavigateTo("/admin/appsettings");


    protected void OnSelectTemplate(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, TemplateFileName> args)
    {
        if (args.ItemData is null)
            return;

        idxFileSelected = args.ItemData.Id;
        if (idxFileSelected < 0 || idxFileSelected >= FileList.Count)
            return;

        ClosePreview();
        filePath = args.ItemData.FilePath ?? FileList[idxFileSelected];
        currentTemplateName = Path.GetFileNameWithoutExtension(filePath);
        currentTemplateDisplayName = args.ItemData.TemplateFile ?? GetTemplateDisplayName(currentTemplateName);
        OnCreated();

        StateHasChanged();
    }
    private string GetTemplateDisplayName(string? templateFileName)
    {
        TemplateDisplayParts parts = TemplateDisplayCatalog.GetDisplayParts(templateFileName);

        string title = string.IsNullOrWhiteSpace(parts.TitleKey)
            ? parts.FallbackTitle
            : Localized(parts.TitleKey);

        if (string.IsNullOrWhiteSpace(title) || title == parts.TitleKey)
            title = parts.FallbackTitle;
        if (string.IsNullOrWhiteSpace(title))
            title = Path.GetFileNameWithoutExtension(templateFileName ?? string.Empty);

        if (!showTemplateLanguage)
            return title;

        string? language = parts.LanguageKey is null ? null : Localized(parts.LanguageKey);
        if (language == parts.LanguageKey)
            language = null;

        return TemplateDisplayCatalog.FormatTitle(title, language, Localized(TemplateDisplayCatalog.LanguageFormatKey));
    }

    private string Localized(string key) => L?[key].Value ?? key;

    protected void DisplayResult(string titleCaption, string errorMessage, AlertMessageType alertMessageType = AlertMessageType.Error)
    {
        WarningMessage = errorMessage;
        AlertTitle = titleCaption;

        AlertMessageType = alertMessageType;
        OpcoesRegisto = OpcoesRegisto.Info;
        AlertVisibility = true;
        StateHasChanged();
    }

    private async Task<ApplicationSettingsVM> GetSettings()
    {
        return await appSettingsService!.GetSettingsAsync();
    }

    protected class TemplateFileName
    {
        public int Id { get; set; }
        public string? TemplateFile { get; set; }
        public string? FilePath { get; set; }
    }
}