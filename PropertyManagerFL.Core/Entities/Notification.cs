namespace PropertyManagerFL.Core.Entities;

/// <summary>
/// A notification addressed to an application user (IdentityUser.Id).
/// SeenAt tracks that the bell was opened; ReadAt tracks that the item itself was acted upon.
/// </summary>
public class Notification
{
    public int Id { get; set; }
    public string RecipientUserId { get; set; } = string.Empty;
    public int? TenantId { get; set; }
    public string? ActorUserId { get; set; }
    public string Kind { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? Link { get; set; }
    public string Severity { get; set; } = "Info";
    /// <summary>Recommended next step shown in the bell/inbox (free text produced by the rules engine).</summary>
    public string? SuggestedAction { get; set; }
    public string? ResourceType { get; set; }
    public int? ResourceId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SeenAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
