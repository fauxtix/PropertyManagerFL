using PropertyManagerFL.Application.Interfaces.Services.AppManager;

namespace PropertyManagerFL.Api.Configuration;

/// <summary>
/// Materialises the operational alerts for every registered user when the API starts.
/// A fresh Production/Dev database therefore has a populated inbox from minute zero,
/// and the operation is idempotent so restarts never duplicate alerts.
/// </summary>
public sealed class NotificationSeedingHostedService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NotificationSeedingHostedService> _logger;

    public NotificationSeedingHostedService(IServiceScopeFactory scopeFactory,
        ILogger<NotificationSeedingHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var inserted = await service.SeedAlertsForAllUsersAsync();
            _logger.LogInformation("Notification seeding complete: {Count} alert(s) inserted", inserted);
        }
        catch (Exception ex)
        {
            // Seeding must never prevent the API from starting.
            _logger.LogWarning(ex, "Notification seeding failed at startup");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
