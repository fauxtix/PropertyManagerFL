using Microsoft.AspNetCore.Components;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Fracoes;

namespace PropertyManagerFL.UI.Pages.ComponentsBase
{
    public class AddEditFracaoBase : ComponentBase
    {
        [Inject] public IFracaoService? UnitsService { get; set; }
        public Fracao FullUnit { get; set; } = new();

        public async Task<FracaoVM> GetUnit(int id)
        {
            return await UnitsService!.GetFracao_ById(id!);
        }

    }
}
