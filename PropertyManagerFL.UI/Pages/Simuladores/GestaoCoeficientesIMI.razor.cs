using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Core.Entities;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using PropertyManagerFL.Application.ViewModels;
using Syncfusion.Blazor.Notifications;

namespace PropertyManagerFL.UI.Pages.Simuladores;
public partial class GestaoCoeficientesIMI
{

    [Inject] public IDistritosConcelhosService? DistritosConcelhosService { get; set; }
    [Inject] public IStringLocalizer<App> L { get; set; }

    protected IEnumerable<LookupTableVM>? Distritos { get; set; }

    protected Concelho Concelho = new();

    protected IEnumerable<DistritoConcelho>? Concelhos { get; set; }
    protected bool distritoSelected;
    protected int idxDistrito;
    protected int idxConcelho;
    protected int idxTipoImovel;
    protected double coeficiente;
    protected string coeficienteCaption = string.Empty;
    protected int ConcelhoId;

    protected string? ToastTitle;
    protected string? ToastMessage;
    protected string? ToastCss;
    protected string? ToastIcon;

    protected SfToast? ToastObj { get; set; }


    protected SfGrid<DistritoConcelho>? coefsGridObj { get; set; }


    protected List<PdfHeaderFooterContent> HeaderContent = new List<PdfHeaderFooterContent>
{
        new PdfHeaderFooterContent() { Type = ContentType.Text, Value = "Coeficientes IMI", Position = new PdfPosition() { X = 0, Y = 50 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 13 } }
    };
    protected override async Task OnInitializedAsync()
    {
        ToastTitle = "";
        ToastMessage = "";
        ToastCss = "";
        ToastIcon = "";

        coeficienteCaption = string.Empty;
        idxTipoImovel = 1;
        coeficiente = 0;
        distritoSelected = false;
        Distritos = (await DistritosConcelhosService!.GetDistritos()).ToList();
        Concelhos = (await DistritosConcelhosService!.GetConcelhos()).ToList();
    }

    protected async void OnChangeDistrito(ChangeEventArgs<int, LookupTableVM> args)
    {
        idxDistrito = args.Value;
        distritoSelected = true;

        // ao escolher distrito, devolve concelhos pertencentes a este; permite visualização da respetiva combo (distritoSelected = true)
        Concelhos = (await DistritosConcelhosService!.GetConcelhosByDistrito(idxDistrito)).ToList();
        StateHasChanged();
    }

    protected async Task CoefficientActionBeginHandler(ActionEventArgs<DistritoConcelho> Args)
    {
        ToastTitle = "Coeficientes IMI"; // L["TituloMenuCoeficientesRendas"];

        if (Args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
        {
            var concelho = Args.Data;

            if (Args.Action.ToLower() == "edit")
            {
                var updateOk = await DistritosConcelhosService!.UpdateCoeficienteIMI(concelho.CodConcelho, concelho.Coeficiente);
                if (!updateOk)
                {
                    ToastCss = "e-toast-danger";
                    ToastMessage = L["FalhaGravacaoRegisto"];
                    ToastIcon = "fas fa-exclamation";
                    await ShowToastMessage();
                }
            }
        }
    }

    public async Task ActionComplete(ActionEventArgs<DistritoConcelho> args)
    {
        if (args.RequestType == Syncfusion.Blazor.Grids.Action.Save)
        {
            ToastTitle = L["editionMsg"] + " " + L["Record"];
            ToastCss = "e-toast-success";
            ToastMessage = L["TituloOperacaoOk"];
            ToastIcon = "fas fa-check";
            Concelhos = (await DistritosConcelhosService!.GetConcelhos()).ToList();

            await ShowToastMessage();
        }


    }

    protected async Task OnCommandClicked(CommandClickEventArgs<DistritoConcelho> args)
    {
        ConcelhoId = args.RowData.Id;
    }

    protected async Task ToolbarClickHandler_Coef(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
        if (args.Item.Id == "CoeficientesConcelhos_Grid_pdfexport")
        {
            PdfExportProperties pdfExportProperties = new PdfExportProperties();
            pdfExportProperties.ExportType = ExportType.AllPages;
            pdfExportProperties.FileName = "Coeficientes_IMI.pdf";
            PdfHeader Header = new PdfHeader()
            {
                FromTop = 0,
                Height = 130,
                Contents = HeaderContent
            };
            pdfExportProperties.Header = Header;

            if (coefsGridObj?.DataSource is not null)
            {
                await coefsGridObj!.ExportToPdfAsync(pdfExportProperties);
                ToastTitle = $"Exportação para Pdf ({pdfExportProperties.FileName})";
                ToastCss = "e-toast-success";
                ToastMessage = L["TituloOperacaoOk"];
                ToastIcon = "fas fa-check";

                await ShowToastMessage();
            }
        }
        else if (args.Item.Id == "CoeficientesConcelhos_Grid_excelexport")
        {
            ExcelExportProperties ExportProperties = new ExcelExportProperties();
            ExportProperties.ExportType = ExportType.AllPages;
            ExportProperties.FileName = "Coeficientes_IMI.xlsx";

            ExcelHeader header = new ExcelHeader();
            header.HeaderRows = 2;
            List<ExcelCell> cell = new List<ExcelCell>
                {
                    new ExcelCell() {  Value= "Coeficientes IMI", Style = new ExcelStyle() { Bold = true, FontSize = 13, Italic= true }  }
                };

            List<ExcelRow> HeaderContent = new List<ExcelRow>
                {
                    new ExcelRow() {  Cells = cell, Index = 1 }
                };

            header.Rows = HeaderContent;
            ExportProperties.Header = header;
            if (coefsGridObj?.DataSource is not null)
            {
                await coefsGridObj!.ExportToExcelAsync(ExportProperties);
                ToastTitle = $"Exportação para Excel ({ExportProperties.FileName})";
                ToastCss = "e-toast-success";
                ToastMessage = L["TituloOperacaoOk"];
                ToastIcon = "fas fa-check";
                await InvokeAsync(StateHasChanged);

                await ShowToastMessage();
            }
        }

    }

    protected async Task ShowToastMessage()
    {
        await Task.Delay(100);
        await ToastObj!.ShowAsync();
    }
    protected async Task HideToast()
    {
        await ToastObj!.HideAsync();
    }

}