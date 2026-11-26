using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Core.Entities;
using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IDapperContext _context;
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(IDapperContext context, ILogger<NotificationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> InsertAsync(Notification notification)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RecipientUserId", notification.RecipientUserId);
            parameters.Add("@TenantId", notification.TenantId);
            parameters.Add("@ActorUserId", notification.ActorUserId);
            parameters.Add("@Kind", notification.Kind);
            parameters.Add("@Title", notification.Title);
            parameters.Add("@Body", notification.Body);
            parameters.Add("@Link", notification.Link);
            parameters.Add("@Severity", notification.Severity);
            parameters.Add("@SuggestedAction", notification.SuggestedAction);
            parameters.Add("@ResourceType", notification.ResourceType);
            parameters.Add("@ResourceId", notification.ResourceId);
            parameters.Add("@CreatedAt", notification.CreatedAt);
            return await connection.QueryFirstAsync<int>("usp_Notifications_Insert",
                param: parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return -1;
        }
    }

    public async Task<IReadOnlyList<string>> GetRecipientUserIdsAsync()
    {
        try
        {
            using var connection = _context.CreateConnection();
            var ids = await connection.QueryAsync<string>(
                "SELECT [Id] FROM [dbo].[AspNetUsers] ORDER BY [Id]");
            return ids.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return Array.Empty<string>();
        }
    }

    public async Task<IEnumerable<Notification>> GetForUserAsync(string recipientUserId, int take)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RecipientUserId", recipientUserId);
            parameters.Add("@Take", take);
            return await connection.QueryAsync<Notification>("usp_Notifications_GetForUser",
                param: parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return Enumerable.Empty<Notification>();
        }
    }

    public async Task<NotificationCountVM> GetCountAsync(string recipientUserId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RecipientUserId", recipientUserId);
            var result = await connection.QueryFirstOrDefaultAsync<NotificationCountVM>("usp_Notifications_GetUnreadCount",
                param: parameters, commandType: CommandType.StoredProcedure);
            return result ?? new NotificationCountVM();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return new NotificationCountVM();
        }
    }

    public async Task<bool> MarkReadAsync(int id, string recipientUserId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@RecipientUserId", recipientUserId);
            await connection.ExecuteAsync("usp_Notifications_MarkRead",
                param: parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return false;
        }
    }

    public async Task<bool> SetCompletedAsync(int id, string recipientUserId, bool completed)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@RecipientUserId", recipientUserId);
            // Completion/reopen is done here (rather than in a new stored procedure) so existing
            // databases need no script upgrade and the generated SQL snapshot stays valid.
            var sql = completed
                ? "UPDATE [dbo].[Notifications] SET [ReadAt] = COALESCE([ReadAt], SYSDATETIME()) WHERE [Id] = @Id AND [RecipientUserId] = @RecipientUserId AND [ReadAt] IS NULL"
                : "UPDATE [dbo].[Notifications] SET [ReadAt] = NULL WHERE [Id] = @Id AND [RecipientUserId] = @RecipientUserId AND [ReadAt] IS NOT NULL";
            var affected = await connection.ExecuteAsync(sql, param: parameters);
            return affected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return false;
        }
    }

    public async Task<int> MarkAllReadAsync(string recipientUserId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RecipientUserId", recipientUserId);
            // The procedure SELECTs @@ROWCOUNT; ExecuteAsync would return -1 because of SET NOCOUNT ON.
            return await connection.ExecuteScalarAsync<int>("usp_Notifications_MarkAllRead",
                param: parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return 0;
        }
    }

    public async Task<int> MarkSeenAsync(string recipientUserId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RecipientUserId", recipientUserId);
            return await connection.ExecuteAsync("usp_Notifications_MarkSeen",
                param: parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return 0;
        }
    }

    public async Task<bool> ExistsRecentAsync(string recipientUserId, string kind, string? resourceType, int? resourceId, DateTime since)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RecipientUserId", recipientUserId);
            parameters.Add("@Kind", kind);
            parameters.Add("@ResourceType", resourceType);
            parameters.Add("@ResourceId", resourceId);
            parameters.Add("@Since", since);
            return await connection.QueryFirstAsync<bool>("usp_Notifications_ExistsRecent",
                param: parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return false;
        }
    }

    public async Task<int> PurgeOldAsync(int retentionDays)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@RetentionDays", retentionDays);
            return await connection.ExecuteAsync("usp_Notifications_PurgeOld",
                param: parameters, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed");
            return 0;
        }
    }
}
