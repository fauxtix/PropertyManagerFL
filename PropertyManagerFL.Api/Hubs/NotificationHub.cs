using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PropertyManagerFL.Api.Hubs;

/// <summary>
/// Per-user notification hub. Clients are grouped by user id (sent in the X-User-Id header
/// by the Blazor server client); no broadcast to all users.
/// </summary>
public class NotificationHub : Hub
{
    public const string UserGroupPrefix = "user:";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Headers["X-User-Id"].ToString();
        if (!string.IsNullOrWhiteSpace(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroupPrefix + userId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.GetHttpContext()?.Request.Headers["X-User-Id"].ToString();
        if (!string.IsNullOrWhiteSpace(userId))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, UserGroupPrefix + userId);

        await base.OnDisconnectedAsync(exception);
    }
}
