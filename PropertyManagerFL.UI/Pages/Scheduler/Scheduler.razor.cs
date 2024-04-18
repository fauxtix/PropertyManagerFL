using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Appointments;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Schedule;
using Syncfusion.Blazor.Spinner;

namespace PropertyManagerFL.UI.Pages.Scheduler;
public partial class Scheduler
{
    [Inject] public IAppointmentsService? ApptsService { get; set; }
    [Inject] protected IStringLocalizer<App>? L { get; set; }
    [Inject] protected ILogger<App>? logger { get; set; }
    [Inject] protected IConfiguration? _env { get; set; }
    [Inject] protected IValidationService? validatorService { get; set; }

    protected string? _appointmentsUri;
    protected string? _appointmentsUri_Insert;
    protected string? _appointmentsUri_Update;
    protected string? _appointmentsUri_Delete;

    private SfSchedule<AppointmentVM>? ScheduleRef;
    private SfToast? ToastObj;
    protected SfSpinner? SpinnerObj { get; set; }

    private IEnumerable<AppointmentVM>? DataSource { get; set; }
    private List<AppointmentVM>? gridDataSource { get; set; }

    private DateTime CurrentDate = DateTime.Now;
    private bool ShowSchedule { get; set; } = true;
    private string SearchValue { get; set; } = string.Empty;
    private string SubjectValue { get; set; } = string.Empty;
    private string LocationValue { get; set; } = string.Empty;
    private DateTime? DateStart { get; set; } = DateTime.Now;
    private DateTime? DateEnd { get; set; } = DateTime.Now;

    protected List<string> ValidationsMessages = new();
    protected bool WarningVisibility { get; set; }


    private DateTime ApptStartAppt { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 09, 00, 00);
    List<AppointmentVM> TestDataSource = new()
{
        new AppointmentVM
        {
            Id = 1,
            ApptType=3,
            Subject = "Issue rent receipts",
            Location = "Finanças",
            StartTime = new DateTime(2024, 4, 9, 12, 0, 0),
            EndTime = new DateTime(2024, 4, 9, 12, 30, 0),
            RecurrenceRule = "FREQ=MONTHLY;INTERVAL=1;BYMONTHDAY=9" // Set the recurrence pattern here
        }
 };

    private List<ResourceData>? ApptResources { get; set; } = new();

    private string? ToastTitle;
    private string? ToastContent;
    private string? ToastCssClass;

    private string? ToastMessage;
    private string? ToastCss;
    private string? ToastIcon;

    private string? NewCaption;
    private string? EditCaption;
    private string? DeleteCaption;


    public View CurrentView { get; set; } = View.Week;


    public class ResourceData
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }

    protected override async Task OnInitializedAsync()
    {
        Initialize();
        ApptResources = CreateSchedulerResources();
        DataSource = await GetAppointments();
    }

    private void Initialize()
    {
        ToastTitle = string.Empty;
        ToastContent = string.Empty;
        ToastCssClass = string.Empty;
        ToastMessage = string.Empty;
        ToastCss = string.Empty;
        ToastIcon = string.Empty;
        NewCaption = string.Empty;
        EditCaption = string.Empty;
        DeleteCaption = string.Empty;
        ShowSchedule = true;
        SearchValue = string.Empty;
        SearchValue = string.Empty;
        SubjectValue = string.Empty;
        LocationValue = string.Empty;
        DateStart = DateTime.Now;
        DateEnd = DateTime.Now;

        _appointmentsUri = $"{_env!["BaseUrl"]}/Appointments";
        _appointmentsUri_Insert = $"{_appointmentsUri}/Insert";
        _appointmentsUri_Update = $"{_appointmentsUri}/Update";
        _appointmentsUri_Delete = $"{_appointmentsUri}/Delete";

    }

    private List<ResourceData> CreateSchedulerResources()
    {
        return new List<ResourceData>
    {
        new ResourceData{ Text = "IRS", Id= 1, Color = "#df5286", Image ="icons/people.svg" },
        new ResourceData{ Text = L["TituloIMI"], Id= 2, Color = "#7fa900", Image ="images/payment.svg"  }, // events related to council tax
        new ResourceData{ Text = L["TituloRecibos"], Id= 3, Color = "#ea7a57", Image ="images/calendar.png"  }, // events related to rental receipts
        new ResourceData{ Text = L["TituloFinancas"], Id= 4, Color = "#4682B4", Image ="images/balance.png"  }, // events related to the tax office
        new ResourceData{ Text = L["TituloCartas"], Id= 5, Color = "#a1d179", Image ="images/people.png"  }, // Letters that should be sent to tenants
        new ResourceData{ Text = L["TituloContratos"], Id= 6, Color = "#455275", Image ="images/home-owner.png"  }, // events relating to lease contracts
        new ResourceData{ Text = L["TituloCondominios"], Id= 7, Color = "#96ADD5", Image ="images/payment.png"  },// events related to condominium
        new ResourceData{ Text = L["TituloPessoais"], Id= 8, Color = "#7f89a3", Image ="images/man_person_icon.png"  } // Other events (personal)
    };
    }

    protected void OnActionBegin(ActionEventArgs<AppointmentVM> args)
    {
        WarningVisibility = false;
        ValidationsMessages = new();
        if (args.ActionType == ActionType.EventCreate || args.ActionType == ActionType.EventChange || args.ActionType == ActionType.EventRemove)
        {
            if (args.AddedRecords != null && args.AddedRecords.Count > 0)
            {
                ValidationsMessages = validatorService!.ValidateAppointmentEntries(args.AddedRecords[0]!);
                if (ValidationsMessages is not null)
                {
                    WarningVisibility = true;
                }
            }
        }
    }

    protected async void OnActionCompleted(ActionEventArgs<AppointmentVM> args)
    {
        if (WarningVisibility) return;

        ToastIcon = "fas fa-check";

        AppointmentVM apptData = new();

        if (args.ActionType == ActionType.EventCreate || args.ActionType == ActionType.EventChange || args.ActionType == ActionType.EventRemove)
        {
            if (args.ActionType == ActionType.EventCreate)
            {
                apptData = args.AddedRecords[0];
                var apptType = apptData.ApptType ?? 3;
                apptData.CategoryColor = ApptResources![apptType - 1].Color;

                var transactionId = await ApptsService!.InsertAsync(apptData);
                if (transactionId == -1) // code returned if  error on insertion
                {
                    ToastTitle = $"{L["NewMsg"]} {L["TituloTarefa"]}";
                    ToastCss = "e-schedule-reminder e-toast-danger";
                    ToastMessage = L["TituloOperacaoComErro"];
                    ToastIcon = "fas fa-exclamation-triangle";

                    StateHasChanged();
                    if (ToastObj is not null)
                    {
                        await Task.Delay(200);
                        await ToastObj.ShowAsync();
                    }
                    return;
                }
            }

            ToastTitle = $"{L["NewMsg"]} {L["TituloTarefa"]}";
            ToastCss = "e-schedule-reminder e-toast-success";
            ToastMessage = L["SuccessInsert"];
            ToastIcon = "fas fa-check";
        }
        if (args.ActionType == ActionType.EventChange || args.ActionType == ActionType.EventRemove)
        {


            if (args.ActionType == ActionType.EventChange)
            {
                apptData = args.ChangedRecords[0];
                var apptType = apptData.ApptType ?? 3;

                apptData.CategoryColor = ApptResources![apptType - 1].Color;
                await ApptsService!.UpdateAsync(apptData.Id, apptData);
            }
            else if (args.ActionType == ActionType.EventRemove)
            {
                apptData = args.DeletedRecords[0];

                await ApptsService!.DeleteAsync(apptData.Id);
            }

            ToastTitle = $"{L["EditMsg"]} {L["TituloTarefa"]}";
            ToastCss = "e-schedule-reminder e-toast-success";
            ToastMessage = L["RegistoGravadoSucesso"];
        }

        if (args.ActionType == ActionType.EventRemove)
        {
            apptData = args.DeletedRecords[0];

            if (args.DeletedRecords is not null && args.DeletedRecords.Count > 0)
            {
                await ApptsService!.DeleteAsync(apptData.Id);
            }

            ToastTitle = $"{L["DeleteMsg"]} {L["TituloTarefa"]}";
            ToastCssClass = "e-schedule-reminder e-toast-success";
            ToastContent = L["RegistoAnuladoSucesso"];
            ToastIcon = "fas fa-check";
        }

        DataSource = await GetAppointments();

        StateHasChanged();
        if (ToastObj is not null && args.ActionType != ActionType.DateNavigate)
        {
            await Task.Delay(200);
            await ToastObj.ShowAsync();
        }
    }


    public void OnActionFailure(ActionEventArgs<AppointmentVM> args)
    {
        var actionType = args.ActionType; // for debugging
        var errors = args.Error;
        logger?.LogError(errors.Message, errors);
    }

    protected async Task<IEnumerable<AppointmentVM>> GetAppointments()
    {
        try
        {
            return (await ApptsService!.GetAllAsync()).ToList() ?? Enumerable.Empty<AppointmentVM>();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex.Message);
            return Enumerable.Empty<AppointmentVM>();
        }
    }

    public void OnEventRendered(EventRenderedArgs<AppointmentVM> args)
    {
        Dictionary<string, object> attributes = new Dictionary<string, object>();
        if (CurrentView == View.Agenda)
        {
            attributes.Add("style", "border-left-color: " + args.Data.CategoryColor);
        }
        else
        {
            attributes.Add("style", "background: " + args.Data.CategoryColor);
        }
        args.Attributes = attributes;
    }

    protected async Task HideToast()
    {
        await ToastObj!.HideAsync();
    }


#pragma warning disable BL0005, CA2000  // Component parameter should not be set outside of its component, Dispose objects before losing scope
    public async Task OnEventSearch()
    {
        if (!string.IsNullOrEmpty(SearchValue) && ScheduleRef != null)
        {
            Query query = new Query().Search(SearchValue, new List<string> { "Subject", "Location", "Description" }, null, true, true);
            List<AppointmentVM> eventCollections = await ScheduleRef.GetEventsAsync(null, null, true);
            object data = await new DataManager() { Json = eventCollections }.ExecuteQuery<AppointmentVM>(query);
            List<AppointmentVM>? resultData = data as List<AppointmentVM>;
            if (resultData?.Count > 0)
            {
                ShowSchedule = false;
                gridDataSource = resultData;
            }
            else
            {
                ShowSchedule = true;
            }
        }
        else
        {
            ShowSchedule = true;
        }
    }

    protected async Task OnSearchClick() // source: syncfusion demos
    {
        DateTime? startDate = null;
        DateTime? endDate = null;
        List<WhereFilter> searchObj = new List<WhereFilter>();
        if (!string.IsNullOrEmpty(SubjectValue))
        {
            searchObj.Add(new WhereFilter() { Field = "Subject", Operator = "contains", value = SubjectValue, Condition = "or", IgnoreCase = true });
        }
        if (!string.IsNullOrEmpty(LocationValue))
        {
            searchObj.Add(new WhereFilter() { Field = "Location", Operator = "contains", value = LocationValue, Condition = "or", IgnoreCase = true });
        }
        if (DateStart != null)
        {
            startDate = Convert.ToDateTime(DateStart);
            searchObj.Add(new WhereFilter() { Field = "StartTime", Operator = "greaterthanorequal", value = startDate, Condition = "and", IgnoreCase = false });
        }
        if (DateEnd != null)
        {
            endDate = Convert.ToDateTime(DateEnd).AddDays(1);
            searchObj.Add(new WhereFilter() { Field = "EndTime", Operator = "lessthanorequal", value = endDate, Condition = "and", IgnoreCase = false });
        }
        if (searchObj.Count > 0)
        {
            Query query = new Query().Where(new WhereFilter() { Condition = "and", IsComplex = true, predicates = searchObj });
            List<AppointmentVM>? eventCollections = await ScheduleRef.GetEventsAsync(startDate, endDate, true);
            object data = await new DataManager() { Json = eventCollections }.ExecuteQuery<AppointmentVM>(query);
            List<AppointmentVM>? resultData = data as List<AppointmentVM>;
            gridDataSource = resultData;
            ShowSchedule = false;
        }
        else
        {
            ShowSchedule = true;
        }
    }
#pragma warning restore BL0005, CA2000 // Component parameter should not be set outside of its component, Dispose objects before losing scope
    protected void OnClearClick()
    {
        ShowSchedule = true;
        SearchValue = SubjectValue = LocationValue = string.Empty;
        DateStart = DateEnd = null;
    }

    //void IDisposable.Dispose()
    //{
    //    ScheduleRef?.Dispose();
    //    ToastObj?.Dispose();
    //    SpinnerObj?.Dispose();

    //}

}