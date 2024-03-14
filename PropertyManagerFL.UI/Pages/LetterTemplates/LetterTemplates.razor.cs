using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using Syncfusion.Blazor.DocumentEditor;
using Syncfusion.Blazor.Inputs;
using System.Text.Json;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.LetterTemplates;
public partial class LetterTemplates
{
    [Inject] ILetterTemplatesService? documentsSevice { get; set; }
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

    bool isDialogVisible = false;


    string fieldName = "";
    AlertMessageType AlertMessageType = AlertMessageType.Warning;
    OpcoesRegisto OpcoesRegisto = OpcoesRegisto.Warning;

    protected int idxFileSelected;

    protected List<Object> Items = new List<Object> { "New", new CustomToolbarItemModel(){ Id="save", Text="Save", PrefixIcon="saveIcon"},  "Undo",
        new CustomToolbarItemModel(){ Id="insertfield", Text="Insert Field", PrefixIcon="fa fa-user fa-lg"},
        "Redo", "Comments", "Image", "Table", "Hyperlink", "Bookmark", "Header",
        "Footer", "PageSetup", "PageNumber", "Find", "LocalClipboard", "RestrictEditing"
    };

#pragma warning disable BL0005, CA2000  // Component parameter should not be set outside of its component, Dispose objects before losing scope

    protected override async Task OnInitializedAsync()
    {
        formatType = ImportFormatType.Docx;
        idxFileSelected = 0;
        AlertVisibility = false;
        AlertTitle = "";
        WarningMessage = "";
        int counter = 0;
        FileList = (await documentsSevice!.GetTemplatesFilenamesFromServer()).ToList();
        foreach (var item in FileList)
        {
            var filename = Path.GetFileNameWithoutExtension(item);
            FileNamesOnlyList?.Add(new TemplateFileName { Id = counter++, TemplateFile = filename });
        }

        idxFileSelected = 0;
        filePath = FileList[0];

        StateHasChanged();
    }


    public async void OnSuccess(UploadingEventArgs action)
    {
        string? base64 = action.FileData.RawFile.ToString();
        fileName = action.FileData.Name;
        filePath = await documentsSevice!.GetTemplateFromServer(fileName);
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
            case "insertfield":
                try
                {
                    ShowInsertFieldDialog();
                }
                catch (Exception ex)
                {
                    DisplayResult("Insert merge field", $"{ex.Message}");
                }
                break;
        }
    }

    async Task SaveFile()
    {
        SfDocumentEditor editor = container!.DocumentEditor;
        string base64Data = await editor.SaveAsBlobAsync(FormatType.Docx);
        byte[] data = Convert.FromBase64String(base64Data);
        using (MemoryStream? stream = new(data))
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }

        DisplayResult("Save file", "File successfully saved", AlertMessageType.Info);
    }



    void ShowInsertFieldDialog()
    {
        isDialogVisible = true;
    }

    async void AddField()
    {
        if (!string.IsNullOrEmpty(fieldName))
        {
            string fieldCode = $"MERGEFIELD  {fieldName}  \\* MERGEFORMAT ";
            string fieldResult = $"'«{fieldName}»";
            await container.DocumentEditor.Editor.InsertFieldAsync(fieldCode, fieldResult);
        }

        CancelDialog();
    }

    void CancelDialog()
    {
        isDialogVisible = false;
    }


    protected void OnSelectTemplate(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, TemplateFileName> args)
    {
        idxFileSelected = args.ItemData.Id;
        filePath = FileList![idxFileSelected];
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

    protected class TemplateFileName
    {
        public int Id { get; set; }
        public string? TemplateFile { get; set; }
    }
}