using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using PropertyManagerFLApplication.Utilities;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System.Globalization;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;
using static PropertyManagerFL.UI.Components.ConfirmMonthlyRentProcessing;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class MonthlyRentProcessingBase : ComponentBase, IDisposable
    {
        [Inject] protected IStringLocalizer<App> L { get; set; }
        [Inject] protected IRecebimentoService TransactionsService { get; set; }
        [Inject] protected IFracaoService UnitsService { get; set; }
        [Inject] protected IArrendamentoService LeasesService { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected ILogger<App> _logger { get; set; }


        protected IEnumerable<RecebimentoVM>? Transactions { get; set; }
        protected IEnumerable<RecebimentoVM>? BatchTransactions { get; set; }

        protected SfToast? ToastObj { get; set; }
        protected SfSpinner? SpinnerObj { get; set; }


        protected string ToastTitle = "";
        protected string ToastContent = "";
        protected string ToastCssClass = "";

        protected string? ToastMessage;
        protected string? ToastCss;
        protected string? ToastIcon;


        protected string SpinnerLabel = "";

        protected bool WarningVisibility { get; set; }
        protected string? WarningMessage { get; set; }

        protected int TransactionId;
        protected bool ConfirmVisibility { get; set; } = false;
        protected string? AlertTitle = "";

        protected string WarningCaption = "";
        protected string WarningTitle = "";

        protected bool EditTransationDialogVisibility { get; set; } = false;
        protected bool SettlePaymentConfirmVisibility { get; set; } = false;
        protected List<string> Error_Warnings_Msgs = new();
        protected bool ErrorVisibility { get; set; } = false;
        protected bool AddEditMonthlyTransactionsVisibility { get; set; } = false;

        protected string MonthToProcess = string.Empty;
        protected string CurrentCulture = CultureInfo.CurrentCulture.Name;


        protected OpcoesRegisto WarningType = OpcoesRegisto.Warning;
        protected AlertMessageType AlertMessageType = AlertMessageType.Warning;

        protected bool NoProcessedMonth { get; set; } = false;
        protected IEnumerable<ProcessamentoRendasDTO>? ProcessedRents { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var retMessage = await CheckIfConditionsMet();
            if (!string.IsNullOrEmpty(retMessage))
            {
                WarningMessage = retMessage;
                WarningVisibility = true;
                return;
            }

            AlertTitle = L["TituloProcessamentoRendas"];
            ToastTitle = AlertTitle;


            ProcessedRents = await GetMonthlyProcessedRents();
            if (ProcessedRents is null)
            {
                NoProcessedMonth = true;
            }

            if (CurrentCulture == "pt")
                CurrentCulture = "pt-PT";

            TextInfo myTI = new CultureInfo(CurrentCulture, false).TextInfo;
            MonthToProcess = $"{DateTime.Now.AddMonths(1).ToString("MMMM", CultureInfo.CreateSpecificCulture(CurrentCulture))}  {DateTime.Now.Year}";
            MonthToProcess = myTI.ToTitleCase(MonthToProcess);

            ConfirmVisibility = false;
            ToastTitle = "";
            ToastMessage = "";
            ToastCss = "e-toast-success";

            WarningVisibility = false;
            WarningMessage = "";

        }

        private async Task<IEnumerable<ProcessamentoRendasDTO>> GetMonthlyProcessedRents()
        {
            try
            {
                return await TransactionsService.GetMonthlyRentsProcessed(DateTime.Now.Year);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString(), ex);
                return null;
            }
        }
        private async Task<string> CheckIfConditionsMet()
        {
            // Antes de criar transação, verificar se há registo de frações/arrendamento disponíveis...
            var errorMessage = "";
            var leases = await LeasesService.GetAll();
            var canContinue = leases is not null || leases.Count() > 0;
            if (leases.Any() == false)
            {
                WarningVisibility = true;
                errorMessage = L["TituloAlertaSemArrendamentos"];
            }

            canContinue = await UnitsService!.GetFracoes_Disponiveis() is not null;
            if (canContinue == false)
            {
                WarningVisibility = true;
                errorMessage = L["TituloAlertaSemFracoes"];
            }

            return errorMessage;
        }

        /// <summary>
        /// Get all leases
        /// </summary>
        /// <returns></returns>
        protected async Task<IEnumerable<ArrendamentoVM>> GetAll()
        {
            try
            {
                return await LeasesService!.GetAll();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Processamento mensal de rendas
        /// </summary>
        /// <param name="args">Selected month and year</param>
        /// <returns></returns>
        protected async Task ProcessMonthlyRents(EventCallbackArgs args)
        {
            try
            {
                var selectedMonth = args.ProcessingMonth;
                var selectedYear = args.ProcessingYear;
                var errorMessage = await IsSelectedPeriodValid(selectedMonth, selectedYear);

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    WarningVisibility = true;
                    WarningMessage = errorMessage;
                    return;
                }

                var alreadyPerformed = await RentalProcessingAlreadyMade(selectedMonth, selectedYear);
                if (alreadyPerformed == false)
                {
                    ConfirmVisibility = false; // dialog to confirm monthly rent processing

                    // No processamento de cartas de atualização, rendas deixaram de ser atualizadas
                    // Na geração de pagamento de rendas (abaixo), deverá ser tomado em
                    // linha de conta que necessitam de alteração.
                    // Ao serem os mostrados os movimentos (antes da confirmação), indicar
                    // quais as frações cujo valor da renda foi alterado.

                    BatchTransactions = await GenerateRents(selectedMonth, selectedYear);
                    if (BatchTransactions.Any())
                    {
                        ToastTitle = L["TituloGerarPagamentos"];
                        ToastMessage = L["TituloSucesso"];
                        ToastCss = "e-toast-success";
                        StateHasChanged();
                        await Task.Delay(100);
                        await ToastObj!.ShowAsync();

                        AddEditMonthlyTransactionsVisibility = true;
                    }
                    else
                    {
                        WarningVisibility = true;
                        WarningMessage = L["TituloNaoHaPagamentosAProcessar"];
                        return;
                    }
                }
                else
                {
                    WarningVisibility = true;
                    WarningMessage = L["TituloPagamentoRendasJaEfetuado"];
                    return;
                }

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString(), ex);
            }
        }

        public async Task HandleBatchTransaction()
        {
            try
            {
                var transactionResult = await TransactionsService!.ProcessMonthlyRentPayments();
                if (transactionResult == 1)
                {
                    WarningType = OpcoesRegisto.Info;
                    WarningMessage = L["TituloOperacaoOk"];
                    AlertMessageType = AlertMessageType.Success;
                    ProcessedRents = await GetMonthlyProcessedRents();
                }
                else
                {
                    WarningType = OpcoesRegisto.Error;
                    WarningMessage = L["TituloErroCriacaoPagamentos"];
                    AlertMessageType = AlertMessageType.Error;
                    _logger?.LogError(WarningMessage);
                }
            }
            catch (Exception ex)
            {
                WarningType = OpcoesRegisto.Error;
                WarningMessage = L["TituloErroCriacaoPagamentos"];
                AlertMessageType = AlertMessageType.Error;
                _logger?.LogError(WarningMessage);

            }
            finally
            {
                StateHasChanged();
                WarningVisibility = true;
            }
        }

        /// <summary>
        /// Cancela processamento de rendas. Lê e apaga registos (temp) gerados.
        /// </summary>
        /// <returns></returns>
        public async Task HandleCancelBatchTransaction()
        {
            try
            {
                var temporaryPayments = GetTemporaryPayments();

                if (temporaryPayments != null)
                {
                    await DeleteTemporaryPayments();

                    WarningType = OpcoesRegisto.Info;
                    AlertMessageType = AlertMessageType.Success;
                    WarningMessage = L["TituloOperacaoOk"];
                }
                else
                {
                    AlertMessageType = AlertMessageType.Error;
                    _logger?.LogWarning($"{L["TituloProcessamentoRendasCancelado"]}.");
                    WarningVisibility = true;

                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString(), ex);
                WarningType = OpcoesRegisto.Error;
                WarningMessage = $"{L["TituloProcessamentoRendasCancelado"]} ({ex.ToString()}). {L["TituloVerificar"]}";
                AlertMessageType = AlertMessageType.Error;
                WarningVisibility = true;
            }

            //AddEditMonthlyTransactionsVisibility = false;
            StateHasChanged();
        }

        private async Task<string> IsSelectedPeriodValid(int selectedMonth, int selectedYear)
        {
            string errorMessage = "";
            try
            {
                var lastPeriodProcessed = await TransactionsService!.GetLastPeriodProcessed();
                if (lastPeriodProcessed is not null)
                {
                    var lastMonthProcessed = lastPeriodProcessed.Mes;
                    var lastYearProcessed = lastPeriodProcessed.Ano;
                    if (selectedYear < lastPeriodProcessed.Ano)
                    {
                        errorMessage = $"Período inválido ({selectedMonth}/{selectedYear}). Último processamento: {lastMonthProcessed} / {lastYearProcessed}";
                    }
                    else if (selectedYear == lastPeriodProcessed.Ano && selectedMonth < lastPeriodProcessed.Mes)
                    {
                        errorMessage = "Período inválido. Verifique, p.f.";
                    }
                    else if (selectedYear == lastPeriodProcessed.Ano && selectedMonth > lastPeriodProcessed.Mes + 1)
                    {
                        var dateDecoded = Helpers.DecodeMonthYear(lastMonthProcessed, lastYearProcessed);
                        errorMessage = $"Mês inválido. Último mês processado: {dateDecoded}. Verifique, p.f.";
                    }
                }

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString(), ex);
                errorMessage = ex.ToString();
            }

            return errorMessage;
        }

        private async Task<bool> RentalProcessingAlreadyMade(int selectedMonth, int selectedYear)
        {
            try
            {
                return await TransactionsService!.RentalProcessingPerformed(selectedMonth, selectedYear);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString(), ex);
                return true;
            }
        }

        private async Task<IEnumerable<RecebimentoVM>> GenerateRents(int selectedMonth, int selectedYear)
        {
            try
            {
                return (await TransactionsService!.GeneratePagamentoRendas(selectedMonth + 1, selectedYear)).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString(), ex);
                return null;
            }
        }

        private async Task<IEnumerable<RecebimentoVM>> GetTemporaryPayments()
        {
            try
            {
                return await TransactionsService!.GetAllTemp();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString(), ex);
                return null;
            }
        }

        private async Task DeleteTemporaryPayments()
        {
            try
            {
                await TransactionsService.DeleteRecebimentosTemp();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString(), ex);
            }
        }


        protected async Task CloseDialog()
        {
            ToastMessage = L["TituloProcessoTerminado"];
            ToastCss = "e-toast-success";
            StateHasChanged();
            await Task.Delay(100);
            await ToastObj!.ShowAsync();
            await Task.Delay(1000);

            GetBack();
        }

        protected void GetBack()
        {
            WarningVisibility = false;
            ConfirmVisibility = false;
            AddEditMonthlyTransactionsVisibility = false;
            NavigationManager!.NavigateTo("/monthlyrentprocessing");
        }

        protected async Task HideToast()
        {
            await ToastObj!.HideAsync();
        }

        public void Dispose()
        {
            SpinnerObj?.Dispose();
            ToastObj?.Dispose();
        }

    }
}
