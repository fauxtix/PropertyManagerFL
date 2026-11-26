using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<int> InsertAsync(Notification notification);
    /// <summary>Identity users that can own an inbox; used to seed alerts at startup.</summary>
    Task<IReadOnlyList<string>> GetRecipientUserIdsAsync();
    Task<IEnumerable<Notification>> GetForUserAsync(string recipientUserId, int take);
    Task<NotificationCountVM> GetCountAsync(string recipientUserId);
    Task<bool> MarkReadAsync(int id, string recipientUserId);
    /// <summary>Completes or reopens a notification without changing stored procedures.</summary>
    Task<bool> SetCompletedAsync(int id, string recipientUserId, bool completed);
    Task<int> MarkAllReadAsync(string recipientUserId);
    Task<int> MarkSeenAsync(string recipientUserId);
    Task<bool> ExistsRecentAsync(string recipientUserId, string kind, string? resourceType, int? resourceId, DateTime since);
    Task<int> PurgeOldAsync(int retentionDays);
}

// Kept in the same namespace as the repository interfaces so API DI only needs one using.
public class NotificationCountVM
{
    /// <summary>Column name returned by usp_Notifications_GetUnreadCount (Dapper mapping).</summary>
    public int Unread { get; set; }
    /// <summary>Pending = not yet concluded. Kept separate from the DB column name.</summary>
    public int Pending { get => Unread; set => Unread = value; }
    public int Unseen { get; set; }
}
