using PropertyManagerFL.Core.Entities;

namespace PropertyManagerFL.Application.Interfaces.Repositories;

public interface INotificationRepository
{
    Task<int> InsertAsync(Notification notification);
    Task<IEnumerable<Notification>> GetForUserAsync(string recipientUserId, int take);
    Task<NotificationCountVM> GetCountAsync(string recipientUserId);
    Task<bool> MarkReadAsync(int id, string recipientUserId);
    Task<int> MarkAllReadAsync(string recipientUserId);
    Task<int> MarkSeenAsync(string recipientUserId);
    Task<bool> ExistsRecentAsync(string recipientUserId, string kind, string? resourceType, int? resourceId, DateTime since);
    Task<int> PurgeOldAsync(int retentionDays);
}

// Kept in the same namespace as the repository interfaces so API DI only needs one using.
public class NotificationCountVM
{
    public int Unread { get; set; }
    public int Unseen { get; set; }
}
