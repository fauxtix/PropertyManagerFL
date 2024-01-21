using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories;
public interface IAppSettingsRepository
{
    Task<ApplicationSettings> GetSettingsAsync();
    Task UpdateOtherSettingsAsync(ApplicationSettings settings);
    Task UpdateSettingsAsync(ApplicationSettings settings);
}