using Microsoft.AspNetCore.Components;
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
        [Inject] public ILogService? logService { get; set; }
        [Inject] public ILogger<ViewAppLogsBase>? logger { get; set; }
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



        protected async override Task OnInitializedAsync()
        {
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
            SelectedAppLog = await logService.GetLog_ById(LogId);
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
            if ( result is not null && result.Any())
            {
                alertTitle = "Log Viewer";
                DeleteLogEntryCaption = $"Marcou {FilteredRecords.Count()} registos para apagar. Não tem reversão! Backup recomendado antes de continuar";
                DeleteLogsDialogVisibility = true;
                StateHasChanged();
            }
            else
            {
                AlertVisibility = true;
                WarningMessage = "Não há registos para apagar... Verifique, p.f.";
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
                await Task.Delay(200);

                logger?.LogError(ex.Message, ex);
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
                    LogGrid.FilterSettings.Columns = new List<GridFilterColumn>();
                }

                if (LogGrid.FilterSettings.Columns.Count > 0)
                {
                    LogGrid.FilterSettings.Columns.RemoveAll(c => c.Field == "TimeStamp");
                }

                //Get all the Grid columns
                var columns = await LogGrid.GetColumns();
                //Fetch the Uid of TimeStamp column
                string fUid = columns[3].Uid;

                LogGrid.FilterSettings.Columns.Add(new GridFilterColumn
                {
                    Field = "TimeStamp",
                    Operator = Syncfusion.Blazor.Operator.GreaterThanOrEqual,
                    Predicate = "and",
                    Value = StartDate,
                    Uid = fUid
                });

                LogGrid.FilterSettings.Columns.Add(new GridFilterColumn
                {
                    Field = "TimeStamp",
                    Operator = Syncfusion.Blazor.Operator.LessThanOrEqual,
                    Predicate = "and",
                    Value = EndDate.AddDays(1).AddSeconds(-1),
                    Uid = fUid
                });
                LogGrid.Refresh();
            }

        }

        public void Dispose()
        {
            LogGrid?.Dispose();
            SpinnerObj?.Dispose();
        }
    }
}
