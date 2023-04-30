using MudBlazor;
using PropertyManagerFL.Shared.Managers;
using System.Threading.Tasks;

namespace PropertyManagerFL.Client.Infrastructure.Managers.Preferences
{
    public interface IClientPreferenceManager : IPreferenceManager
    {
        Task<MudTheme> GetCurrentThemeAsync();

        Task<bool> ToggleDarkModeAsync();
    }
}