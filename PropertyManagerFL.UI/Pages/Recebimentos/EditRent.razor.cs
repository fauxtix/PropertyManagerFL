using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Recebimentos;

namespace PropertyManagerFL.UI.Pages.Recebimentos;
public partial class EditRent
{

    [Inject] public IArrendamentoService? ArrendamentosService { get; set; }
    [Inject] IRecebimentoService? RecebimentosService { get; set; }
    [Inject] protected IStringLocalizer<App>? L { get; set; }

    [Parameter] public RecebimentoVM? SelectedRecord { get; set; }
    [Parameter] public string? HeaderCaption { get; set; }

    protected int idxTipoRecebimento;
    protected int idxFracao;

    protected decimal ValorRenda { get; set; }
    protected decimal ValorEmFalta { get; set; }
    protected bool PagamentoRenda { get; set; }

    protected int idTipoRecebimento;
    protected int idFracao;
    protected int idInquilino;
    protected string nomeInquilino = "";
    protected DateTime ultimoPagamentoRenda = DateTime.MinValue;
    protected bool EnableValue;
    protected bool HideTenantName;
    protected bool HideComboTipoRecebimento = true;

    protected decimal MaxValueAllowed { get; set; }
    protected decimal ValueReceived { get; set; }

    protected DateTime transactionDate { get; set; } = DateTime.Now;

    protected bool ErrorVisibility { get; set; } = false;
    protected List<string> ValidationsMessages = new();

    protected bool AlertVisibility { get; set; } = false;
    protected string? AlertTitle = "";

    protected bool HideMessageVisibility { get; set; }
    protected bool WarningVisibility { get; set; }
    protected string? WarningMessage { get; set; }

    protected string InDebtColor = "e-normal";


    protected Dictionary<string, object> NotesAttribute = new Dictionary<string, object>()
{
        {"rows", "3" }
};
    protected Dictionary<string, object> ColorAttribute = new Dictionary<string, object>()
{
        {"color", "yellow" }
};

    protected override void OnInitialized()
    {
        idFracao = SelectedRecord.ID_Propriedade;
        EnableValue = false;
        HideTenantName = true;
        PagamentoRenda = true;
        ValorRenda = 0;
        ValorEmFalta = 0;

        HideMessageVisibility = true;
    }
    protected override async Task OnParametersSetAsync()
    {
        PagamentoRenda = SelectedRecord.Renda;

        ValorRenda = SelectedRecord!.ValorRecebido;
        ValorEmFalta = SelectedRecord.ValorEmFalta;
        ultimoPagamentoRenda = await ArrendamentosService!.GetLastPaymentDate(SelectedRecord.ID_Propriedade);
        ValueReceived = SelectedRecord!.ValorRecebido;
        if (ValorEmFalta > 0)
            InDebtColor = "e-warning";
    }

    protected async void onAmountChanged(Syncfusion.Blazor.Inputs.ChangeEventArgs<decimal> args)
    {
        var inputAmount = args.Value;
        if (inputAmount > SelectedRecord!.ValorPrevisto)
        {
            AlertVisibility = true;
            AlertTitle = L["TituloValorRecebidoAlterado"];
            WarningMessage = $"{L["TituloValorMaximoPermitido"]} {SelectedRecord.ValorPrevisto} {L["TituloUltrapassado"]}. {L["TituloVerificar"]}";
            StateHasChanged();
            return;
        }

        if (SelectedRecord?.ValorPrevisto != inputAmount)
        {
            SelectedRecord!.ValorRecebido = inputAmount;
            SelectedRecord!.ValorEmFalta = SelectedRecord.ValorPrevisto - inputAmount;
            SelectedRecord.Estado = 2; // pago parcialmente
            HideMessageVisibility = false;
            InDebtColor = "e-danger";
        }
        else
        {
            SelectedRecord.Estado = 3; // pago na totalidade
            SelectedRecord!.ValorEmFalta = 0;
            SelectedRecord!.ValorRecebido = inputAmount;
        }
        StateHasChanged();
    }
}