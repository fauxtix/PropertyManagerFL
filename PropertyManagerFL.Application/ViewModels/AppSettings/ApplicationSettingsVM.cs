namespace PropertyManagerFL.Application.ViewModels.AppSettings;

public class ApplicationSettingsVM
{
    public int Id { get; set; }
    public string? DisplayName { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int Port { get; set; }
    public string? Host { get; set; }

    public string? FromEmail { get; set; }
    public string? SmtpServer { get; set; }
    public int EmailPort { get; set; }
    public string? EmailUsername { get; set; }
    public string? EmailPassword { get; set; }

    public string? HotmailHostname { get; set; }
    public int HotmailPort { get; set; }
    public bool UseSSL { get; set; }
    public string? HotmailUsername { get; set; }
    public string? HotmailPassword { get; set; }

    public string? PaperCutSmtpServer { get; set; }
    public int PaperCutPort { get; set; }
    public bool EnableSSL { get; set; }

    public string? ApiKey { get; set; }

}
