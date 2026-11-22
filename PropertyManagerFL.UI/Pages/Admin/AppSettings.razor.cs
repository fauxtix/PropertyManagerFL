using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.Common;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Notifications;

namespace PropertyManagerFL.UI.Pages.Admin;
public partial class AppSettings
{
    [Inject] public IStringLocalizer<App>? L { get; set; }
    [Inject] public ILogger<App>? logger { get; set; }
    [Inject] public IAppSettingsService? AppSettingsService { get; set; }

    private ApplicationSettingsVM? settings;
    private string? culture;
    private string selectedCulture = Thread.CurrentThread.CurrentCulture.Name;

    protected SfToast? ToastObj { get; set; }
    protected string? ToastTitle;
    protected string? ToastMessage;
    protected string? ToastCss;
    protected string? ToastIcon;

    private bool InitializationMessageVisibility = false;
    private string InitializationMessage = string.Empty;

    private List<SettingsGroup> SettingsGroups { get; set; } = new();
    private string SelectedNode { get; set; } = "geral";
    private readonly HashSet<string> ExpandedGroups = new() { "configuracao" };

    private string SelectedLeafTitle =>
        SettingsGroups.SelectMany(group => group.Leaves)
                      .FirstOrDefault(leaf => leaf.Id == SelectedNode)?.ResxKey ?? string.Empty;

    private bool HasEditableSettings => SelectedNode is "geral" or "email";

    protected override async Task OnInitializedAsync()
    {
        ToastTitle = "";
        ToastMessage = "";
        ToastCss = "";
        ToastIcon = "";

        try
        {
            settings = await AppSettingsService!.GetSettingsAsync();
        }
        catch
        {
            settings = new();
        }

        if (!string.IsNullOrWhiteSpace(settings.DefaultLanguage))
        {
            selectedCulture = settings.DefaultLanguage;
        }

        SettingsGroups = new List<SettingsGroup>
        {
            new("configuracao", "TituloDefinicoes", "fa-cog", new List<SettingsLeaf>
            {
                new("geral", "Configuração geral", "fa-sliders"),
                new("proprietario", "TituloMenuProprietario", "fa-user"),
                new("modelos", "TituloModelosMenu", "fa-file-text-o")
            }),
            new("coeficientes", "Gestão de coeficientes", "fa-calculator", new List<SettingsLeaf>
            {
                new("coeficientes-rendas", "Coeficientes de atualização de rendas", "fa-line-chart"),
                new("coeficientes-imi", "Coeficientes de IMI", "fa-percent")
            }),
            new("manutencao", "lblManutencao", "fa-wrench", new List<SettingsLeaf>
            {
                new("tabelas-apoio", "menu-admin-lookup-tables", "fa-table"),
                new("inicializar-tabelas", "Inicializar tabelas", "fa-refresh")
            }),
            new("integracoes", "Integrações", "fa-plug", new List<SettingsLeaf>
            {
                new("email", "E-Mail", "fa-envelope")
            })
        };
    }

    private void ToggleGroup(string id)
    {
        if (!ExpandedGroups.Remove(id))
        {
            ExpandedGroups.Add(id);
        }
    }

    private void SelectNode(string id)
    {
        SelectedNode = id;
    }

    private async Task SaveCurrentSettings()
    {
        if (SelectedNode == "email")
        {
            await UpdateSettings();
        }
        else if (SelectedNode == "geral")
        {
            await UpdateOtherSettings();
        }
    }

    private async Task UpdateSettings()
    {
        ToastTitle = L["E-Mail"];

        try
        {
            await AppSettingsService.UpdateSettingsAsync(settings!);
            ToastCss = "e-toast-success";
            ToastMessage = $"{L["SuccessUpdate"]}";
            ToastIcon = "fas fa-check";
            await ShowToastMessage();
        }
        catch (Exception ex)
        {
            ToastCss = "e-toast-danger";
            ToastMessage = $"{L["MSG_ApiError"]}";
            ToastIcon = "fas fa-times";
            await ShowToastMessage();
            logger?.LogError(ex.Message);
        }
    }

    private async Task UpdateOtherSettings()
    {
        ToastTitle = L["TituloDefinicoes"];
        try
        {
            await AppSettingsService.UpdateOtherSettingsAsync(settings!);
            ToastCss = "e-toast-success";
            ToastMessage = $"{L["SuccessUpdate"]}";
            ToastIcon = "fas fa-check";
            await ShowToastMessage();
        }
        catch (Exception ex)
        {
            ToastCss = "e-toast-danger";
            ToastMessage = $"{L["MSG_ApiError"]}";
            ToastIcon = "fas fa-times";
            await ShowToastMessage();
            logger?.LogError(ex.Message);
        }
    }

    private void OnSelected(Syncfusion.Blazor.DropDowns.ChangeEventArgs<string, CountryModel> e)
    {
        if (e.Value == null)
        {
            return;
        }

        culture = e.Value.ToString();
        selectedCulture = culture;
        settings!.DefaultLanguage = culture;
    }

    private async Task InitializeTables()
    {
        var processed = await AppSettingsService.InitializeRentProcessingTables();
        InitializationMessage = processed ? L["Processo terminou com sucesso"] : L["Erro na inicialização. Verifique log, p.f."];
        InitializationMessageVisibility = true;
    }

    private async Task ShowToastMessage()
    {
        StateHasChanged();
        await Task.Delay(100);
        await ToastObj!.ShowAsync();
    }

    protected async Task HideToast()
    {
        await ToastObj!.HideAsync();
    }

    public List<CountryModel> CountryData = new List<CountryModel>
    {
        new CountryModel { ID = "pt-PT", Text = "TituloIdiomaPortugues", Pic = "portugal" },
        new CountryModel { ID = "es", Text = "TituloIdiomaEspanhol", Pic = "espanha" },
        new CountryModel { ID = "en-US", Text = "TituloIdiomaIngles", Pic = "american_flag" },
        new CountryModel { ID = "fr", Text = "TituloIdiomaFrances", Pic = "franca" }
    };

    public class CountryModel
    {
        public string ID { get; set; } = string.Empty;
        public string? Text { get; set; }
        public string? Pic { get; set; }
    }

    private sealed class SettingsGroup
    {
        public SettingsGroup(string id, string resxKey, string icon, List<SettingsLeaf> leaves)
        {
            Id = id;
            ResxKey = resxKey;
            Icon = icon;
            Leaves = leaves;
        }

        public string Id { get; }
        public string ResxKey { get; }
        public string Icon { get; }
        public List<SettingsLeaf> Leaves { get; }
    }

    private sealed class SettingsLeaf
    {
        public SettingsLeaf(string id, string resxKey, string icon)
        {
            Id = id;
            ResxKey = resxKey;
            Icon = icon;
        }

        public string Id { get; }
        public string ResxKey { get; }
        public string Icon { get; }
    }
}
