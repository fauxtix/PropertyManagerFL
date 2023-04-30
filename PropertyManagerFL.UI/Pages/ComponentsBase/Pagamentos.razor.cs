using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class Pagamentos : ComponentBase
    {
        [CascadingParameter]
        protected Task<AuthenticationState>? authenticationStateTask { get; set; }
        [Inject] protected IStringLocalizer<App>? L { get; set; }
        [Inject] protected IDespesaService? expensesService { get; set; }
        [Inject] protected IConfiguration? config { get; set; }
        [Inject] protected HttpClient? _httpClient { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }
        //[Inject] protected UserManager<ApplicationUser> _UserManager { get; set; }


    }
}
