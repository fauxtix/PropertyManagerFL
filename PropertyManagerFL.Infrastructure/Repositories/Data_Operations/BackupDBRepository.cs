using Dapper;
using Microsoft.Extensions.Logging;
using PropertyManagerFL.Application.Interfaces.Repositories.Data_Operations;
using PropertyManagerFL.Infrastructure.Context;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Repositories.Data_Operations
{
    public class BackupDBRepository : IBackupDBRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<BackupDBRepository> _logger;

        public BackupDBRepository(DapperContext context, ILogger<BackupDBRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> BackupDatabase()
        {
            string backupDIR = @"E:\BackupPropertyDB";
            if (!Directory.Exists(backupDIR))
            {
                Directory.CreateDirectory(backupDIR);
            }

            StringBuilder sb = new();
            sb.Append("backup database ");
            sb.Append($"PropertyManagerDB to disk='{backupDIR}");
            sb.Append($@"\{DateTime.Now:ddMMyyyy_HHmmss}.Bak'");

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    await connection.ExecuteAsync(sb.ToString());
                    return true;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
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
                _logger.LogError(exc.Message);
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
