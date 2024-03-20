using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;

namespace PropertyManagerFL.UI.Pages.Notifications;
public partial class NotificationPopup
{
    [Parameter] public bool showPopup { get; set; }
    [Parameter] public EventCallback<int> Alerts { get; set; }

    [Inject] public IArrendamentoService? LeasesService { get; set; }
    [Inject] public IInquilinoService? TenantsService { get; set; }
    [Inject] public IRecebimentoService? RentPaymentsService { get; set; }
    [Inject] protected IAppSettingsService? AppSettingsService { get; set; }
    [Inject] protected IAppointmentsService? AppointmentsService { get; set; }
    [Inject] public ILogger<App>? Logger { get; set; }
    [Inject] public IStringLocalizer<App>? L { get; set; }

    protected List<string> AppAlerts = new();

    protected IEnumerable<ArrendamentoVM>? Leases { get; set; }
    protected ApplicationSettingsVM? AppSettings { get; set; } = new();

    //protected List<string> Alerts { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {
        Leases = await GetAllLeases();
        if (Leases is not null && Leases.Any())
        {
            AppSettings = await GetSettings();
            await CheckForAlerts();
        }
    }

    protected async Task<ApplicationSettingsVM> GetSettings()
    {
        return await AppSettingsService!.GetSettingsAsync();
    }

    private async Task CheckForAlerts()
    {
        AppAlerts.Clear();
        var appts = await AppointmentsService!.GetAllAsync();
        if(appts is not null && appts.Any())
        {
            var todayAppts = appts.Where(a => a.StartTime.Date == DateTime.Now.Date);
            if (todayAppts.Any())
            {
                foreach (var todayAppt in todayAppts)
                {
                    var notes = todayAppt.Description ?? "";
                    AppAlerts.Add($"{todayAppt.Subject} {todayAppt.Location} ({todayAppt.StartTime.ToShortTimeString()}) {notes}");
                }
            }
        }

        if (AppSettings?.CartasAumentoAutomaticas == false)
        {
            // envio de carta de aumento Manual ==> Tipo de documento = 16 => 'Carta de atualização de renda' 
            var tenantDocuments = await TenantsService!.GetDocumentos();
            var updateLetterSentCurrentYear = tenantDocuments.Where(td => td.DocumentType == 16 && td.CreationDate.Year < DateTime.Now.Year);
            if (updateLetterSentCurrentYear.Any())
            {
                foreach (var document in tenantDocuments)
                {
                    AppAlerts.Add($"Necessário envio de carta de atualização ao inquilino {document.NomeInquilino}");
                }
            }

            var leasesWhoNeedToUpdateRent = Leases?.Where(l => l.Data_Inicio.Month == DateTime.Now.Month + 1);
            if (leasesWhoNeedToUpdateRent?.Count() > 0)
            {
                foreach (var item in leasesWhoNeedToUpdateRent)
                {
                    var alertMsg = $"Necessário atualizar renda do inquilino {item.NomeInquilino} ({item.Fracao})";
                    if (item.EnvioCartaAtualizacaoRenda == false)
                        alertMsg += " - não foi enviada carta de atualização!";

                    AppAlerts.Add(alertMsg);
                }
            }
            else // Cartas de aumento de rendas automáticas
            {
                if (Leases?.Count() > 0)
                {
                    var rentPayments = (await RentPaymentsService!.GetAll()).ToList().Count();
                    if (rentPayments > 0)
                    {
                        var UpdateLetterSent = await LeasesService!.CartaAtualizacaoRendasEmitida(DateTime.Now.Year);
                        if (UpdateLetterSent == false)
                        {
                            AppAlerts.Add("Cartas de atualização de rendas não foram emitidas para o ano corrente");
                        }
                    }
                }
            }
        }

        await Alerts.InvokeAsync(AppAlerts.Count);
        StateHasChanged();
    }

    protected async Task<IEnumerable<ArrendamentoVM>> GetAllLeases()
    {
        try
        {
            IEnumerable<ArrendamentoVM> listOfleases = await LeasesService!.GetAll();
            if (listOfleases is not null && listOfleases.Any())
            {
                listOfleases = listOfleases.OrderByDescending(l => l.Id).ToList();
                return listOfleases;
            }
            else
                return new List<ArrendamentoVM>();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex.Message, ex);
            return new List<ArrendamentoVM>();

        }
    }
    protected int GetMonthDifference(DateTime startDate, DateTime endDate)
    {
        if (endDate.Date < startDate.Date)
        {
            return 0;
        }

        int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
        return Math.Abs(monthsApart);
    }
}