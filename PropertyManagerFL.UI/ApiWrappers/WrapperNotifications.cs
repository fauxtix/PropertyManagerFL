using System.Net.Http.Json;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Notifications;
using PropertyManagerFL.UI.Services.ClientApi;

namespace PropertyManagerFL.UI.ApiWrappers;

/// <summary>
/// UI-side access to the notification API. The recipient user id is passed explicitly by the
/// calling component (from AuthenticationState) and sent as X-User-Id on every request.
/// </summary>
public class WrapperNotifications : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<WrapperNotifications> _logger;
    private readonly HttpClient _httpClient;

    private readonly string _notificationsUri;
    private readonly string _hubUrl;

    public WrapperNotifications(IConfiguration configuration,
        ILogger<WrapperNotifications> logger,
        HttpClient httpClient,
        HttpClientConfigurationService httpClientConfigService)
    {
        _configuration = configuration;
        _logger = logger;
        _httpClient = httpClient;
        _notificationsUri = $"{_configuration["BaseUrl"]}/Notifications";
        _hubUrl = _configuration["BaseUrl"]!.Replace("/api", "") + "/hubs/notifications";

        httpClientConfigService.ConfigureHttpClient(httpClient);
    }

    public string HubUrl => _hubUrl;

    private HttpRequestMessage CreateRequest(HttpMethod method, string path, string userId, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, $"{_notificationsUri}{path}");
        request.Headers.Add("X-User-Id", userId);
        if (content is not null) request.Content = content;
        return request;
    }

    public async Task<IEnumerable<NotificationVM>> GetForUserAsync(string recipientUserId, int take = 100)
    {
        try
        {
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Get, $"?take={take}", recipientUserId));
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<NotificationVM>();
            return await response.Content.ReadFromJsonAsync<List<NotificationVM>>() ?? new List<NotificationVM>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao pesquisar notificações");
            return Enumerable.Empty<NotificationVM>();
        }
    }

    public async Task<NotificationCountVM> GetCountAsync(string recipientUserId)
    {
        try
        {
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Get, "/unread-count", recipientUserId));
            if (!response.IsSuccessStatusCode) return new NotificationCountVM();
            return await response.Content.ReadFromJsonAsync<NotificationCountVM>() ?? new NotificationCountVM();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter contagem de notificações");
            return new NotificationCountVM();
        }
    }

    public async Task<int> NotifyAsync(CreateNotificationVM notification)
    {
        try
        {
            var userId = notification.RecipientUserId ?? string.Empty;
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Post, "", userId, JsonContent.Create(notification)));
            if (!response.IsSuccessStatusCode) return -1;
            return await response.Content.ReadFromJsonAsync<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar notificação");
            return -1;
        }
    }

    public async Task<bool> MarkReadAsync(int id, string recipientUserId)
    {
        try
        {
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Post, $"/{id}/read", recipientUserId));
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar notificação como lida");
            return false;
        }
    }

    public async Task<bool> SetCompletedAsync(int id, string recipientUserId, bool completed)
    {
        try
        {
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Post, $"/{id}/complete", recipientUserId,
                    JsonContent.Create(new SetCompletedVM { Completed = completed })));
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar estado da notificação");
            return false;
        }
    }

    public async Task<int> MarkAllCompleteAsync(string recipientUserId) => await MarkAllReadAsync(recipientUserId);

    public async Task<int> MarkAllReadAsync(string recipientUserId)
    {
        try
        {
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Post, "/read-all", recipientUserId));
            if (!response.IsSuccessStatusCode) return 0;
            return await response.Content.ReadFromJsonAsync<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar notificações como lidas");
            return 0;
        }
    }

    public async Task<int> MarkSeenAsync(string recipientUserId)
    {
        try
        {
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Post, "/seen", recipientUserId));
            if (!response.IsSuccessStatusCode) return 0;
            return await response.Content.ReadFromJsonAsync<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar notificações como vistas");
            return 0;
        }
    }

    public async Task<int> RefreshAlertsAsync(string recipientUserId)
    {
        try
        {
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Post, "/alerts/refresh", recipientUserId));
            if (!response.IsSuccessStatusCode) return 0;
            return await response.Content.ReadFromJsonAsync<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar alertas operacionais");
            return 0;
        }
    }

    public async Task<int> SeedAlertsForAllUsersAsync()
    {
        try
        {
            using var response = await _httpClient.SendAsync(
                CreateRequest(HttpMethod.Post, "/alerts/refresh-all", string.Empty));
            if (!response.IsSuccessStatusCode) return 0;
            return await response.Content.ReadFromJsonAsync<int>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar alertas iniciais");
            return 0;
        }
    }

    public async Task<IEnumerable<NotificationAlertVM>> GetAlertsAsync()
    {
        try
        {
            using var response = await _httpClient.GetAsync($"{_notificationsUri}/alerts");
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<NotificationAlertVM>();
            return await response.Content.ReadFromJsonAsync<List<NotificationAlertVM>>() ?? new List<NotificationAlertVM>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter alertas operacionais");
            return Enumerable.Empty<NotificationAlertVM>();
        }
    }
}
