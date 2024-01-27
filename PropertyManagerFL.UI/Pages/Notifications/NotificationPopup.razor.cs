using Microsoft.AspNetCore.Components;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Arrendamentos;

namespace PropertyManagerFL.UI.Pages.Notifications;
public partial class NotificationPopup
{
    [Parameter] public bool showPopup { get; set; }
    [Parameter] public EventCallback<bool> ShowPopupChanged { get; set; }

    [Inject] public IArrendamentoService? arrendamentosService { get; set; }
    [Inject] public IInquilinoService? inquilinosService { get; set; }
    [Inject] public IRecebimentoService? recebimentosService { get; set; }
    [Inject] public ILogger<App>? logger { get; set; }

    protected IEnumerable<ArrendamentoVM>? leases { get; set; }

    protected List<string> Alerts { get; set; } = new();
    protected override async Task OnInitializedAsync()
    {
        leases = await GetAllLeases();
        if (leases is not null && leases.Any())
        {
            await CheckForAlerts();
        }
    }

    private async Task CheckForAlerts()
    {
        Alerts.Clear();
        var tenantDocuments = await inquilinosService!.GetDocumentos();
        var updateLetterSentCurrentYear = tenantDocuments.Where(td => td.DocumentType == 16 && td.CreationDate.Year < DateTime.Now.Year);
        if (updateLetterSentCurrentYear.Any())
        {
            foreach (var document in tenantDocuments)
            {
                Alerts.Add($"Necessário envio de carta de atualização ao inquilino {document.NomeInquilino}");
            }
        }

        var leasesWhoNeedToUpdateRent =
            leases?.Where(l => l.Data_Inicio.Month == DateTime.Now.Month + 1);
        if (leasesWhoNeedToUpdateRent is not null && leasesWhoNeedToUpdateRent.Any())
        {
            foreach (var item in leasesWhoNeedToUpdateRent)
            {
                var alertMsg = $"Necessário atualizar renda do inquilino {item.NomeInquilino} ({item.Fracao})";
                if (item.EnvioCartaAtualizacaoRenda == false)
                    alertMsg += " - não foi enviada carta de atualização!";

                Alerts.Add(alertMsg);
            }
        }

        foreach (var _leaseitem in leases)
        {
            var monthsToEnd = GetMonthDifference(DateTime.Now, _leaseitem.Data_Fim);
            if (monthsToEnd >= 0 && monthsToEnd <= 4)
            {
                Alerts.Add($"Contrato do inquilino {_leaseitem.NomeInquilino} ({_leaseitem.Fracao}) está prestes a terminar. Renovar data-fim, ou enviar carta de revogação");
            }
        }

        // Este procedimento está condicionado pela chamada feita no início: (leases is not null)~=> não faz sentido haver recebimentos se não houver contratos ('leases')
        var rentPayments = (await recebimentosService!.GetAll()).ToList().Count();
        if (rentPayments > 0)
        {
            var UpdateLetterSent = await arrendamentosService!.CartaAtualizacaoRendasEmitida(DateTime.Now.Year);
            if (UpdateLetterSent == false)
            {
                Alerts.Add("Cartas de atualização de rendas não foram emitidas para o ano corrente!! Diploma é publicado em Outubro; cartas deverão ser enviadas antes do fim do ano.");
            }
        }
    }

    protected async Task<IEnumerable<ArrendamentoVM>> GetAllLeases()
    {
        try
        {
            IEnumerable<ArrendamentoVM> listOfleases = await arrendamentosService!.GetAll();
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
            logger?.LogError(ex.Message, ex);
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