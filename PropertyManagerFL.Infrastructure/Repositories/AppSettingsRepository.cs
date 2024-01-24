using Dapper;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.ViewModels.AppSettings;
using PropertyManagerFL.Core.Entities;
using System.Data;

namespace PropertyManagerFL.Infrastructure.Repositories;
public class AppSettingsRepository : IAppSettingsRepository
{
    private readonly IDapperContext _context;

    public AppSettingsRepository(IDapperContext context)
    {
        _context = context;
    }

    public async Task<ApplicationSettings> GetSettingsAsync()
    {
        using (var connection = _context.CreateConnection())
        {
            string sql = "SELECT TOP 1 * FROM ApplicationSettings";
            var result = (await connection.QueryFirstOrDefaultAsync<ApplicationSettings>(sql));
            return result;
        }
    }

    public async Task UpdateSettingsAsync(ApplicationSettings settings)
    {

        var parameters = new DynamicParameters();

        parameters.Add("@DisplayName", settings?.DisplayName);
        parameters.Add("@Username", settings?.Username);
        parameters.Add("@Password", settings?.Password);
        parameters.Add("@Port", settings?.Port);
        parameters.Add("@Host", settings?.Host);

        parameters.Add("@FromEmail", settings?.FromEmail);
        parameters.Add("@SmtpServer", settings?.SmtpServer);
        parameters.Add("@EmailPort", settings?.EmailPort);
        parameters.Add("@EmailUsername", settings?.EmailUsername);
        parameters.Add("@EmailPassword", settings?.EmailPassword);

        parameters.Add("@HotMailHostname", settings?.HotmailHostname);
        parameters.Add("@HotmailPort", settings?.HotmailPort);
        parameters.Add("@UseSSL", settings?.UseSSL);
        parameters.Add("@HotmailUsername", settings?.HotmailUsername);
        parameters.Add("@HotmailPassword", settings?.HotmailPassword);

        parameters.Add("@PaperCutSmtpServer", settings?.PaperCutSmtpServer);
        parameters.Add("@PaperCutPort", settings?.PaperCutPort);
        parameters.Add("@EnableSsl", settings?.EnableSSL);

        parameters.Add("@ApiKey", settings?.ApiKey);

        using (var connection = _context.CreateConnection())
        {
            string sp_Name = "usp_AppSettings_Update";
            var result = (await connection.ExecuteAsync(sp_Name, param: parameters, commandType: CommandType.StoredProcedure));
            return;
        }

    }

    public async Task UpdateOtherSettingsAsync(ApplicationSettings settings)
    {

        var parameters = new DynamicParameters();

        parameters.Add("@PrazoContratoEmAnos", settings.PrazoContratoEmAnos);
        parameters.Add("@PrazoEnvioCartaAtraso", settings?.PrazoEnvioCartaAtraso);
        parameters.Add("@PrazoEnvioCartaAumento", settings?.PrazoEnvioCartaAumento);
        parameters.Add("@PrazoEnvioCartaRevogacao", settings?.PrazoEnvioCartaRevogacao);
        parameters.Add("@PrazoRespostaCartaAtraso", settings?.PrazoRespostaCartaAtraso);
        parameters.Add("@PrazoRespostaCartaAumento", settings?.PrazoRespostaCartaAumento);
        parameters.Add("@PrazoRespostaCartaRevogacao", settings?.PrazoRespostaCartaRevogacao);

        parameters.Add("@RenovacaoAutomatica", settings?.RenovacaoAutomatica);
        parameters.Add("@ComprovativoIRS", settings?.ComprovativoIRS);
        parameters.Add("@ComprovativoReciboVencimento", settings?.ComprovativoReciboVencimento);
        parameters.Add("@CaucaoRequerida", settings?.CaucaoRequerida);

        parameters.Add("@PrazoInspecaoGas", settings?.PrazoInspecaoGas);

        parameters.Add("@PercentagemMultaPorAtrasoPagamento", settings?.PercentagemMultaPorAtrasoPagamento);

        parameters.Add("@BackupBaseDados", settings?.BackupBaseDados);
        parameters.Add("@BackupOutrosFicheiros", settings?.BackupOutrosFicheiros);
        parameters.Add("@DefaultLanguage", settings?.DefaultLanguage);

        using (var connection = _context.CreateConnection())
        {
            string sp_Name = "usp_AppOtherSettings_Update";
            var result = (await connection.ExecuteAsync(sp_Name, param: parameters, commandType: CommandType.StoredProcedure));
            return;
        }

    }

}

