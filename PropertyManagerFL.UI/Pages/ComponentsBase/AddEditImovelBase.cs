using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;
using Syncfusion.Blazor.Inputs;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using Syncfusion.Blazor.Calendars;
using PropertyManagerFL.Application.Formatting;

namespace PropertyManagerFL.UI.Pages.ComponentsBase;

public class AddEditImovelBase : ComponentBase
{
    [Inject] public ILookupTableService? LookupTablesService { get; set; }
    [Inject] public IImovelService? imovelService { get; set; }
    [Inject] public IWebHostEnvironment? HostingEnvironment { get; set; }
    [Inject] public IStringLocalizer<App>? L { get; set; }

    [Parameter] public ImovelVM? Property { get; set; }
    [Parameter] public OpcoesRegisto EditMode { get; set; }
    [Parameter] public string? HeaderCaption { get; set; }

    protected SfUploader? sfUploader;
    protected string uploadedFile { get; set; } = "";

    protected string? InspecaoCaption;
    protected DateTime DataProxInspecaoGas;
    protected bool InibeSelecaoUltimaInspecao = false;

    protected List<string> AllowedFileTypes = new List<string>() { ".jpg", ".jpeg", ".png", ".gif" };
    protected int MaxFileSize = 10 * 1024 * 1024; // 10 MB
    protected string? propertyImage { get; set; }

    protected int idxConservationState;
    protected int idxCertificate;

    public IEnumerable<LookupTableVM>? ConservationStates { get; set; }

    protected SfTextBox? txtPorta { get; set; }

    protected float latitude;
    protected float longitude;


    protected Dictionary<string, object> NotesAttribute = new Dictionary<string, object>()
    {
            {"rows", "4" }
    };

    protected override async Task OnParametersSetAsync()
    {
        ConservationStates = (await LookupTablesService!.GetLookupTableData("EstadoConservacao")).ToList();
        idxConservationState = Property!.Conservacao;

        if (EditMode == OpcoesRegisto.Gravar)
        {
            var _pstEx = Property.CodPstEx;
            var _pst = Property!.CodPst;
            var result = await imovelService!.GetFreguesiaConcelho(_pst, _pstEx);
            if (result.centro is not null)
            {
                latitude = result.centro![0];
                longitude = result.centro![1];
            }
        }

        CalcDataProxInspecao();
    }

    protected void onChangeConservationState(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, LookupTableVM> args)
    {
        idxConservationState = args.Value;
        Property!.Conservacao = idxConservationState;
    }

    protected void UltDataInspecaoChanged(ChangedEventArgs<DateTime> args)
    {
        //Property!.DataUltimaInspecaoGas = args.Value;
        //CalcDataProxInspecao();

    }

    private void CalcDataProxInspecao()
    {
        var DPIG_Dia = Property!.DataUltimaInspecaoGas.Day;
        var DPIG_Mes = Property.DataUltimaInspecaoGas.Month;
        var AnoC = Convert.ToInt32(Property!.AnoConstrucao);

        int AnoPrimeiraInspecaoRequerida = AnoC + 20;
        int AnoCorrente = DateTime.Now.Year;
        if (AnoPrimeiraInspecaoRequerida >= AnoCorrente)
        {
            DataProxInspecaoGas = new DateTime(AnoPrimeiraInspecaoRequerida, DPIG_Mes, DPIG_Dia);
        }
        else
        {
            var AnoProximaInspGas = AnoC + 5;
            while (AnoProximaInspGas < AnoCorrente)
            {
                AnoProximaInspGas += 5;
            }

            DataProxInspecaoGas = new DateTime(AnoProximaInspGas, DPIG_Mes, DPIG_Dia);
        }

        InspecaoCaption = DateDifference(DataProxInspecaoGas.Date, DateTime.Now.ToLocalTime().Date).Trim();
        StateHasChanged();
    }

    protected async Task OnPstChange(Syncfusion.Blazor.Inputs.ChangedEventArgs args)
    {
        var _pstEx = args.Value;
        var _pst = Property!.CodPst;
        var result = await imovelService!.GetFreguesiaConcelho(_pst, _pstEx);
        if (result.centro is not null)
        {
            Property.FreguesiaImovel = result.Localidade;
            Property.ConcelhoImovel = result.Concelho;

            latitude = result.centro![0];
            longitude = result.centro![1];

            if (!string.IsNullOrEmpty(result!.ruas![0]))
            {
                Property.Morada = result!.ruas![0];
                await txtPorta!.FocusAsync();
            }
        }
    }

    protected void OnYearChange(Syncfusion.Blazor.Inputs.ChangedEventArgs args)
    {
        var ConstrYear = int.Parse(args.Value);
        if (DataFormat.IsInteger(ConstrYear))
        {
            var dUltInsp = new DateTime(ConstrYear, 1, 1);
            if (DataFormat.IsValidDate(dUltInsp))
            {
                Property!.DataUltimaInspecaoGas = new DateTime(ConstrYear, 1, 1);
                CalcDataProxInspecao();
                StateHasChanged();
            }
        }
    }


    protected void OnChangeUpload(Syncfusion.Blazor.Inputs.UploadChangeEventArgs args)
    {
        string sExtensao = "";
        if (args.Files is null) return;

        foreach (var file in args.Files)
        {
            uploadedFile = Path.Combine(HostingEnvironment!.WebRootPath, "uploads", "properties", file.FileInfo.Name);
            if (File.Exists(uploadedFile))
            {
                Property!.Foto = Path.GetFileName(uploadedFile);
                StateHasChanged();
            }
            else
            {
                if (uploadedFile != Property!.Foto)
                {
                    sExtensao = Path.GetExtension(uploadedFile);

                    FileStream filestream = new FileStream(uploadedFile, FileMode.Create, FileAccess.Write);
                    file.Stream.WriteTo(filestream);
                    filestream.Close();
                    file.Stream.Close();

                    propertyImage = uploadedFile;
                    Property.Foto = uploadedFile;
                    StateHasChanged();
                }
            }
        }
    }

    protected void OnActionCompleteHandler(Syncfusion.Blazor.Inputs.ActionCompleteEventArgs args)
    {
        var fileName = args.FileData[0].Name;
        uploadedFile = Path.Combine(HostingEnvironment!.WebRootPath, "uploads", "properties", fileName);
        if (File.Exists(uploadedFile))
        {
            Property!.Foto = Path.GetFileName(uploadedFile);

            StateHasChanged();
        }
        else
        {
            if (uploadedFile != Property!.Foto)
            {
                propertyImage = uploadedFile;
                Property!.Foto = Path.GetFileName(uploadedFile);
                StateHasChanged();
            }
        }
    }

    private string DateDifference(DateTime d1, DateTime d2)
    {
        int[] monthDays = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        DateTime fromDate = (d1 > d2) ? d2 : d1;
        DateTime toDate = (d1 > d2) ? d1 : d2;

        int increment = (fromDate.Day > toDate.Day) ? monthDays[fromDate.Month - 1] : 0;

        if (increment == -1)
        {
            increment = DateTime.IsLeapYear(fromDate.Year) ? 29 : 28;
        }

       // int day = (increment != 0) ? (toDate.Day + increment) - fromDate.Day : toDate.Day - fromDate.Day;

        increment = (fromDate.Month + increment) > toDate.Month ? 1 : 0;
        int month = (increment != 0) ? (toDate.Month + 12) - (fromDate.Month + increment) : toDate.Month - (fromDate.Month + increment);

        int year = toDate.Year - (fromDate.Year + increment);

        string yearToShow = (year > 0) ? year.ToString() : "";
        string monthToShow = (month > 0) ? month.ToString() : "";
        string yearLabel = (year == 1) ? L["TituloAno"] : (year > 1) ? L["TituloAnos"] : "";
        string monthLabel = (month == 1) ? L["TituloMes"] : (month > 1) ? L["TituloMeses"] : "";
        string _em = (!string.IsNullOrEmpty(monthLabel) && year > 0) ? L["MSG_E"] : "";

        return $"{yearToShow} {yearLabel} {_em} {monthToShow} {monthLabel}";
    }

}
