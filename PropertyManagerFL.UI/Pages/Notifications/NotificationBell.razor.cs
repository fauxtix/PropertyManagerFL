using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Routing;
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
    private int BadgeCount { get; set; }
    private int PendingCount => _items.Count(i => !i.IsCompleted);
    private IEnumerable<NotificationVM> VisibleItems => _items;
    private IEnumerable<NotificationVM> PendingItems => _items.Where(i => !i.IsCompleted);
    private SfToast? ToastObj { get; set; }
    private bool ConfirmAllDialogVisible { get; set; }
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
        Navigation.LocationChanged += OnLocationChanged;
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
            BadgeCount = count.Pending;
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
        }
    }

    private void ClosePanel() => ShowPanel = false;

    private void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        if (!ShowPanel) return;
        ShowPanel = false;
        _ = InvokeAsync(StateHasChanged);
    }

    private Task OpenFullPageAsync()
    {
        ClosePanel();
        Navigation.NavigateTo("/notifications");
        return Task.CompletedTask;
    }

    private async Task ToggleCompleteAsync(NotificationVM item)
    {
        var completed = !item.IsCompleted;
        if (!await Notifications.SetCompletedAsync(item.Id, _userId, completed)) return;

        item.IsCompleted = completed;
        ToastTitle = L["NotificationsTitle"];
        ToastMessage = completed ? L["NotificationsMarkedComplete"] : L["NotificationsReopened"];
        if (ToastObj is not null) await ToastObj.ShowAsync();
        await LoadAsync();
    }

    private void OpenMarkAllDialog() => ConfirmAllDialogVisible = true;

    private void CancelMarkAll() => ConfirmAllDialogVisible = false;

    private async Task ConfirmMarkAllAsync()
    {
        ConfirmAllDialogVisible = false;
        await MarkAllCompleteAsync();
    }

    private async Task MarkAllCompleteAsync()
    {
        var count = await Notifications.MarkAllCompleteAsync(_userId);
        if (count > 0)
        {
            foreach (var item in _items) item.IsCompleted = true;
            ToastTitle = L["NotificationsTitle"];
            ToastMessage = L["NotificationsMarkedAllComplete"];
            if (ToastObj is not null) await ToastObj.ShowAsync();
        }
        await LoadAsync();
    }

    private async Task OpenItemAsync(NotificationVM item)
    {
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
        "RentLetter" or "RentUpdate" or "RentLetters" => "fa-file-lines",
        "LeaseEnding" => "fa-triangle-exclamation",
        "PaymentOverdue" => "fa-credit-card",
        "RentDueSoon" => "fa-clock",
        "LateLetterMissing" or "LetterResponseOverdue" => "fa-envelope",
        "ContractDocumentMissing" => "fa-file",
        "Message" => "fa-envelope",
        "Payment" => "fa-credit-card",
        "Document" => "fa-file",
        _ => "fa-bell"
    };

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        _pollingCts?.Cancel();
        _pollingCts?.Dispose();
        NotificationClient.NotificationChanged -= OnNotificationChanged;
        Navigation.LocationChanged -= OnLocationChanged;
        await NotificationClient.StopAsync();
    }
}
