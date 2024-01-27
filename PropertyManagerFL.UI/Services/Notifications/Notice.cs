using static PropertyManagerFL.UI.Services.Notifications.Enums;

namespace PropertyManagerFL.UI.Services.Notifications;

public class Notice
{
    public string Message { get; set; }
    public string AlertCss { get; private set; }
    public string AlertShow { get; set; } = "show";
    public string IconCss { get; private set; }
    public DateTime RemoveAt { get; }
    public Notice(string message, BootstrapColor bootstrapColor, int removeAfterSeconds)
    {
        Message = message;
        RemoveAt = DateTime.Now.AddSeconds(removeAfterSeconds);
        setCss(bootstrapColor);
    }

    void setCss(BootstrapColor bootstrapColor)
    {
        //" alert-dismissible fade show";

        switch (bootstrapColor)
        {
            case BootstrapColor.Danger:
                AlertCss = "alert alert-danger fade";
                IconCss = "bi bi-exclamation-triangle";
                break;
            case BootstrapColor.Info:
                AlertCss = "alert alert-info fade";
                IconCss = "bi bi-info-circle";
                break;
            case BootstrapColor.Primary:
                AlertCss = "alert alert-primary fade";
                IconCss = "bi bi-info-circle";
                break;
            case BootstrapColor.Secondary:
                AlertCss = "alert alert-secondary fade";
                IconCss = "bi bi-info-circle";
                break;
            case BootstrapColor.Success:
                AlertCss = "alert alert-success fade";
                IconCss = "bi bi-check-circle";
                break;
            case BootstrapColor.Warning:
                AlertCss = "alert alert-warning fade";
                IconCss = "bi bi-exclamation-triangle";
                break;
        }
    }
}