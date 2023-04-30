using PropertyManagerFL.Core.Entities;
using PropertyManagerFL.Application.ViewModels.Logins;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;
using PropertyManagerFL.Application.ViewModels.Logs;

namespace PropertyManagerFL.Application.Interfaces.Repositories
{
    public interface ILogRepository
    {
        Task DeleteAll();
        Task DeleteById(int id);
        Task<IEnumerable<AppLog>> GetAppLogs();
        Task<AppLog> GetAppLog_ById(int Id);
        Task<IEnumerable<LoginLogVM>> GetLogins();
        Task<LoginLogVM> GeUserNameAndEmail(string userId);
        void LogCRUD(LogCRUDModel model);
        void LogLogin(string userId, OpcaoCRUD Operacao, string sessionId);
        Task<IEnumerable<AppLog>> FilterLogins();
        Task DeleteByFilter(IEnumerable<AppLogDto> filteredLogs);
    }
}