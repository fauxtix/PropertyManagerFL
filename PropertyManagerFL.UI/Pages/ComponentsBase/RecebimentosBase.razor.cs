using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using PropertyManagerFLApplication.Utilities;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.SplitButtons;
using System.Globalization;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class RecebimentosBase : ComponentBase, IDisposable
    {
        [Inject] protected IStringLocalizer<App>? L { get; set; }
        [Inject] protected IRecebimentoService? transactionsService { get; set; }
        [Inject] protected IFracaoService? unitsService { get; set; }
        [Inject] protected IArrendamentoService? leasesService { get; set; }
        [Inject] protected ILookupTableService? lookupsService { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }


        protected IEnumerable<RecebimentoVM>? Transactions { get; set; }
        protected IEnumerable<RecebimentoVM>? BatchTransactions { get; set; }

        protected string currentCulture = CultureInfo.CurrentCulture.Name;

        protected SfToast? ToastObj { get; set; }
        protected SfSpinner? SpinnerObj { get; set; }
        protected SfGrid<RecebimentoVM>? gridObj { get; set; }


        protected string ToastTitle = "";
        protected string ToastContent = "";
        protected string ToastCssClass = "";

        protected string? ToastMessage;
        protected string? ToastCss;
        protected string? ToastIcon;


        protected string spinnerLabel = "";

        protected bool WarningVisibility { get; set; }
        protected string? WarningMessage { get; set; }

        protected int transactionId;
        protected bool AlertVisibility { get; set; } = false;
        protected string? alertTitle = "";

        protected Modules modulo { get; set; }
        protected OpcoesRegisto RecordMode { get; set; }

        protected string WarningCaption = "";
        protected string WarningTitle = "";

        protected bool EditTransationDialogVisibility { get; set; } = false;
        protected bool DeleteConfirmVisibility { get; set; } = false;
        protected bool SettlePaymentConfirmVisibility { get; set; } = false;
        protected List<string> Error_Warnings_Msgs = new();
        protected bool ErrorVisibility { get; set; } = false;
        protected bool AddEditMonthlyTransactionsVisibility { get; set; } = false;


        protected bool editRecord = true;
        protected RecebimentoVM SelectedTransation = new();
        protected RecebimentoVM OriginalSelectedTransaction = new();

        protected bool IsDirty = false;
        protected List<string> ValidationsMessages = new();
        protected string? NewCaption { get; set; }
        protected string? EditCaption { get; set; }
        protected string? DeleteCaption;

        protected int TransactionsCount { get; set; }
        protected string? pageBadgeCaption;

        protected string[] GroupedColumns = new string[] { "Imovel" };

        protected string tbrSelectedValue = "AllExpenses";
        protected class SelectOption
        {
            public string? Id { get; set; }
            public string? CaptionText { get; set; }
        }

        protected async override Task OnInitializedAsync()
        {
            AddEditMonthlyTransactionsVisibility = false;
            SettlePaymentConfirmVisibility = false;

            pageBadgeCaption = L["TituloPagamentosTodosPagamentos"];

            if (currentCulture == "pt")
                currentCulture = "pt-PT";

            WarningCaption = "";
            WarningTitle = "";


            await GetTransactions();
            TransactionsCount = Transactions?.Count() == 6 ? 12 : Transactions.Count();

        }

        protected async Task GetTransactions()
        {
            spinnerLabel = L["MSG_PreparandoDados"];
            await Task.Delay(200);
            await SpinnerObj!.ShowAsync();

            Transactions = (await transactionsService!.GetAll()).ToList();

            await Task.Delay(200);
            await SpinnerObj.HideAsync();
        }


        // Outros pagamentos - pagamento de rendas em módulo autónomo
        protected async Task onAddTransaction(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            // Antes de criar transação, verificar se há registo de frações/arrendamento disponíveis...

            var leases = await leasesService.GetAll();
            var canContinue = leases.Count() > 0;
            if (canContinue == false)
            {
                AlertVisibility = true;
                alertTitle = "Criar registo de pagamento";
                WarningMessage = "Não há registo de Arrendamentos! Verifique, p.f.";
                return;
            }

            canContinue = await unitsService!.GetFracoes_Disponiveis() is not null;
            if (canContinue == false)
            {
                AlertVisibility = true;
                alertTitle = "Novo pagamento";
                WarningMessage = "Não é possivel continuar. Não há registo de frações!";
                return;
            }

            NewCaption = "Novo pagamento";
            editRecord = false;
            SelectedTransation = new()
            {
                DataMovimento = DateTime.UtcNow,
                Renda = false,
                ID_Inquilino = 0,
                ID_Propriedade = 0,
                ID_TipoRecebimento = 0,
                ValorEmFalta = 0,
                ValorRecebido = 0,
                ValorPrevisto = 0,
                GeradoPeloPrograma = false,
                Notas = ""
            };

            EditTransationDialogVisibility = true;
        }


        protected async Task HandleBatchTransaction()
        {
            ToastTitle = L["TituloProcessamentoRendas"];
            try
            {
                //var month = args.ProcessingMonth;
                //var year = args.ProcessingYear;

                // if this work, then above sentence shoul be passing the parameters
                var transactionResult = await transactionsService!.ProcessMonthlyRentPayments(); // ProcessMonthlyRentPayments(month, year)
                if (transactionResult == 1)
                {
                    ToastCss = "e-toast-success";
                    ToastMessage = L["TituloOperacaoOk"];
                    ToastIcon = "fas fa-check";
                }
                else
                {
                    ToastCss = "e-toast-danger";
                    ToastMessage = "Error while processings monthly rents! Check log, please.";
                    ToastIcon = "fas fa-exclamation";
                }
            }
            catch (Exception ex)
            {
                ToastCss = "e-toast-danger";
                ToastMessage = $"Error while processings monthly rents! ({ex.Message}). Please check log.";
                ToastIcon = "fas fa-exclamation";
            }
            finally
            {
                AddEditMonthlyTransactionsVisibility = false;
                await GetTransactions();

                StateHasChanged();

                await Task.Delay(100);
                await ToastObj!.ShowAsync();
            }
        }

        public async Task HandleCancelBatchTransaction()
        {
            ToastTitle = "Cancelamento de Processamento de rendas";
            try
            {
                // Lê pagamentos (temp) criados
                var pagamentosGerados = await transactionsService!.GetAllTemp();
                if (pagamentosGerados != null)
                {
                    await transactionsService.DeleteRecebimentosTemp();
                }
                ToastCss = "e-toast-success";
                ToastMessage = L["TituloOperacaoOk"];
                ToastIcon = "fas fa-check";
            }
            catch (Exception)
            {
                ToastCss = "e-toast-danger";
                ToastMessage = "Erro ao cancelar! Verifique log, p.f.";
                ToastIcon = "fas fa-exclamation";
            }
            finally
            {
                StateHasChanged();
                AddEditMonthlyTransactionsVisibility = false;
                await GetTransactions();

                await Task.Delay(100);
                await ToastObj!.ShowAsync();
            }
        }


        public async Task AddOrSaveTransaction()
        {
            IsDirty = false;
            ErrorVisibility = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationsMessages = validatorService!.ValidateTransactonsEntries(SelectedTransation!);
            ToastTitle = "Pagamentos";

            if (ValidationsMessages == null)
            {

                // Se fôr pagamento de renda, verifica se data de entrada é anterior à data do último pagamento
                if (SelectedTransation.Renda)
                {
                    var rentsPaid = Transactions.Where(r => r.Renda && r.ID_Inquilino == SelectedTransation.ID_Inquilino).ToList();
                    if (rentsPaid.Count > 0)
                    {
                        var lastPayment = rentsPaid.OrderByDescending(r => r.DataMovimento).FirstOrDefault().DataMovimento;
                        if (SelectedTransation.DataMovimento < lastPayment)
                        {
                            ValidationsMessages = new List<string>
                            {
                                $"Data de pagamento ({SelectedTransation.DataMovimento.ToShortDateString()}) inválida. Já existe movimento com data posterior! ({lastPayment.ToShortDateString()})" };
                            ErrorVisibility = true;
                            return;
                        }
                    }
                }

                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    if (SelectedTransation.Renda == false)
                    {
                        SelectedTransation.ValorPrevisto = 0;
                    }


                    var updateOk = await transactionsService.UpdateRecebimento(transactionId, SelectedTransation);

                    if (updateOk)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = "Erro ao atualizar dados";
                        ToastIcon = "fas fa-exclamation";
                    }

                }

                else // !editMode (Insert) - só acessível a 'outros' recebimentos
                {
                    SelectedTransation.ValorPrevisto = 0;
                    var InsertOk = await transactionsService!.InsertRecebimento(SelectedTransation!);
                    if (InsertOk > 0)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["TituloOperacaoOk"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = "Erro ao inserir pagamento na base de dados";
                        ToastIcon = "fas fa-exclamation";
                    }
                }

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

                await GetTransactions();
                await gridObj!.Refresh();

                EditTransationDialogVisibility = false;
            }
            else // dados inválidos ou em falta
            {
                ErrorVisibility = true;
            }
        }

        protected async Task CustomToolbarItemSelected(MenuEventArgs args)
        {
            var optionSelected = args.Item.Id;

            tbrSelectedValue = optionSelected;

            await GetTransactions();

            if (optionSelected != null)
            {
                switch (optionSelected.ToLower())
                {
                    case "thisyear":
                        DateTime startTY = new DateTime(DateTime.Now.Year, 1, 1);
                        DateTime endTY = startTY.AddYears(1);

                        var TYQry = from record in Transactions
                                    where record.DataMovimento >= startTY
                                     && record.DataMovimento < endTY
                                    select record;
                        pageBadgeCaption = L["TituloDespesasEsteAno"];
                        Transactions = TYQry;

                        break;
                    case "lastyear":
                        DateTime startLY = new DateTime(DateTime.Now.Year, 1, 1).AddYears(-1);
                        DateTime endLY = startLY.AddYears(1);

                        var LYQry = from record in Transactions
                                    where record.DataMovimento >= startLY
                                     && record.DataMovimento < endLY
                                    select record;
                        pageBadgeCaption = L["TituloDespesasAnoAnterior"];
                        Transactions = LYQry;

                        break;
                    case "thismonth":
                        var thisMonthQry = Transactions
                        .Where(p => p.DataMovimento.Year == DateTime.Today.Year &&
                        p.DataMovimento.Date.Month == DateTime.Today.Month);
                        pageBadgeCaption = L["TituloDespesasEsteMes"];

                        Transactions = thisMonthQry;

                        break;
                    case "lastmonth":
                        var lastMonthQry = Transactions
                        .Where(p => p.DataMovimento.Year == DateTime.Today.Year && p.DataMovimento.Date.Month == DateTime.Today.Month - 1);
                        pageBadgeCaption = L["TituloMesAnterior"];

                        Transactions = lastMonthQry;

                        break;
                    case "rentsonly":
                        var rentsQry = Transactions
                            .Where(p => p.Renda == true && p.Estado == 1);

                        pageBadgeCaption = L["TituloRenda"] + "s";

                        Transactions = rentsQry;
                        break;
                    case "partlypaidrents":
                        var partlyPaidRentsQry = Transactions
                            .Where(p => p.Renda && p.Estado == 2);

                        pageBadgeCaption = L["TituloEmFalta"];

                        Transactions = partlyPaidRentsQry;
                        break;
                    case "duerentspayments":
                        var dueRentPaymentsQry = Transactions
                            .Where(p => p.Renda && p.Estado == 3);

                        pageBadgeCaption = L["TituloValorDivida"];

                        Transactions = dueRentPaymentsQry;
                        break;
                    case "otherpayments":
                        var notRentsQry = Transactions
                            .Where(p => p.Renda == false);

                        pageBadgeCaption = L["TituloPagamentosOutrosPagamentos"];

                        Transactions = notRentsQry;
                        break;
                    case "allrents":
                        pageBadgeCaption = L["TituloPagamentosTodosPagamentos"];
                        await GetTransactions();
                        break;
                    default:
                        break;
                }
            }
        }

        protected async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {

            if (args.Item.Id == "Payments_Grid_excelexport")
            {

                var canContinue = Transactions.Count() > 0;
                if (canContinue == false)
                {
                    AlertVisibility = true;
                    alertTitle = "Exportar pagamentos para Excel";
                    WarningMessage = "Não há pagamentos para listar. Verifique, p.f.";
                    return;
                }

                ExcelExportProperties excelExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "Imovel", HeaderText = "Imóvel", Width = "200" });
                ExportColumns.Add(new GridColumn() { Field = "DataMovimento", HeaderText = "Data", Width = "120", Format = "d" });
                ExportColumns.Add(new GridColumn() { Field = "ValorPrevisto", HeaderText = "Previsto", Width = "120", Format = "C2", TextAlign = TextAlign.Right });
                ExportColumns.Add(new GridColumn() { Field = "ValorRecebido", HeaderText = "Recebido", Width = "120", Format = "C2", TextAlign = TextAlign.Right });
                ExportColumns.Add(new GridColumn() { Field = "ValorEmFalta", HeaderText = "Em falta", Width = "120", Format = "C2", TextAlign = TextAlign.Right });
                ExportColumns.Add(
                    new GridColumn()
                    {
                        Field = "Estado",
                        HeaderText = "Estado",
                        Width = "150"
                    });
#pragma warning restore BL0005
                excelExportProperties.Columns = ExportColumns;
                excelExportProperties.IncludeTemplateColumn = true;
                excelExportProperties.IncludeHeaderRow = true;
                excelExportProperties.FileName = $"Payments_{DateTime.Now.ToShortTimeString()}.xlsx";
                await gridObj!.ExportToExcelAsync(excelExportProperties);
            }
        }

        protected async Task PaymentDoubleClickHandler(RecordDoubleClickEventArgs<RecebimentoVM> args)
        {
            transactionId = args.RowData.Id;
            modulo = Modules.Recebimentos;

            SelectedTransation = await transactionsService.GetRecebimento_ById(transactionId);
            OriginalSelectedTransaction = await transactionsService.GetRecebimento_ById(transactionId);
            EditTransationDialogVisibility = true;
            EditCaption = $"Editar dados de pagamento";
            RecordMode = OpcoesRegisto.Gravar;
        }


        public async Task OnTransactionCommandClicked(CommandClickEventArgs<RecebimentoVM> args)
        {
            transactionId = args.RowData.Id;
            modulo = Modules.Recebimentos;
            DeleteCaption = "";

            SelectedTransation = await transactionsService.GetRecebimento_ById(transactionId);
            OriginalSelectedTransaction = await transactionsService.GetRecebimento_ById(transactionId);
            DeleteCaption = $"{SelectedTransation.DataMovimento.ToString("MMMM-yyyy").ToTitleCase()} - {SelectedTransation.Inquilino}";

            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                EditTransationDialogVisibility = true;
                EditCaption = $"Editar dados de pagamento";
                RecordMode = OpcoesRegisto.Gravar;
                StateHasChanged();
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                if (SelectedTransation.ID_TipoRecebimento == 13) // Caução
                {
                    alertTitle = "Valores das cauções não podem ser removidos";
                    WarningMessage = "Esclareça com Administrador da aplicação, p.f.";
                    AlertVisibility = true;
                    return;
                }

                WarningTitle = SelectedTransation.Renda ? L["TituloMarcaRendaComoNaoPaga"] : L["DeleteMsg"] + " " + L["TituloPagamentos"];
                DeleteConfirmVisibility = true;
                StateHasChanged();
            }

            // TODO - ao regularizar, texto da 'Nota' deveria passar a 'regularizado'
            if (args.CommandColumn.Type == CommandButtonType.None) // Acertar pagamento
            {
                WarningMessage = $"Transação: {SelectedTransation.DataMovimento.ToShortDateString()} / {SelectedTransation.ValorEmFalta} ";
                StateHasChanged();
                SettlePaymentConfirmVisibility = true;
            }
        }

        protected async Task ConfirmDeleteYes()
        {
            ToastTitle = $"{L["DeleteMsg"]} {L["TituloPagamentos"]}";
            var expenseDeleted = false;

            try
            {
                var deleteOk = await transactionsService!.DeleteRecebimento(transactionId);

                if (deleteOk == false)
                {
                    ToastCssClass = "e-toast-danger";
                    ToastContent = $"{L["lblExplanation"]}";
                }
                else
                {
                    expenseDeleted = true;
                    ToastCssClass = "e-toast-success";
                    ToastMessage = L["TituloOperacaoOk"];
                }

                DeleteConfirmVisibility = false;

                if (expenseDeleted)
                    await GetTransactions();

                StateHasChanged();

                await Task.Delay(200);
                await ToastObj!.ShowAsync();

            }
            catch (Exception ex)
            {
                expenseDeleted = false;
                ToastCssClass = "e-toast-danger";
                ToastContent = $"Transação não terminou com sucesso. Verifique (erro: {ex.Message})";
                await Task.Delay(200);
                await ToastObj!.ShowAsync();
            }
        }

        protected async Task SettlePayment()
        {
            // Estado: 1 => Pago, 2=> Pago parcialmente, 3=> Em atraso
            SettlePaymentConfirmVisibility = false;
            var paymentState = SelectedTransation.Estado;
            if (paymentState == 3) // renda em atraso
            {
                ToastTitle = L["TituloPagamentoRendaEmAtraso"];
            }
            else //  parcialmente
            {
                ToastTitle = L["TituloPagamentosEmDivida"];
            }

            var processOk = await transactionsService!.AcertaPagamentoRenda(transactionId, paymentState, SelectedTransation.ValorEmFalta);
            if (processOk == false)
            {
                ToastCssClass = "e-toast-danger";
                ToastContent = $"Erro ao acertar pagamento. Verifique log, p.f.";
            }
            else
            {
                ToastCssClass = "e-toast-success";
                ToastMessage = L["TituloOperacaoOk"];
            }

            await GetTransactions();
            StateHasChanged();
            await Task.Delay(200);
            await ToastObj!.ShowAsync();

        }


        protected void IgnoreChangesAlert()
        {
            IsDirty = false;
            ErrorVisibility = false;
            EditTransationDialogVisibility = false;
        }

        protected void ContinueEdit()
        {
            IsDirty = false;
            EditTransationDialogVisibility = true;
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
