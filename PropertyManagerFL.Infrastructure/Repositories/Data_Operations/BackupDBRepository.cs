using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories;
using PropertyManagerFL.Application.Interfaces.Repositories.Data_Operations;
using PropertyManagerFL.Infrastructure.Context;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories.Data_Operations
{
    public class BackupDBRepository : IBackupDBRepository
    {
        private readonly DapperContext _context;
        private readonly IAppSettingsRepository _settings_Repo;
        private readonly ILogger<BackupDBRepository> _logger;

        public BackupDBRepository(DapperContext context, ILogger<BackupDBRepository> logger, IAppSettingsRepository settings_Repo)
        {
            _context = context;
            _logger = logger;
            _settings_Repo = settings_Repo;
        }

        public async Task<bool> BackupDatabase()
        {
            try
            {
                var settings = await _settings_Repo.GetSettingsAsync();
                string backupDIR = settings.BackupBaseDados;
                if (string.IsNullOrWhiteSpace(backupDIR))
                {
                    throw new InvalidOperationException("O caminho de backup não está configurado (ApplicationSettings.BackupBaseDados).");
                }

                if (!Directory.Exists(backupDIR))
                {
                    Directory.CreateDirectory(backupDIR);
                }

                var backupPath = Path.Combine(backupDIR, $"{DateTime.Now:ddMMyyyy_HHmmss}.Bak");
                StringBuilder sb = new();
                sb.Append("backup database ");
                sb.Append($"PropertyManagerDB to disk='{backupPath}'");

                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(sb.ToString());
                    _logger.LogWarning("Backup efetuado ({Time})", DateTime.Now);

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao efetuar backup da base de dados");
                throw;
            }
        }

        public async Task SetupSqlServerTables()
        {
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync("usp_Development_initializeWorkTables", commandType: System.Data.CommandType.StoredProcedure);
                }


                // Valores 'default' para tabelas auxiliares (livros/ebooks)
                //InicializaTabela("LocalArquivo", "Descricao", "Desconhecido");
                //InicializaTabela("Autor", "Descricao", "Desconhecido");
                //InicializaTabela("Categoria", "Descricao", "Desconhecida");
                //InicializaTabela("Editora", "Descricao", "Desconhecida");
                //InicializaTabela("Formato", "Descricao", "Desconhecido");
                //InicializaTabela("Formato_Media", "Descricao", "Desconhecido");
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Operation failed");
                throw new ApplicationException(exc.Message);
            }
        }


        private void InicializaTabela(string sTabela, string sCampo, string InitialValue)
        {
            try
            {
                string sQuery = $"INSERT INTO {sTabela} ({sCampo}) VALUES ('{InitialValue}')";
                using (var connection = _context.CreateConnection())
                {
                    connection.Execute(sQuery);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
