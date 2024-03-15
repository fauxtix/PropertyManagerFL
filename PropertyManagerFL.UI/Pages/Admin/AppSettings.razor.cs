using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.Notifications;

namespace PropertyManagerFL.UI.Pages.Admin;
public partial class AppSettings
{

    [Inject] public IStringLocalizer<App>? L { get; set; }
    [Inject] public ILogger<App>? logger { get; set; }

    private ApplicationSettingsVM? settings;
    private SfButton? ToggleBtnObj;
    private string IconCss = "fa fa-play";
    private string Content = "Ssl";

    private string? culture;
    private string selectedCulture = Thread.CurrentThread.CurrentCulture.Name;

    private string portugues;
    protected SfToast? ToastObj { get; set; }

    protected string? ToastTitle;
    protected string? ToastMessage;
    protected string? ToastCss;
    protected string? ToastIcon;

    private bool InitializationMessageVisibility = false;
    private string InitializationMessage = string.Empty;


    protected override async Task OnInitializedAsync()
    {
        portugues = L["TituloIdiomaPortugues"]; // TODO set languages
        ToastTitle = "";
        ToastMessage = "";
        ToastCss = "";
        ToastIcon = "";


        try
        {
            settings = await AppSettingsService.GetSettingsAsync();
        }
        catch
        {
            settings = new();
        }
    }

    public void OnToggleClick()
    {
        if (ToggleBtnObj?.Content == "Ssl")
        {
            settings!.EnableSSL = true; // Hotmail
            Content = "No Ssl";
            IconCss = "fa fa-pause";
        }
        else
        {
            settings!.EnableSSL = false;

            Content = "Ssl";
            IconCss = "fa fa-play";
        }
    }

    private async Task UpdateSettings()
    {
        ToastTitle = "Update email settings";

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
        ToastTitle = "Update other settings";
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
            return;

        culture = e.Value.ToString();
        selectedCulture = culture;

        settings!.DefaultLanguage = culture;

    }

    private async void InitializeTables()
    {
        var processed = await AppSettingsService.InitializeRentProcessingTables();
        InitializationMessage = processed ? "Processo terminou com sucesso" : "Erro na inicialização. Verifique log, p.f.";
        InitializationMessageVisibility = true;
        StateHasChanged();
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
        new CountryModel { ID ="pt-PT", Text = "TituloIdiomaPortugues", Pic = "portugal" },
        new CountryModel { ID = "es", Text = "TituloIdiomaEspanhol", Pic = "espanha", },
        new CountryModel { ID ="en-US", Text = "TituloIdiomaIngles", Pic = "american_flag",},
        new CountryModel { ID ="fr", Text = "TituloIdiomaFrances", Pic = "franca" }
};

    public class CountryModel
    {
        public string ID { get; set; } = string.Empty;
        public string? Text { get; set; }
        public string? Pic { get; set; }
    }


}