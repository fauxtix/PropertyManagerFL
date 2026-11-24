using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace PropertyManagerFL.UI.Components.Layout.SideBar;

/// <summary>
/// Builds the sidebar menu tree. Items and localisation keys are taken from the
/// previous Blazored menu in <c>Shared/NavMenuBlazored.razor</c>.
/// </summary>
public sealed class SidebarData
{
    private readonly IStringLocalizer<App> _localizer;
    private readonly IConfiguration _config;

    public SidebarData(IStringLocalizer<App> localizer, IConfiguration config)
    {
        _localizer = localizer;
        _config = config;
    }

    public List<MenuItem> GetGeneralMenuItems()
    {
        var items = new List<MenuItem>
        {
            new(href: "", title: T("Site_Menu_Inicio"), icon: "fa fa-home"),
            new(href: "imoveis", title: T("TituloMenuImoveis"), icon: "fa fa-building"),
            new(href: "inquilinos", title: T("TituloMenuInquilinos"), icon: "fa fa-users"),
            GetRentalsItem(),
            new(title: T("TituloDespesas"), icon: "fas fa-euro", childMenuItems: new List<MenuItem>
            {
                new(href: "/despesas/manutencao", title: T("lblManutencao"), icon: "fas fa-table-cells-large"),
                new(href: "/despesas/tipodespesas", title: T("TituloSubCategorias"), icon: "fas fa-cogs")
            }),
            new(title: T("TituloPagamentos"), icon: "fa fa-money-bill", childMenuItems: new List<MenuItem>
            {
                new(href: "/recebimentos/manutencao", title: T("lblManutencao"), icon: "fa fa-money-bill"),
                new(href: "/monthlyrentprocessing", title: T("TituloMenuProcessaRendas"), icon: "fa fa-money-bill")
            }),
            new(title: T("TituloCalculadoras"), icon: "fas fa-calculator", childMenuItems: new List<MenuItem>
            {
                new(href: "/CalculadoraIMI", title: "IMI", icon: "fas fa-calculator"),
                new(href: "/CalculadoraVPT", title: "VPT", icon: "fas fa-calculator")
            }),
            new(href: "Contactos", title: T("Site_Menu_Contactos"), icon: "fa fa-users"),
            new(href: "documentos", title: T("TituloDocumentacao"), icon: "fas fa-file-pdf"),
            new(href: "messages-modern", title: T("TituloMenuMensagens"), icon: "fas fa-paper-plane"),
            new(href: "scheduler", title: T("MenuAgenda"), icon: "fas fa-calendar")
        };

        return items;
    }

    public List<MenuItem> GetStandardMenuItems()
    {
        return new List<MenuItem>
        {
            new(href: "/admin/viewapplogs", title: T("MenuLogs"), icon: "fas fa-table"),
            new(title: T("lblManutencao"), icon: "fas fa-cog", childMenuItems: new List<MenuItem>
            {
                new(href: "/admin/appsettings", title: T("TituloDefinicoes"), icon: "fas fa-cog"),
                new(href: "/backupdatabase", title: T("mnuBackup"), icon: "fas fa-database"),
                new(href: "/backupuploads/1", title: T("MenuFiles"), icon: "fas fa-images")
            })
        };
    }

    public List<MenuItem> GetFooterMenuItems() => new();

    private MenuItem GetRentalsItem()
    {
        // The old menu only showed the submenu when the setting was missing;
        // with "true" it showed nothing. Keep both options visible.
        var setting = _config.GetSection("AppSettings:AutomaticRentAdjustment").Value;
        var automaticRentAdjustment = bool.TryParse(setting, out var parsed) && parsed;

        if (!automaticRentAdjustment)
        {
            return new(href: "arrendamentos", title: T("TituloMenuArrendamentos"), icon: "fa fa-file");
        }

        return new(title: T("TituloMenuArrendamentos"), icon: "fa fa-file-contract", childMenuItems: new List<MenuItem>
        {
            new(href: "arrendamentos", title: T("lblManutencao"), icon: "fa fa-file-contract"),
            new(href: "updaterentsletter", title: T("TituloMenuAtualizacaoRendas"), icon: "fa fa-file")
        });
    }

    private string T(string key) => _localizer[key];
}
