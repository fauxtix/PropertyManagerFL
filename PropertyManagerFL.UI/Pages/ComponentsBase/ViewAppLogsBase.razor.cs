using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Logs;
using Syncfusion.Blazor.Calendars;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Spinner;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class ViewAppLogsBase : ComponentBase, IDisposable
    {
        [Inject] UserManager<IdentityUser>? UserManager { get; set; }

        [Inject] public ILogService? logService { get; set; }
        [Inject] public ILogger<ViewAppLogsBase>? logger { get; set; }
        [Inject] public IWebHostEnvironment? hostEnvironment { get; set; }
        [Inject] protected IStringLocalizer<App>? L { get; set; }

        [CascadingParameter] protected Task<AuthenticationState>? authenticationStateTask { get; set; }

        protected AppLogDto? SelectedAppLog { get; set; }
        protected List<AppLogDto>? Logs { get; set; }
        protected int LogId { get; set; }
        protected bool ViewLogVisibility { get; set; }
        protected bool SpinnerVisibility { get; set; }
        protected bool DeleteLogsDialogVisibility { get; set; }
        protected bool DeleteLogDialogVisibility { get; set; }
        protected string ViewLogCaption { get; set; } = string.Empty;
        protected string DeleteLogEntryCaption { get; set; } = string.Empty;

        protected SfGrid<AppLogDto>? LogGrid { get; set; }
        protected SfSpinner? SpinnerObj { get; set; }

        protected DateTime StartDate { get; set; }
        protected DateTime EndDate { get; set; }

        protected IEnumerable<AppLogDto>? FilteredRecords { get; set; }

        protected string? alertTitle = "";
        protected bool AlertVisibility { get; set; }
        protected string? WarningMessage { get; set; }

        protected string? pageBadgeCaption;

        protected System.Security.Claims.ClaimsPrincipal? CurrentUser;
        protected IdentityUser? user;
        protected string email = "";

        protected async override Task OnInitializedAsync()
        {
            CurrentUser = (await authenticationStateTask!).User;
            user = await UserManager!.FindByNameAsync(CurrentUser?.Identity?.Name);
            email = user.Email;


            pageBadgeCaption = L["TituloTodosRegistos"];
            await GetAllLogs();
        }

        protected async Task OnAppLogDoubleClickHandler(RecordDoubleClickEventArgs<AppLogDto> args)
        {

            LogId = args.RowData.Id;
            SelectedAppLog = await logService.GetLog_ById(LogId);

            ViewLogVisibility = true;
            ViewLogCaption = "Visualizar Log";
            StateHasChanged();
        }

        protected async Task AppLogRowSelectHandler(RowSelectEventArgs<AppLogDto> args)
        {

            LogId = args.Data.Id;
            SelectedAppLog = await logService!.GetLog_ById(LogId);
            ViewLogVisibility = true;
            ViewLogCaption = "Visualizar Log";
            StateHasChanged();
        }

        public async Task OnAppLogCommandClicked(CommandClickEventArgs<AppLogDto> args)
        {

            LogId = args.RowData.Id;
            SelectedAppLog = await logService.GetLog_ById(LogId);

            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                ViewLogVisibility = true;
                ViewLogCaption = "Ver dados da entrada no Log";
                StateHasChanged();
            }
            else if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteLogEntryCaption = $"{args.RowData.Message}";
                DeleteLogDialogVisibility = true;
                StateHasChanged();
            }
        }


        protected async Task onDeleteLogs()
        {
            var filteredData = await LogGrid!.GetFilteredRecordsAsync();
            FilteredRecords = JsonConvert.DeserializeObject<List<AppLogDto>>(JsonConvert.SerializeObject(filteredData));

            var result = FilteredRecords?.ToList();
            if (result is not null && result.Any())
            {
                alertTitle = "Log Viewer";
                DeleteLogEntryCaption = $"Marcou {FilteredRecords?.Count()} registos para apagar. Não tem reversão! Backup recomendado antes de continuar";
                DeleteLogsDialogVisibility = true;
                logger?.LogWarning($"Utilizador ({user}) apagou {FilteredRecords?.Count()} registos ");
                StateHasChanged();
            }
            else
            {
                AlertVisibility = true;
                pageBadgeCaption = L["TituloTodosRegistos"];
                WarningMessage = "Não há registos para apagar... Verifique, p.f.";
                logger?.LogWarning($"Utilizador ({user}) tentou apagar registos com tabela vazia... ");

                return;
            }

        }

        protected async Task DeleteLogRecord()
        {
            await logService!.DeleteById(LogId);
            DeleteLogDialogVisibility = false;
            await GetAllLogs();
        }


        protected async Task onGetAll()
        {
            await GetAllLogs();
        }
        protected async Task onViewLogins()
        {
            Logs = (await logService!.ViewLogins()).ToList();
        }

        public async Task ConfirmDeleteYes()
        {
            DeleteLogsDialogVisibility = false;
            await Task.Delay(200);
            await SpinnerObj!.ShowAsync();
            try
            {
                if (FilteredRecords is not null)
                {
                    await logService.DeleteFilteredRecords(FilteredRecords);
                }
                else
                {
                    await logService!.DeleteAll();
                }

                StateHasChanged();

                await Task.Delay(200);
                await SpinnerObj!.HideAsync();
                await LogGrid!.SearchAsync("");
                await GetAllLogs();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.Message, ex);
                await SpinnerObj!.HideAsync();
            }
        }

        protected async Task GetAllLogs()
        {
            Logs = (await logService!.GetAppLogs()).ToList();
        }

        protected void onOpenLogDialog(Syncfusion.Blazor.Popups.OpenEventArgs args)
        {
            args.PreventFocus = true;
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            await GetAllLogs();

            if (args.Item.Id.ToLower().Contains("excelexport"))
            {
                var logFile = $"Log{DateTime.Now.ToShortDateString()}.xlsx";
                logFile = logFile.Replace("/", "_");
                var fullExcelFilePath = logFile;
                try
                {
                    ExcelExportProperties excelExportProperties = new ExcelExportProperties
                    {
                        IncludeTemplateColumn = true,
                        IncludeHeaderRow = true,
                        FileName = fullExcelFilePath
                    };
                    await LogGrid!.ExportToExcelAsync(excelExportProperties);
                    logger?.LogInformation($"Exportado ficheiro excel ({excelExportProperties.FileName})");
                    await GetAllLogs();
                }
                catch (Exception ex)
                {
                    logger?.LogInformation(ex.Message, ex);
                }
            }
            else if (args.Item.Id.ToLower() == "warninglogs")
            {
                var warnings = from record in Logs
                               where record.Level.ToLower() == "warning"
                               select record;
                pageBadgeCaption = "Warnings";
                Logs = warnings.ToList();

            }
            else if (args.Item.Id.ToLower() == "errorlogs")
            {
                var errors = from record in Logs
                             where record.Level.ToLower() == "error"
                             select record;
                pageBadgeCaption = "Errors";
                Logs = errors.ToList();
            }
            else if (args.Item.Id.ToLower() == "infologs")
            {
                var infos = from record in Logs
                            where record.Level.ToLower() == "information"
                            select record;
                pageBadgeCaption = "Info";
                Logs = infos.ToList();
            }
            else if (args.Item.Id.ToLower() == "loginlogs")
            {
                pageBadgeCaption = "Logins";

                Logs = (await logService!.ViewLogins()).ToList();
            }
            else if (args.Item.Id.ToLower() == "deletelogs")
            {
                pageBadgeCaption = L["DeleteMsg"];

                await onDeleteLogs();
            }
            else // All Logs
            {
                await GetAllLogs();
                pageBadgeCaption = L["TituloTodosRegistos"];
            }
        }


        protected void LogDateChangeHandler(RangePickerEventArgs<DateTime> args)
        {
            StartDate = args.StartDate;
            EndDate = args.EndDate;
        }

        public async Task OnActionBegin(ActionEventArgs<AppLogDto> args)
        {

            if (args.CurrentFilteringColumn is null) return;

            if (args.RequestType.Equals(Syncfusion.Blazor.Grids.Action.Filtering) && args.CurrentFilteringColumn.Equals("TimeStamp"))
            {
                args.Cancel = true; //cancel default filter action
                if (LogGrid.FilterSettings.Columns == null)
                {
#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
                    LogGrid.FilterSettings.Columns = new List<GridFilterColumn>();
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.
                }

                if (LogGrid.FilterSettings.Columns.Count > 0)
                {
                    LogGrid.FilterSettings.Columns.RemoveAll(c => c.Field == "TimeStamp");
                }

                //Get all the Grid columns
                var columns = await LogGrid.GetColumnsAsync();
                //Fetch the Uid of TimeStamp column
                string fUid = columns[3].Uid;

#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
                LogGrid.FilterSettings.Columns.Add(new GridFilterColumn
                {
                    Field = "TimeStamp",
                    Operator = Syncfusion.Blazor.Operator.GreaterThanOrEqual,
                    Predicate = "and",
                    Value = StartDate,
                    Uid = fUid
                });
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

#pragma warning disable BL0005 // Component parameter should not be set outside of its component.
                LogGrid.FilterSettings.Columns.Add(new GridFilterColumn
                {
                    Field = "TimeStamp",
                    Operator = Syncfusion.Blazor.Operator.LessThanOrEqual,
                    Predicate = "and",
                    Value = EndDate.AddDays(1).AddSeconds(-1),
                    Uid = fUid
                });
#pragma warning restore BL0005 // Component parameter should not be set outside of its component.

                await LogGrid.Refresh();
            }

        }

        public void Dispose()
        {
            LogGrid?.Dispose();
            SpinnerObj?.Dispose();
        }
    }
}
