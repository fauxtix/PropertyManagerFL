namespace PropertyManagerFL.Application.ViewModels.Notifications;

public class NotificationVM
{
    public int Id { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string Severity { get; set; } = "Info";
    public string? ResourceType { get; set; }
    public int? ResourceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public class CreateNotificationVM
{
    public string? RecipientUserId { get; set; }
    public int? TenantId { get; set; }
    public string? ActorUserId { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string Severity { get; set; } = "Info";
    public string? ResourceType { get; set; }
    public int? ResourceId { get; set; }
}

/// <summary>
/// An operational alert produced by the shared rules engine (also rendered on Arrendamentos).
/// </summary>
public class NotificationAlertVM
{
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info";
    public string? Link { get; set; }
    public string? ResourceType { get; set; }
    public int? ResourceId { get; set; }

    /// <summary>Structured values so each UI surface can render its own localized message.</summary>
    public string? PrimaryText { get; set; }
    public string? SecondaryText { get; set; }
    public bool NeedsAction { get; set; }
}
