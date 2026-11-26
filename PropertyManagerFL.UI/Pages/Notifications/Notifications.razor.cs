using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Notifications;

namespace PropertyManagerFL.UI.Pages.Notifications;

public partial class Notifications : ComponentBase
{
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private List<NotificationVM> _items = new();
    private string _userId = string.Empty;
    private int SelectedTab { get; set; }
    private bool ConfirmAllDialogVisible { get; set; }
    private IEnumerable<NotificationVM> VisibleItems => _items;
    private IEnumerable<NotificationVM> PendingItems => _items.Where(i => !i.IsCompleted);

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask is null) return;
        var state = await AuthenticationStateTask;
        _userId = state.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_userId)) return;
        try { await NotificationService.RefreshAlertsAsync(_userId); }
        catch (Exception ex) { Logger.LogWarning(ex, "Failed to refresh operational alerts"); }
        _items = (await NotificationService.GetForUserAsync(_userId, take: 200)).ToList();
    }

    private async Task ToggleCompleteAsync(NotificationVM item)
    {
        var completed = !item.IsCompleted;
        if (!await NotificationService.SetCompletedAsync(item.Id, _userId, completed)) return;
        item.IsCompleted = completed;
        _items = (await NotificationService.GetForUserAsync(_userId, take: 200)).ToList();
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
        var count = await NotificationService.MarkAllCompleteAsync(_userId);
        if (count > 0)
            foreach (var item in _items) item.IsCompleted = true;
    }

    private async Task ClosePageAsync()
    {
        // history.back() is only useful when the list was opened from another page;
        // opening /notifications directly must still have a working exit.
        var hasHistory = await JS.InvokeAsync<bool>("eval", "window.history.length > 1");
        if (hasHistory) await JS.InvokeVoidAsync("history.back");
        else Navigation.NavigateTo("/");
    }

    private void OpenItemAsync(NotificationVM item)
    {
        if (!string.IsNullOrWhiteSpace(item.Link))
            Navigation.NavigateTo(item.Link);
    }

    private IEnumerable<IGrouping<string, NotificationVM>> GroupItems(IEnumerable<NotificationVM> items)
        => items.GroupBy(g => GroupFor(g.CreatedAt));

    private string GroupFor(DateTime createdAt) => Formatter.GroupFor(createdAt);

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
}
