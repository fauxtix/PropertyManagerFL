namespace PropertyManagerFL.Application.Interfaces.Services.Common
{
    public interface IBackupDatabaseService
    {
        Task<bool?> BackupDatabase();
    }
}