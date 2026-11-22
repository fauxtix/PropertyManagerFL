using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Core.Helpers;
using Syncfusion.Blazor.DocumentEditor;
using Syncfusion.Blazor.Inputs;
using System.Globalization;
using System.Text.Json;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.LetterTemplates;
public partial class LetterTemplates
{
    [Inject] ILetterTemplatesService? documentsSevice { get; set; }
    [Inject] public IAppSettingsService? appSettingsService { get; set; }

    [Inject] public IStringLocalizer<App>? L { get; set; }

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

    [Inject] public NavigationManager? NavManager { get; set; }

    protected bool IsFullScreen { get; private set; }
    protected bool FieldPanelVisible { get; set; }
    protected IReadOnlyList<MergeField> AvailableFields =>
        MergeFieldCatalog.GetFieldsForTemplate(currentTemplateName);


    protected List<Object> Items => new List<Object> { "New", new CustomToolbarItemModel(){ Id="save", Text=L?["Guardar"] ?? "Save", PrefixIcon="saveIcon"},  "Undo",
        "Redo", "Comments", "Image", "Table", "Hyperlink", "Bookmark", "Header",
        "Footer", "PageSetup", "PageNumber", "Find", "LocalClipboard", "RestrictEditing"
    };

#pragma warning disable BL0005, CA2000  // Component parameter should not be set outside of its component, Dispose objects before losing scope

    protected override async Task OnInitializedAsync()
    {
        string relativeUrl = NavManager?.ToBaseRelativePath(NavManager.Uri) ?? string.Empty;
        IsFullScreen = relativeUrl.StartsWith("LetterTemplates", StringComparison.OrdinalIgnoreCase);

        formatType = ImportFormatType.Docx;
        idxFileSelected = 0;
        AlertVisibility = false;
        AlertTitle = "";
        WarningMessage = "";
        int counter = 0;

        appSettings = await GetSettings();
        selectedCulture = appSettings.DefaultLanguage;
        var culture = new CultureInfo(selectedCulture!);

        FileList = (await documentsSevice!.GetTemplatesFilenamesFromServer(selectedCulture!)).ToList();
        foreach (var item in FileList)
        {
            var filename = Path.GetFileNameWithoutExtension(item);
            FileNamesOnlyList?.Add(new TemplateFileName { Id = counter++, TemplateFile = filename });
        }

        idxFileSelected = 0;
        filePath = FileList[0];
        currentTemplateName = Path.GetFileNameWithoutExtension(filePath);

        StateHasChanged();
    }


    public async void OnSuccess(UploadingEventArgs action)
    {
        string? base64 = action.FileData.RawFile.ToString();
        fileName = action.FileData.Name;
        filePath = await documentsSevice!.GetTemplateFromServer(fileName);
        currentTemplateName = Path.GetFileNameWithoutExtension(fileName);
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
            DisplayResult("Letter templates", "File was not loaded from the expected location.", AlertMessageType.Warning);
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
        DisplayResult("Error uploading file", args.Response.StatusText, AlertMessageType.Error);
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
                    DisplayResult("Templates", $"File ({filePath}) not found. Please verify.", AlertMessageType.Error);
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
                DisplayResult($"Error loading file ({filePath}).", $"{ex.Message}");
            }
        }
    }

    public async void onItemClick(ClickEventArgs args)
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
                        DisplayResult("Save document", ex.Message);
                    }
                }
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
        DisplayResult("Save file", "File successfully saved", AlertMessageType.Info);
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
        idxFileSelected = args.ItemData.Id;
        filePath = FileList![idxFileSelected];
        currentTemplateName = Path.GetFileNameWithoutExtension(filePath);
        OnCreated();

        StateHasChanged();
    }
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
    }
}