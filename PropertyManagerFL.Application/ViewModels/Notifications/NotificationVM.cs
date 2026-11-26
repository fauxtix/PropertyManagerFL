namespace PropertyManagerFL.Application.ViewModels.Notifications;

public class NotificationVM
{
    public int Id { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string Severity { get; set; } = "Info";
    /// <summary>Recommended next step, rendered as a separate line in the bell/inbox.</summary>
    public string? SuggestedAction { get; set; }
    public string? ResourceType { get; set; }
    public int? ResourceId { get; set; }
    public DateTime CreatedAt { get; set; }
    /// <summary>True once the user has marked the suggested action as done. Opening the item does not set this.</summary>
    public bool IsCompleted { get; set; }
}

/// <summary>Payload for completing or reopening a notification.</summary>
public class SetCompletedVM
{
    public bool Completed { get; set; }
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
    public string? SuggestedAction { get; set; }
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

    /// <summary>Recommended next step, persisted with the alert and shown in the inbox.</summary>
    public string? SuggestedAction { get; set; }

    /// <summary>Structured values so each UI surface can render its own localized message.</summary>
    public string? PrimaryText { get; set; }
    public string? SecondaryText { get; set; }
    public bool NeedsAction { get; set; }
    /// <summary>True when the lease has automatic renewal enabled (global + per-lease).</summary>
    public bool AutoRenewal { get; set; }
}
