namespace PropertyManagerFL.Application.Interfaces.Repositories.Data_Operations
{
    public interface IBackupDBRepository
    {
        Task<bool> BackupDatabase();
        Task SetupSqlServerTables();
    }
}