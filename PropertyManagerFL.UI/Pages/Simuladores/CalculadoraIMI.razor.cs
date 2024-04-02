using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.LookupTables;
using Syncfusion.Blazor.DropDowns;

namespace PropertyManagerFL.UI.Pages.Simuladores;
public partial class CalculadoraIMI
{

    /*
    * Se o valor do IMI for igual ou inferior a 100 euros, tem de ser pago na totalidade, em maio.
    * Caso o valor do imposto seja superior a 100 euros e inferior a 500 euros, o pagamento é efetuado em duas prestações, em maio e novembro.
    * A partir de 500 euros, o valor de IMI é pago em três prestações, em maio, agosto e novembro.
    */

    [Inject] public IImovelService? ImoveisService { get; set; }
    [Inject] public IFracaoService? FracoesService { get; set; }
    [Inject] public IDistritosConcelhosService? DistritosConcelhosService { get; set; }
    [Inject] public IStringLocalizer<App> L { get; set; }
    [Inject] public NavigationManager? navigation { get; set; }


    protected IEnumerable<LookupTableVM>? PropertiesLookup { get; set; }
    protected List<LookupTableVM>? UnitsLookup { get; set; } = new();
    protected List<FracaoVM>? Units { get; set; } = new();

    protected IEnumerable<LookupTableVM>? Distritos { get; set; }
    protected IEnumerable<DistritoConcelho>? Concelhos { get; set; }
    protected bool distritoSelected;
    protected int idxDistrito;
    protected int idxConcelho;
    protected int idxTipoImovel;
    protected double coeficiente;
    protected string coeficienteCaption = string.Empty;
    protected decimal valorPatrimonio;
    protected decimal valorIMI;
    protected decimal valorPrestacaoIMI;
    protected List<string> listaCaptionsPrestacoes = new();

    protected bool generalUse = false;
    protected int idxProperty;
    protected int idxUnit;
    protected bool ShowUnitsCombo;
    protected bool HideResults = true;
    protected string? coefCaption;

    protected SfDropDownList<int, LookupTableVM>? ddlUnits;

    protected int pagamentosFracionados = 1;

    protected bool ShowCalculateButton = false;
    protected bool ResultVisibility = false;
    protected string? DescricaoImovel = "";
    protected string? DescricaoFracao = "";

    protected List<LookupTableVM> TiposImovel { get; set; } = new List<LookupTableVM>()
{
    new LookupTableVM{Id = 1, Descricao = "Urbano"},
    new LookupTableVM{Id = 2, Descricao = "Rústico"}
};
    protected override async Task OnInitializedAsync()
    {
        coeficienteCaption = string.Empty;
        idxTipoImovel = 1;
        coeficiente = 0;
        valorIMI = 0;
        distritoSelected = false;
        Distritos = (await DistritosConcelhosService!.GetDistritos()).ToList();
        Concelhos = (await DistritosConcelhosService.GetConcelhos()).ToList();
        PropertiesLookup = await ImoveisService!.GetPropertiesAsLookupTables();
    }

    protected async void OnChangeDistrito(ChangeEventArgs<int, LookupTableVM> args)
    {
        idxDistrito = args.Value;
        distritoSelected = true;

        // ao escolher distrito, devolve concelhos pertencentes a este; permite visualização da respetiva combo (distritoSelected = true)
        Concelhos = (await DistritosConcelhosService!.GetConcelhosByDistrito(idxDistrito)).ToList();
        StateHasChanged();
    }
    protected void OnChangeConcelho(ChangeEventArgs<int, DistritoConcelho> args)
    {
        if (args.Value == 0) return;

        idxConcelho = args.Value;
        // ao escolher concelho, determina qual o coeficiente aplicável ao mesmo
        coeficiente = args.ItemData.Coeficiente;

        // valor em % para apresentar ao utilizador
        coeficienteCaption = Math.Round(args.ItemData.Coeficiente * 100, 2).ToString();

        StateHasChanged();
    }
    protected void OnChangeTipoImovel(ChangeEventArgs<int, LookupTableVM> args)
    {
        idxTipoImovel = args.Value;
        StateHasChanged();
    }

    protected async Task OnChangeProperty(ChangeEventArgs<int, LookupTableVM> args)
    {
        idxProperty = args.Value;

        if (idxProperty == 0) return;

        var freguesia = (await ImoveisService.GetImovel_ById(idxProperty)).FreguesiaImovel;
        if (freguesia is not null)
        {
            var concelhos = await DistritosConcelhosService.GetConcelhos();
            Tuple<int, int, float>? codes = concelhos
                .Where(p => p.Descricao == freguesia)
                .Select(c => new { c.IdDistrito, c.Id, c.Coeficiente })
                .AsEnumerable()
                .Select(c => new Tuple<int, int, float>(c.IdDistrito, c.Id, c.Coeficiente))
                .SingleOrDefault();

            if (codes?.Item1 > 0 && codes?.Item2 > 0)
            {
                idxDistrito = codes.Item1;
                idxConcelho = codes.Item2;
                coeficiente = codes.Item3;
                coeficienteCaption = Math.Round(coeficiente * 100, 2).ToString();

            }
            else
            {
                idxDistrito = 0;
                idxConcelho = 0;
                coeficiente = 0;
            }

            await ddlUnits!.ClearAsync();

            StateHasChanged();
        }

        UnitsLookup = new();
        Units = (await FracoesService!.GetFracoes_Imovel(idxProperty)).ToList();
        foreach (var unit in Units)
        {
            UnitsLookup!.Add(
                 new LookupTableVM { Id = unit.Id, Descricao = unit.Descricao });
        }

        // após escolha do imóvel, e filtragem das frações pertencentes ao mesmo, disponibiliza combo das frações
        ShowUnitsCombo = true;
    }
    protected async Task OnChangeUnit(ChangeEventArgs<int, LookupTableVM> args)
    {
        if (args.Value < 1) return;
        var unit = await FracoesService!.GetFracao_ById(args.Value);
        // ao escolher uma fração, guarda valor tributável
        valorPatrimonio = unit.ValorUltAvaliacao;

        DescricaoImovel = unit.DescricaoImovel;
        DescricaoFracao = unit.Descricao;

        ShowCalculateButton = true;

    }

    protected async Task Clear()
    {
        idxProperty = 0;
        idxDistrito = 0;
        idxTipoImovel = 1;
        ShowUnitsCombo = false;
        HideResults = true;
        valorPatrimonio = 0;
        valorIMI = 0;
        valorPrestacaoIMI = 0;
        ShowCalculateButton = false;
        await ddlUnits!.ClearAsync();
    }

    protected void Calculate()
    {
        listaCaptionsPrestacoes.Clear();
        coefCaption = $"{coeficiente:F2}%";
        valorIMI = Math.Round(valorPatrimonio * (decimal)coeficiente, 2);
        switch (valorIMI)
        {
            case decimal valor when valor <= 100:
                pagamentosFracionados = 1;
                listaCaptionsPrestacoes = new List<string>
            {
                "Maio"
            };
                break;
            case decimal valor when valor < 500:
                pagamentosFracionados = 2;
                listaCaptionsPrestacoes = new List<string>
            {
                "Maio", "Novembro"
            };
                break;
            case decimal valor when valor >= 500:
                pagamentosFracionados = 3;
                listaCaptionsPrestacoes = new List<string>
            {
                "Maio", "Agosto", "Setembro"
            };
                break;
        }

        valorPrestacaoIMI = valorIMI / pagamentosFracionados;
        ResultVisibility = true;
        // HideResults = false;
    }

    protected void GoBack()
    {
        navigation?.NavigateTo("/");
    }

    protected class IMIResults
    {
        public decimal ValPatrimonio { get; set; }
        public string? Descricao { get; set; }
        public double Coeficiente { get; set; }
        public decimal ValorPagar { get; set; }
    }

}