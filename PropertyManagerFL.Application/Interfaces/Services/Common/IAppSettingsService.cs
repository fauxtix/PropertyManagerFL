using PropertyManagerFL.Application.ViewModels.AppSettings;

namespace PropertyManagerFL.Application.Interfaces.Services.Common;
public interface IAppSettingsService
{
    Task<ApplicationSettingsVM> GetSettingsAsync();
    Task UpdateSettingsAsync(ApplicationSettingsVM settings);
    Task UpdateOtherSettingsAsync(ApplicationSettingsVM settings);
    Task<(string, bool)> UpdateLanguageAsync(string language);
    Task<bool> InitializeRentProcessingTables();
}