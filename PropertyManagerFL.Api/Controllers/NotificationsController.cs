using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PropertyManagerFL.Api.Hubs;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Notifications;

namespace PropertyManagerFL.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private const string UserIdHeader = "X-User-Id";

    private readonly INotificationService _service;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService service,
        IHubContext<NotificationHub> hub,
        ILogger<NotificationsController> logger)
    {
        _service = service;
        _hub = hub;
        _logger = logger;
    }

    private string GetUserId() => Request.Headers.TryGetValue(UserIdHeader, out var id)
        ? id.ToString()
        : string.Empty;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int take = 100)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("X-User-Id header is required.");
        return Ok(await _service.GetForUserAsync(userId, Math.Clamp(take, 1, 500)));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("X-User-Id header is required.");
        return Ok(await _service.GetCountAsync(userId));
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts() => Ok(await _service.GetAlertsAsync());

    [HttpPost("alerts/refresh")]
    public async Task<IActionResult> RefreshAlerts()
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("X-User-Id header is required.");
        var inserted = await _service.RefreshAlertsAsync(userId);
        if (inserted > 0) await NotifyClientAsync(userId);
        return Ok(inserted);
    }

    [HttpPost("alerts/refresh-all")]
    public async Task<IActionResult> RefreshAllAlerts() => Ok(await _service.SeedAlertsForAllUsersAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationVM notification)
    {
        var userId = GetUserId();
        if (!string.IsNullOrWhiteSpace(userId) && string.IsNullOrWhiteSpace(notification.RecipientUserId))
            notification.RecipientUserId = userId;

        var id = await _service.NotifyAsync(notification);
        if (id <= 0) return BadRequest("Notification could not be created.");

        await NotifyClientAsync(notification.RecipientUserId);
        return Ok(id);
    }

    [HttpPost("{id:int}/complete")]
    public async Task<IActionResult> SetCompleted(int id, [FromBody] SetCompletedVM? payload)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("X-User-Id header is required.");
        if (payload is null) return BadRequest("Completion state is required.");
        var ok = await _service.SetCompletedAsync(id, userId, payload.Completed);
        if (!ok) return StatusCode(StatusCodes.Status500InternalServerError);
        await NotifyClientAsync(userId);
        return NoContent();
    }

    [HttpPost("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("X-User-Id header is required.");
        var ok = await _service.MarkReadAsync(id, userId);
        if (!ok) return StatusCode(StatusCodes.Status500InternalServerError);
        await NotifyClientAsync(userId);
        return NoContent();
    }

    [HttpPost("complete-all")]
    public async Task<IActionResult> MarkAllComplete() => await MarkAllRead();

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("X-User-Id header is required.");
        var count = await _service.MarkAllReadAsync(userId);
        await NotifyClientAsync(userId);
        return Ok(count);
    }

    [HttpPost("seen")]
    public async Task<IActionResult> MarkSeen()
    {
        var userId = GetUserId();
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("X-User-Id header is required.");
        var count = await _service.MarkSeenAsync(userId);
        await NotifyClientAsync(userId);
        return Ok(count);
    }

    private async Task NotifyClientAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;
        try
        {
            await _hub.Clients.Group(NotificationHub.UserGroupPrefix + userId)
                .SendAsync("NotificationChanged", new { userId, at = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            // SignalR is an enhancement; failures must not break the REST operation.
            _logger.LogWarning(ex, "SignalR notification push failed");
        }
    }
}
