using Microsoft.Extensions.Configuration;
using Microsoft.Win32.SafeHandles;
using PropertyManagerFLApplication.Configurations;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Runtime.InteropServices;

namespace CommonLayer.Factories
{
    public class ConnectionManager : IDisposable
    {
        // strings 'PROVIDER' são lidas a partir do ficheiro 'Config'
         const string SQL_SERVER_DB_PROVIDER = "SQLSERVER";
         const string MY_SQL_DB_PROVIDER = "MYSQL";
         const string ORACLE_DB_PROVIDER = "ORACLE";
         const string EXCESS_DB_PROVIDER = "MSACCESS";
         const string OLE_DB_PROVIDER = "OLEDB";
         const string ODBC_DB_PROVIDER = "ODBC";
         const string SQLITE_DB_PROVIDER = "SQLITE";

        private readonly IConfiguration _config;

        bool disposed = false;

        // Instancia uma instância 'SafeHandle'.
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public ConnectionManager(IConfiguration config)
        {
            _config= config;
        }
        /// <summary>
        /// Estabelece uma conexão com a base de dados e devolve uma conexão aberta
        /// </summary>
        /// <returns>Conexão à base de dados</returns>
        public static IDbConnection GetConnection()
        {
            IDbConnection connection = null;
            string connectionString = ApplicationConfiguration.ConnectionString;
            string provider = ApplicationConfiguration.DBProvider;
            
            switch (provider.Trim().ToUpper())
            {
                case SQL_SERVER_DB_PROVIDER:
                    connection = new SqlConnection(connectionString);
                    break;
                case MY_SQL_DB_PROVIDER:
                    //connection = new MySqlConnection(connectionString);
                    break;
                case ORACLE_DB_PROVIDER:
                    //    connection = new OracleConnection(connectionString);
                    break;
                case EXCESS_DB_PROVIDER:
                    connection = new OleDbConnection(connectionString);
                    break;
                case SQLITE_DB_PROVIDER:
                    connection = new SQLiteConnection(connectionString, true);
                    break;
                case ODBC_DB_PROVIDER:
                    connection = new OdbcConnection(connectionString);
                    break;
                case OLE_DB_PROVIDER:
                    connection = new OleDbConnection(connectionString);
                    break;
            }

            return connection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Implementação do padrão 'Dispose'.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // LIbertar aqui outros obetos 'managed'.
                //
            }

            disposed = true;
        }
    }
}
