using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Notifications;
using PropertyManagerFL.UI.Services;
using Syncfusion.Blazor.Notifications;

namespace PropertyManagerFL.UI.Pages.Notifications;

public partial class NotificationBell : ComponentBase, IAsyncDisposable
{
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    private List<NotificationVM> _items = new();
    private string _userId = string.Empty;
    private bool ShowPanel { get; set; }
    private int SelectedTab { get; set; }
    private int UnseenCount { get; set; }
    private int UnreadCount => _items.Count(i => !i.IsRead);
    private IEnumerable<NotificationVM> VisibleItems => _items;
    private IEnumerable<NotificationVM> UnreadItems => _items.Where(i => !i.IsRead);
    private SfToast? ToastObj { get; set; }
    private string ToastTitle { get; set; } = string.Empty;
    private string ToastMessage { get; set; } = string.Empty;
    private CancellationTokenSource? _pollingCts;
    private bool _disposed;

    protected override async Task OnInitializedAsync()
    {
        _userId = await GetCurrentUserIdAsync();
        if (string.IsNullOrWhiteSpace(_userId)) return;

        // Materialise any new operational alerts into this user's inbox before loading it.
        try { await Notifications.RefreshAlertsAsync(_userId); }
        catch (Exception ex) { Logger.LogWarning(ex, "Failed to refresh operational alerts"); }

        await LoadAsync();

        NotificationClient.NotificationChanged += OnNotificationChanged;
        await NotificationClient.StartAsync(_userId);

        _pollingCts = new CancellationTokenSource();
        _ = PollLoopAsync(_pollingCts.Token);
    }

    private async Task<string> GetCurrentUserIdAsync()
    {
        if (AuthenticationStateTask is null) return string.Empty;
        var state = await AuthenticationStateTask;
        return state.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    private async Task LoadAsync()
    {
        if (string.IsNullOrWhiteSpace(_userId)) return;
        try
        {
            _items = (await Notifications.GetForUserAsync(_userId)).ToList();
            var count = await Notifications.GetCountAsync(_userId);
            UnseenCount = count.Unseen;
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to load notifications");
        }
    }

    private async Task PollLoopAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(60));
        try
        {
            while (!cancellationToken.IsCancellationRequested && await timer.WaitForNextTickAsync(cancellationToken))
            {
                // Always reconcile, even with SignalR connected, so a missed event self-heals.
                await InvokeAsync(async () => { await LoadAsync(); StateHasChanged(); });
            }
        }
        catch (OperationCanceledException) { }
    }

    private async Task OnNotificationChanged()
    {
        await InvokeAsync(async () => { await LoadAsync(); StateHasChanged(); });
    }

    private async Task TogglePanelAsync()
    {
        ShowPanel = !ShowPanel;
        if (ShowPanel)
        {
            await LoadAsync();
            var cleared = await Notifications.MarkSeenAsync(_userId);
            if (cleared > 0) UnseenCount = 0;
        }
    }

    private void ClosePanel() => ShowPanel = false;

    private async Task MarkAllReadAsync()
    {
        var count = await Notifications.MarkAllReadAsync(_userId);
        if (count > 0)
        {
            foreach (var item in _items) item.IsRead = true;
            ToastTitle = L["NotificationsTitle"];
            ToastMessage = L["NotificationsMarkedAllRead"];
            if (ToastObj is not null) await ToastObj.ShowAsync();
        }
    }

    private async Task OpenItemAsync(NotificationVM item)
    {
        if (!item.IsRead)
        {
            var ok = await Notifications.MarkReadAsync(item.Id, _userId);
            if (ok)
            {
                item.IsRead = true;
                if (UnseenCount > 0) UnseenCount--;
            }
        }
        ClosePanel();
        if (!string.IsNullOrWhiteSpace(item.Link)) Navigation.NavigateTo(item.Link);
    }

    private void HandleKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Escape") ClosePanel();
    }

    private static string IconFor(string kind) => kind switch
    {
        "Appointment" => "fa-calendar",
        "RentLetter" or "RentUpdate" or "RentLetters" => "fa-file-text-o",
        "LeaseEnding" => "fa-exclamation-triangle",
        "PaymentOverdue" => "fa-credit-card",
        "RentDueSoon" => "fa-clock-o",
        "LateLetterMissing" or "LetterResponseOverdue" => "fa-envelope-o",
        "ContractDocumentMissing" => "fa-file-o",
        "Message" => "fa-envelope",
        "Payment" => "fa-credit-card",
        "Document" => "fa-file-o",
        _ => "fa-bell"
    };

    private static string RelativeTime(DateTime createdAt)
    {
        var local = createdAt.ToLocalTime();
        var delta = DateTime.Now - local;
        if (delta.TotalMinutes < 1) return "agora";
        if (delta.TotalMinutes < 60) return $"{(int)delta.TotalMinutes} min";
        if (delta.TotalHours < 24) return $"{(int)delta.TotalHours} h";
        if (delta.TotalDays < 7) return $"{(int)delta.TotalDays} d";
        return local.ToString("dd/MM/yyyy");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        _pollingCts?.Cancel();
        _pollingCts?.Dispose();
        NotificationClient.NotificationChanged -= OnNotificationChanged;
        await NotificationClient.StopAsync();
    }
}
