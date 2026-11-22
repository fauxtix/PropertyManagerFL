using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Notifications;

namespace PropertyManagerFL.UI.Pages.Notifications;

public partial class Notifications : ComponentBase
{
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    private List<NotificationVM> _items = new();
    private string _userId = string.Empty;
    private int SelectedTab { get; set; }
    private IEnumerable<NotificationVM> VisibleItems => _items;
    private IEnumerable<NotificationVM> UnreadItems => _items.Where(i => !i.IsRead);

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

    private async Task MarkAllReadAsync()
    {
        var count = await NotificationService.MarkAllReadAsync(_userId);
        if (count > 0)
            foreach (var item in _items) item.IsRead = true;
    }

    private async Task OpenItemAsync(NotificationVM item)
    {
        if (!item.IsRead && await NotificationService.MarkReadAsync(item.Id, _userId))
            item.IsRead = true;
        if (!string.IsNullOrWhiteSpace(item.Link))
            Navigation.NavigateTo(item.Link);
    }

    private static IEnumerable<IGrouping<string, NotificationVM>> GroupItems(IEnumerable<NotificationVM> items)
        => items.GroupBy(g => GroupFor(g.CreatedAt));

    private static string GroupFor(DateTime createdAt)
    {
        var local = createdAt.ToLocalTime().Date;
        var today = DateTime.Today;
        if (local == today) return "Hoje";
        if (local == today.AddDays(-1)) return "Ontem";
        return local.ToString("MMMM yyyy");
    }

    private static string IconFor(string kind) => kind switch
    {
        "Appointment" => "fa-calendar",
        "RentLetter" or "RentUpdate" or "RentLetters" => "fa-file-text-o",
        "LeaseEnding" => "fa-exclamation-triangle",
        "Message" => "fa-envelope",
        "Payment" => "fa-credit-card",
        "Document" => "fa-file-o",
        _ => "fa-bell"
    };
}
