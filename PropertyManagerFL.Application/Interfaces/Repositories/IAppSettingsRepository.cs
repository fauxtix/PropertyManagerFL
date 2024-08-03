using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories;
public interface IAppSettingsRepository
{
    Task<ApplicationSettings> GetSettingsAsync();
    Task InitializeRentProcessingTables();
    Task UpdateOtherSettingsAsync(ApplicationSettings settings);
    Task UpdateSettingsAsync(ApplicationSettings settings);
    Task UpdateLanguageAsync(string language);

}