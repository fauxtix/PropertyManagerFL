namespace PropertyManagerFL.Core.Entities;

public class ApplicationSettings
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
    public string? DefaultLanguage { get; set; }

    public byte PrazoContratoEmAnos { get; set; }
    public byte PrazoEnvioCartaAtraso { get; set; }
    public byte PrazoEnvioCartaAumento { get; set; }
    public bool CartasAumentoAutomaticas { get; set; }
    public byte PrazoEnvioCartaRevogacao { get; set; }
    public byte PrazoRespostaCartaAtraso { get; set; }
    public byte PrazoRespostaCartaAumento { get; set; }
    public byte PrazoRespostaCartaRevogacao { get; set; }
    public bool RenovacaoAutomatica { get; set; }
    public bool ComprovativoIRS { get; set; }
    public bool ComprovativoReciboVencimento { get; set; }
    public bool CaucaoRequerida { get; set; }

    public byte PrazoInspecaoGas { get; set; }
    public byte PercentagemMultaPorAtrasoPagamento { get; set; }
    public string? BackupBaseDados { get; set; }
    public string? BackupOutrosFicheiros { get; set; }
    public decimal TaxaIRS { get; set; }
}
