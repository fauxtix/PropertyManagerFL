using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.Notifications;

namespace PropertyManagerFL.Application.Interfaces.Services.AppManager;

/// <summary>
/// Application-level notification operations, shared by the API controller and SignalR publisher.
/// </summary>
public interface INotificationService
{
    Task<int> NotifyAsync(CreateNotificationVM notification);
    Task<IEnumerable<NotificationVM>> GetForUserAsync(string recipientUserId, int take = 100);
    Task<NotificationCountVM> GetCountAsync(string recipientUserId);
    Task<bool> MarkReadAsync(int id, string recipientUserId);
    Task<int> MarkAllReadAsync(string recipientUserId);
    Task<int> MarkSeenAsync(string recipientUserId);
    Task<IEnumerable<NotificationAlertVM>> GetAlertsAsync();
    /// <summary>Materialises any new operational alerts into the user's inbox. Returns the number inserted.</summary>
    Task<int> RefreshAlertsAsync(string recipientUserId);
}
