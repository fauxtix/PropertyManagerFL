using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Services.Common;
public interface IAppSettingsService
{
    Task<ApplicationSettingsVM> GetSettingsAsync();
    Task UpdateSettingsAsync(ApplicationSettingsVM settings);
    Task UpdateOtherSettingsAsync(ApplicationSettingsVM settings);
}