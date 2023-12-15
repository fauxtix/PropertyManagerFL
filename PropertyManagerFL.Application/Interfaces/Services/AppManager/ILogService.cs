using PropertyManagerFL.Application.ViewModels.Logs;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager
{
    public interface ILogService
    {
        Task<IEnumerable<AppLogDto>> GetAppLogs();
        Task<AppLogDto> GetLog_ById(int id);
        Task<IEnumerable<AppLogDto>> ViewLogins();
        Task DeleteAll();
        Task DeleteById(int Id);
        Task DeleteFilteredRecords(IEnumerable<AppLogDto> FilteredRecords);
    }
}
