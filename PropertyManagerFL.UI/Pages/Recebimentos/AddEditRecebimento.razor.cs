using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using PropertyManagerFL.Application.ViewModels.Recebimentos;
using Syncfusion.Blazor.Calendars;

namespace PropertyManagerFL.UI.Pages.Recebimentos;
public partial class AddEditRecebimento
{

    [Inject] public IArrendamentoService? ArrendamentosService { get; set; }
    [Inject] public IFracaoService? FracoesService { get; set; }
    [Inject] public ILookupTableService? LookupTablesService { get; set; }
    [Inject] IRecebimentoService? RecebimentosService { get; set; }
    [Inject] IStringLocalizer<App>? L { get; set; }

    public IEnumerable<LookupTableVM>? TiposRecebimento { get; set; }
    public IEnumerable<LookupTableVM>? Fracoes { get; set; }

    [Parameter] public RecebimentoVM? SelectedRecord { get; set; }
    [Parameter] public bool EditMode { get; set; }
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
    protected string valorRenda = "";
    protected DateTime ultimoPagamentoRenda = DateTime.MinValue;
    protected bool EnableValue;
    protected bool HideTenantName;
    protected bool HideComboTipoRecebimento = true;

    protected decimal MaxValueAllowed { get; set; }
    protected decimal ValueReceived { get; set; }

    protected DateTime transactionDate { get; set; } = DateTime.Now;

    // TODO mostrar valor em dívida, após selecionar fração
    protected decimal UnitDueValue { get; set; } = 0;
    protected bool DueRentsSelected = false;

    protected bool ErrorVisibility { get; set; } = false;
    protected List<string> ValidationsMessages = new();

    protected bool AlertVisibility { get; set; } = false;
    protected string? alertTitle = "";

    protected bool WarningVisibility { get; set; }
    protected string? WarningMessage { get; set; }


    protected Dictionary<string, object> NotesAttribute = new Dictionary<string, object>()
{
        {"rows", "3" }
};

    protected override void OnInitialized()
    {
        idFracao = SelectedRecord.ID_Propriedade;
        EnableValue = false;
        HideTenantName = true;
        PagamentoRenda = true;
        ValorRenda = 0;
        ValorEmFalta = 0;
    }
    protected override async Task OnParametersSetAsync()
    {
        PagamentoRenda = SelectedRecord.Renda;

        if (EditMode)
        {
            ValorRenda = SelectedRecord!.ValorRecebido;
            ValorEmFalta = SelectedRecord.ValorEmFalta;
            ultimoPagamentoRenda = await ArrendamentosService!.GetLastPaymentDate(SelectedRecord.ID_Propriedade);
            ValueReceived = SelectedRecord!.ValorRecebido;
        }

        if (PagamentoRenda)
        {
            HideComboTipoRecebimento = true;
            idxTipoRecebimento = 99;
        }
        else
        {
            idxTipoRecebimento = SelectedRecord!.ID_TipoRecebimento;
            HideComboTipoRecebimento = false;
        }

        idxFracao = SelectedRecord!.ID_Propriedade;
        TiposRecebimento = (await LookupTablesService!.GetLookupTableData("TipoRecebimento")).ToList();

        Fracoes = (await FracoesService.GetFracoes_Disponiveis()).ToList();
    }

    protected async Task onChangeTipoRecebimento(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, LookupTableVM> args)
    {
        idxTipoRecebimento = args.Value;
        if (idxTipoRecebimento == 1) // pagamento parcial
        {
            DueRentsSelected = true;
            Fracoes = (await FracoesService!.GetFracoes_WithDuePayments()).ToList(); // 11/04/2023
        }
        else
        {
            Fracoes = (await FracoesService.GetFracoes_Disponiveis()).ToList();
        }

        SelectedRecord.ID_TipoRecebimento = idxTipoRecebimento;
        StateHasChanged();
    }

    protected async Task onChangeFracao(Syncfusion.Blazor.DropDowns.ChangeEventArgs<int, LookupTableVM> args)
    {
        idxFracao = args.Value;
        SelectedRecord.ID_Propriedade = idxFracao;
        var unit = await FracoesService.GetFracao_ById(idxFracao);

        if (PagamentoRenda)
        {
            ValorRenda = unit.ValorRenda;
            ultimoPagamentoRenda = await ArrendamentosService.GetLastPaymentDate(idxFracao);
        }
        else
        {
            ValorRenda = 0;
        }


        idInquilino = await ArrendamentosService.GetIdInquilino_ByUnitId(idxFracao);
        if (!EditMode)
        {
            ValorRenda = (await FracoesService.GetFracao_ById(idxFracao)).ValorRenda;
            ultimoPagamentoRenda = await ArrendamentosService.GetLastPaymentDate(idxFracao);
        }

        MaxValueAllowed = await RecebimentosService.GetMaxValueAllowed_ManualInput(idInquilino);
        if (MaxValueAllowed == -1) // no debts, max value can be set for no higher than 3x the value of the fee (rent value)-- could be more... ==> configured in appsetting?
        {
            // máximo = 3x renda (mais que isso, inquilino deverá ter contrato revogado (?))
            MaxValueAllowed = ValorRenda * 3;
        }

        nomeInquilino = await ArrendamentosService.GetNomeInquilino(idInquilino);
        nomeInquilino = nomeInquilino.Replace("\"", "");
        SelectedRecord.ID_Inquilino = idInquilino;

        SelectedRecord.ValorRecebido = ValorRenda;

        EnableValue = true;
        HideTenantName = false;
        StateHasChanged();
    }

    protected void OnDateChange(ChangedEventArgs<DateTime> args)
    {
        SelectedRecord.DataMovimento = args.Value;
        StateHasChanged();
    }

    protected void onAmountChanged(Syncfusion.Blazor.Inputs.ChangeEventArgs<decimal> args)
    {
        var inputAmount = args.Value;
        if (inputAmount > MaxValueAllowed && PagamentoRenda) // TODO não faz sentido  esta validação; este form não permitirá criação de pagamento de rendas
        {
            AlertVisibility = true;
            WarningMessage = $"{L["TituloValorMaximoPermitido"]} {MaxValueAllowed} {L["TituloUltrapassado"]}. {L["TituloVerificar"]}";

            alertTitle = L["TituloPagamentosOutrosPagamentos"];
            StateHasChanged();
            return;
        }

        if (PagamentoRenda)
        {
            var inDebt = ValorRenda - inputAmount;
            SelectedRecord!.ValorEmFalta = inDebt;
        }
        else
        {
            SelectedRecord!.ValorEmFalta = 0;
        }

        StateHasChanged();
    }

    private void OnChangeRent(Syncfusion.Blazor.Buttons.ChangeEventArgs<bool> args)
    {
        PagamentoRenda = args.Checked == true;
        HideComboTipoRecebimento = PagamentoRenda;
        SelectedRecord!.Renda = PagamentoRenda;
    }
}