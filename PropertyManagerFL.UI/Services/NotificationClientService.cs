using Microsoft.AspNetCore.SignalR.Client;
using PropertyManagerFL.UI.ApiWrappers;

namespace PropertyManagerFL.UI.Services;

/// <summary>
/// Thin wrapper around the API's SignalR notification hub. Listens only to the user's own group;
/// polling remains the reconciliation fallback.
/// </summary>
public class NotificationClientService : IAsyncDisposable
{
    private readonly ILogger<NotificationClientService> _logger;
    private readonly WrapperNotifications _wrapper;
    private readonly string _apiKey;
    private readonly string _hubUrl;

    private HubConnection? _connection;
    private string? _currentUserId;

    public event Func<Task>? NotificationChanged;

    public NotificationClientService(ILogger<NotificationClientService> logger,
        WrapperNotifications wrapper,
        IConfiguration configuration)
    {
        _logger = logger;
        _wrapper = wrapper;
        _apiKey = configuration["ApiKey"] ?? string.Empty;
        _hubUrl = wrapper.HubUrl;
    }

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public async Task StartAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;
        if (_connection is not null && _currentUserId == userId && IsConnected) return;

        await StopAsync();
        _currentUserId = userId;

        try
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options =>
                {
                    options.Headers.Add("ApiKey", _apiKey);
                    options.Headers.Add("X-User-Id", userId);
                })
                .WithAutomaticReconnect()
                .Build();

            _connection.On("NotificationChanged", async () =>
            {
                try
                {
                    if (NotificationChanged is not null)
                        await NotificationChanged.Invoke();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "NotificationChanged handler failed");
                }
            });

            await _connection.StartAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR connection could not be established; polling will be used");
        }
    }

    public async Task StopAsync()
    {
        if (_connection is null) return;
        try { await _connection.DisposeAsync(); }
        catch (Exception ex) { _logger.LogWarning(ex, "SignalR dispose failed"); }
        _connection = null;
    }

    public async ValueTask DisposeAsync() => await StopAsync();
}
