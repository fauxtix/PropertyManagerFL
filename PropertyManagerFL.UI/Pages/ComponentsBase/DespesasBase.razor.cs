using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using ObjectsComparer;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Despesas;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System.Globalization;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class DespesasBase : ComponentBase, IDisposable
    {
        [CascadingParameter]
        protected Task<AuthenticationState>? authenticationStateTask { get; set; }
        [Inject] protected IStringLocalizer<App>? L { get; set; }
        [Inject] protected IDespesaService? expensesService { get; set; }
        [Inject] protected IConfiguration? config { get; set; }
        [Inject] protected HttpClient? _httpClient { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }

        protected IEnumerable<DespesaVM>? expensesList { get; set; }

        protected SfToast? ToastObj { get; set; }
        protected SfSpinner? SpinnerObj { get; set; }
        protected SfGrid<DespesaVM>? gridObj { get; set; }


        protected string ToastTitle = "";
        protected string ToastContent = "";
        protected string ToastCssClass = "";

        protected string? ToastMessage;
        protected string? ToastCss;
        protected string? ToastIcon;

        protected string? pageBadgeCaption;


        protected string spinnerLabel = "";
        protected string[] GroupedColumns = new string[] { "DescrImputacao" };
        protected string currentCulture = CultureInfo.CurrentCulture.Name;
        protected bool ErrorVisibility { get; set; } = false;

        protected OpcoesRegisto RecordMode { get; set; }

        protected string WarningCaption = "";
        protected string WarningTitle = "";

        protected bool EditExpenseDialogVisibility { get; set; } = false;
        protected bool DeleteConfirmVisibility { get; set; } = false;
        protected List<string> Error_Warnings_Msgs = new();

        protected bool editRecord { get; set; } = true;
        protected DespesaVM SelectedExpense = new();
        protected DespesaVM OriginalSelectedExpense = new();

        protected bool IsDirty = false;
        protected List<string> ValidationsMessages = new();

        protected bool WarningVisibility { get; set; }
        protected string? WarningMessage { get; set; }

        protected int expenseId;
        protected bool AlertVisibility { get; set; } = false;
        protected string? alertTitle = "";

        protected Modules modulo { get; set; }

        protected string? NewCaption { get; set; }
        protected string? EditCaption { get; set; }
        protected string? DeleteCaption;

        protected bool ShowToolbarOptions { get; set; } = true;


        protected async override Task OnInitializedAsync()
        {
            ShowToolbarOptions = false;
            pageBadgeCaption = L["TituloDespesasTodasDespesas"];
            if (currentCulture == "pt")
                currentCulture = "pt-PT";

            WarningCaption = "";
            WarningTitle = "";
            await GetExpenses();
        }

        protected async Task ConfirmDeleteYes()
        {
            ToastTitle = $"{L["DeleteMsg"]} {L["TituloDespesa"]}";
            var expenseDeleted = false;

            try
            {
                var deleteOk = await expensesService!.Delete(expenseId);

                if (deleteOk == false)
                {
                    ToastCssClass = "e-toast-danger";
                    ToastContent = $"{L["lblExplanation"]}";
                }
                else
                {
                    expenseDeleted = true;
                    ToastCssClass = "e-toast-success";
                    ToastContent = L["RegistoAnuladoSucesso"];
                        }

                DeleteConfirmVisibility = false;

                if (expenseDeleted)
                    await GetExpenses();

                StateHasChanged();

                await Task.Delay(200);
                await ToastObj!.ShowAsync();

            }
            catch (Exception ex)
            {
                expenseDeleted = false;
                ToastCssClass = "e-toast-danger";
                ToastContent = $"{L["FalhaAnulacaoRegisto"]}. Erro: {ex.Message}";
                await Task.Delay(200);
                await ToastObj!.ShowAsync();
            }
        }

        protected async Task GetExpenses()
        {
            spinnerLabel = L["MSG_PreparandoDados"];
            await Task.Delay(200);
            await SpinnerObj!.ShowAsync();

            expensesList = (await expensesService!.GetAll()).ToList();

            ShowToolbarOptions = expensesList.Any();

            await Task.Delay(200);
            await SpinnerObj.HideAsync();

        }

        protected async Task onAddExpense(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {

            // Antes de inputar despesas, verificar se há registo de imóveis/frações...

            var canContinue = await expensesService.AreThereProperties();
            if (canContinue == false)
            {
                AlertVisibility = true;
                alertTitle = L["TituloImputacaoDespesas"];
                WarningMessage = L["TituloErroImputacaoDespesas"];
                return;
            }

            NewCaption = $"{L["NewMsg"]} {L["TituloDespesa"]}";
            editRecord = false;
            SelectedExpense = new()
            {
                DataMovimento = DateTime.UtcNow,
                Valor_Pago = 0,
                ID_TipoDespesa = 0,
                ID_CategoriaDespesa = 0,
                ID_ModoPagamento = 2, // transferência
                NumeroDocumento = "",
                Notas = ""
            };

            EditExpenseDialogVisibility = true;
        }

        public async Task AddOrSaveExpense()
        {
            IsDirty = false;
            ErrorVisibility = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationsMessages = validatorService!.ValidatePaymentEntries(SelectedExpense!);
            ToastTitle = L["TituloDespesas"];

            if (ValidationsMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    editRecord = true;
                    EditCaption = L["EditMsg"] + " " + L["TituloDespesa"];

                    var updateOk = await expensesService!.Update(expenseId, SelectedExpense);

                    if (updateOk)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["RegistoGravadoSucesso"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["TituloErroProcessarAlteracoes"];
                        ToastIcon = "fas fa-exclamation";
                    }

                }

                else // !editMode (Insert)
                {
                    editRecord = false;
                    NewCaption = L["NewMsg"] + " " + L["TituloDespesa"];


                    // 1 = Ok, -1 = Erro
                    var InsertOk = await expensesService!.Insert(SelectedExpense!);
                    if (InsertOk > 0)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["SuccessInsert"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["FalhaCriacaoRegisto"];
                        ToastIcon = "fas fa-exclamation";
                    }
                }
                await GetExpenses();
                EditExpenseDialogVisibility = false;

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();
            }
            else
            {
                ErrorVisibility = true;
            }
        }

        public async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            await GetExpenses(); // return 'expensesList'

            if (expensesList.Count() == 0)
            {
                ToastTitle = L[""];
                ToastMessage = L["TituloManutencaoDespesas"];
                ToastCss = "e-toast-warning";

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();
                await Task.Delay(1000);
                return;
            }

            if (args.Item.Id == "Expenses_Grid_excelexport")
            {
                ExcelExportProperties excelExportProperties = new ExcelExportProperties();
                excelExportProperties.IncludeTemplateColumn = true;
                excelExportProperties.IncludeHeaderRow = true;
                excelExportProperties.FileName = $"Expenses_{DateTime.Now.ToShortTimeString()}.xlsx";
                await gridObj!.ExportToExcelAsync(excelExportProperties);
            }
            else if (args.Item.Id == "ThisYear")
            {
                DateTime start = new DateTime(DateTime.Now.Year, 1, 1),
                                end = start.AddYears(1);

                var yearQry = from record in expensesList
                              where record.DataMovimento >= start
                               && record.DataMovimento < end
                              select record;
                pageBadgeCaption = L["TituloDespesasEsteAno"];
                expensesList = yearQry;

            }
            else if (args.Item.Id == "LastYear")
            {
                DateTime start = new DateTime(DateTime.Now.Year, 1, 1).AddYears(-1),
                                end = start.AddYears(1);

                var yearQry = from record in expensesList
                              where record.DataMovimento >= start
                               && record.DataMovimento < end
                              select record;
                pageBadgeCaption = L["TituloDespesasAnoAnterior"];
                expensesList = yearQry;

            }
            else if (args.Item.Id == "ThisMonth")
            {
                var monthQry = expensesList
                .Where(p => p.DataMovimento.Year == DateTime.Today.Year &&
                p.DataMovimento.Date.Month == DateTime.Today.Month);
                pageBadgeCaption = L["TituloMesAnterior"]; ;

                expensesList = monthQry;
            }
            else // All expenses
            {
                pageBadgeCaption = L["TituloTotalDespesas"];
            }
        }


        protected async Task OnExpenseDoubleClickHandler(RecordDoubleClickEventArgs<DespesaVM> args)
        {
            expenseId = args.RowData.Id;
            modulo = Modules.Pagamentos;
            SelectedExpense = await expensesService!.GetDespesa_ById(expenseId);
            OriginalSelectedExpense = await expensesService.GetDespesa_ById(expenseId);

            EditExpenseDialogVisibility = true;

            EditCaption = L["EditMsg"] + " " + L["TituloDespesa"];
            RecordMode = OpcoesRegisto.Gravar;
        }

        public async Task OnExpenseCommandClicked(CommandClickEventArgs<DespesaVM> args)
        {
            expenseId = args.RowData.Id;
            modulo = Modules.Pagamentos;
            DeleteCaption = "";

            SelectedExpense = await expensesService!.GetDespesa_ById(expenseId);
            OriginalSelectedExpense = await expensesService.GetDespesa_ById(expenseId);
            DeleteCaption = SelectedExpense.DataMovimento.ToShortDateString(); ;


            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {

                EditExpenseDialogVisibility = true;
                EditCaption = L["EditMsg"] + " " + L["TituloDespesa"];
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                WarningTitle = $"{L["DeleteMsg"]} {L["TituloDespesa"]}";
                DeleteConfirmVisibility = true;
                StateHasChanged();
            }
            if (args.CommandColumn.Type == CommandButtonType.None)
            {
                editRecord = false;
                RecordMode = OpcoesRegisto.Inserir;
                EditExpenseDialogVisibility = true;
                NewCaption = L["TituloNovaDespesaPorDuplicacao"];
                StateHasChanged();
            }
        }

        protected void IgnoreChangesAlert()
        {
            IsDirty = false;
            ErrorVisibility = false;
            EditExpenseDialogVisibility = false;
        }

        protected void ContinueEdit()
        {
            IsDirty = false;
            EditExpenseDialogVisibility = true;

        }

        public void ConfirmDeleteNo()
        {
            DeleteConfirmVisibility = false;
        }

        public void CloseValidationErrorBox()
        {
            DeleteConfirmVisibility = false;
            EditExpenseDialogVisibility = true;
        }

        protected void CloseEditDialog()
        {
            IsDirty = false;
            ErrorVisibility = false;

            var comparer_I = new ObjectsComparer.Comparer<DespesaVM>();
            var currentData_I = SelectedExpense;
            var originalData_I = OriginalSelectedExpense;
            IEnumerable<Difference> differences_I;
            var isEqual_I = comparer_I.Compare(currentData_I!, originalData_I!, out differences_I);
            if (!isEqual_I)
            {
                IsDirty = true;
            }
            else
            {
                EditExpenseDialogVisibility = false;
            }
        }
        protected async Task HideToast()
        {
            await ToastObj!.HideAsync();
        }

        public void Dispose()
        {
            ToastObj?.Dispose();
            SpinnerObj?.Dispose();
            gridObj?.Dispose();
        }
    }
}
