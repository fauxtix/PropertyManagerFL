using PropertyManagerFL.Application;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Security;
using PropertyManagerFL.Application.ViewModels.Logins;
using PropertyManagerFL.Application.ViewModels.Logs;
using PropertyManagerFL.Application.ViewModels.Security.Models;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;

namespace PropertyManagerFL.Infrastructure.Services.SecurityServices
{
    public class LogService : ILogService
    {
        protected readonly ILogRepository _repoLog;
        protected readonly IUsersService _usersService;
        public LogService(ILogRepository repoLog, IUsersService usersService)
        {
            _repoLog = repoLog;
            _usersService = usersService;
        }

        public async Task<IEnumerable<LoginLogVM>> GeLogData()
        {
            var logs = await _repoLog.GetLogins();
            List<LoginLogVM> loginInfo = new List<LoginLogVM>();
            foreach (var logRecord in logs)
            {
                var userData = await GeUserNameAndEmail(logRecord.UserId);

                loginInfo.Add(new LoginLogVM()
                {
                    LoginDate = logRecord.LoginDate,
                    LogoutDate = logRecord.LogoutDate,
                    SessionId = logRecord.SessionId,
                    UserId = logRecord.UserId,
                    UserEmail = userData.UserEmail,
                    UserName = userData.UserName,
                    UserRole = _usersService.GetUserRoleName_ByEmail(userData.UserEmail)
                });
            }

            return loginInfo;
        }

        public async Task<IEnumerable<AppLogDto>> GetAppLogs()
        {
            return await _repoLog.GetAppLogs();.
        }

        public Task<IEnumerable<LoginLogVM>> GetLogData()
        {
            throw new NotImplementedException();
        }

        public string GetUserRoleName_ByEmail(string userEmail)
        {
            return _usersService.GetUserRoleName_ByEmail(userEmail);
        }

        public async Task<LoginLogVM> GeUserNameAndEmail(string userId)
        {
            return await _repoLog.GeUserNameAndEmail(userId);
        }

        public void LogCRUD(LogCRUDModel model)
        {
            _repoLog.LogCRUD(model);
        }

        public void LogLogin(string userId, OpcaoCRUD Operacao, string sessionId)
        {
            _repoLog.LogLogin(userId, Operacao, sessionId);
        }
    }
}
