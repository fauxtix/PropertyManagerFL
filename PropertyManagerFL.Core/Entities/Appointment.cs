namespace PropertyManagerFL.Core.Entities;
public class Appointment
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Location { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsAllDay { get; set; }
    public string? CategoryColor { get; set; } = string.Empty;
    public string? RecurrenceRule { get; set; } = string.Empty;
    public int? RecurrenceID { get; set; }
    public int? FollowingID { get; set; }
    public string? RecurrenceException { get; set; } = string.Empty;
    public string? StartTimezone { get; set; } = string.Empty;
    public string? EndTimezone { get; set; } = string.Empty;
    public int? ApptType { get; set; }
}
