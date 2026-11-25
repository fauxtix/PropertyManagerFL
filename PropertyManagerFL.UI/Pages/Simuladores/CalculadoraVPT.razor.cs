using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using Syncfusion.Blazor.DropDowns;

namespace PropertyManagerFL.UI.Pages.Simuladores;


public partial class CalculadoraVPT
{
    [Inject] public IImovelService? ImoveisService { get; set; }
    [Inject] public IFracaoService? FracoesService { get; set; }
    [Inject] public IStringLocalizer<App> L { get; set; }
    [Inject] public NavigationManager? navigation { get; set; }


    protected IEnumerable<LookupTableVM>? PropertiesLookup { get; set; }
    protected List<LookupTableVM>? UnitsLookup { get; set; } = new();
    protected List<FracaoVM>? Units { get; set; } = new();

    protected decimal ValorPatrimonio;
    protected string? AnoAvaliacao = string.Empty;
    int IdadePredio = 0;


    double ValorPatrimonialTributario = 0;
    double PrecoConstrucaoMetroQuadrado = 603.00;
    double AreaBrutaConstrucao = 0.00;
    double CoeficienteAfetacao = 1.00;
    double CoeficienteLocalizacao = 1.20;
    double CoeficienteQualidadeConforto = 0.98;
    double CoeficienteVetustez = 0;

    protected int idxProperty;
    protected int idxUnit;
    protected bool ShowUnitsCombo;
    protected bool HideResults = true;
    protected bool HasProperties => PropertiesLookup?.Any() == true;
    protected bool CanCalculate => string.IsNullOrEmpty(CalculateRequirementMessage);

    protected string CalculateRequirementMessage =>
        !HasProperties ? L["CalculadoraSemImoveis"]
        : idxProperty == 0 ? L["CalculadoraSelecioneImovelEFracao"]
        : UnitsLookup?.Any() != true ? L["CalculadoraSemFracoes"]
        : idxUnit == 0 ? L["CalculadoraSelecioneImovelEFracao"]
        : ValorPatrimonio <= 0 || AreaBrutaConstrucao <= 0 || CoeficienteVetustez <= 0 || PrecoConstrucaoMetroQuadrado <= 0
            ? L["CalculadoraDadosInsuficientes"]
        : string.Empty;

    protected bool ShowCalculateButton => CanCalculate;

    protected SfDropDownList<int, LookupTableVM>? ddlUnits;
    protected override async Task OnInitializedAsync()
    {
        PropertiesLookup = await ImoveisService!.GetPropertiesAsLookupTables() ?? Enumerable.Empty<LookupTableVM>();
        ValorPatrimonialTributario = 0;
    }
    private void Calculate()
    {
        if (!CanCalculate) return;

        ValorPatrimonialTributario = Math.Round(PrecoConstrucaoMetroQuadrado * AreaBrutaConstrucao *
        CoeficienteAfetacao * CoeficienteLocalizacao * CoeficienteQualidadeConforto * CoeficienteVetustez);
        ValorPatrimonialTributario = (10 - ValorPatrimonialTributario % 10) + ValorPatrimonialTributario;

        HideResults = false;
    }

    protected async Task OnChangeProperty(ChangeEventArgs<int, LookupTableVM> args)
    {
        idxProperty = args.Value;
        idxUnit = 0;
        ShowUnitsCombo = false;
        HideResults = true;
        ValorPatrimonialTributario = 0;
        ValorPatrimonio = 0;
        AreaBrutaConstrucao = 0;
        AnoAvaliacao = string.Empty;
        CoeficienteVetustez = 0;

        UnitsLookup = new();
        Units = new();

        if (idxProperty == 0) return;

        if (ddlUnits is not null)
        {
            await ddlUnits.ClearAsync();
        }

        Units = (await FracoesService!.GetFracoes_Imovel(idxProperty)).ToList();
        foreach (var unit in Units)
        {
            UnitsLookup!.Add(
                 new LookupTableVM { Id = unit.Id, Descricao = unit.Descricao });
        }

        var property = await ImoveisService.GetImovel_ById(idxProperty);
        if (property is not null && int.TryParse(property.AnoConstrucao, out var anoConstrucao))
        {
            IdadePredio = DateTime.Now.Year - anoConstrucao;
            CoeficienteVetustez = CalcularCoeficienteVetustez(IdadePredio);
        }
        else
        {
            CoeficienteVetustez = 0.80;
        }

        // ap�s escolha do im�vel, e filtragem das fra��es pertencentes ao mesmo, disponibiliza combo das fra��es
        ShowUnitsCombo = true;
        StateHasChanged();
    }

    protected async Task OnChangeUnit(ChangeEventArgs<int, LookupTableVM> args)
    {
        idxUnit = args.Value;
        if (idxUnit < 1)
        {
            idxUnit = 0;
            ValorPatrimonio = 0;
            AreaBrutaConstrucao = 0;
            return;
        }

        var unit = await FracoesService!.GetFracao_ById(idxUnit);
        // ao escolher uma fra��o, guarda valor tribut�vel
        ValorPatrimonio = unit.ValorUltAvaliacao;
        AreaBrutaConstrucao = unit.AreaBrutaPrivativa + unit.AreaBrutaDependente;
        AnoAvaliacao = unit.AnoUltAvaliacao;

        StateHasChanged();
    }

    private double CalcularCoeficienteVetustez(int anosPredio)
    {
        double output = 0.00;
        switch (anosPredio)
        {
            case int idade when idade < 2:
                output = 1; break;
            case int idade when idade >= 2 && idade <= 8:
                output = 0.9; break;
            case int idade when idade >= 9 && idade <= 15:
                output = 0.85; break;
            case int idade when idade >= 16 && idade <= 25:
                output = 0.80; break;
            case int idade when idade >= 26 && idade <= 40:
                output = 0.75; break;
            case int idade when idade >= 41 && idade <= 50:
                output = 0.65; break;
            case int idade when idade >= 51 && idade <= 60:
                output = 0.55; break;
            case int idade when idade > 60:
                output = 0.4; break;
        }
        return output;
    }

    protected async Task Clear()
    {
        idxProperty = 0;
        idxUnit = 0;
        ShowUnitsCombo = false;
        HideResults = true;
        ValorPatrimonialTributario = 0;
        ValorPatrimonio = 0;
        AreaBrutaConstrucao = 0;
        AnoAvaliacao = string.Empty;
        CoeficienteVetustez = 0;
        UnitsLookup = new();
        Units = new();
        if (ddlUnits is not null)
        {
            await ddlUnits.ClearAsync();
        }

    }
    protected void GoBack()
    {
        navigation?.NavigateTo("/");
    }
}