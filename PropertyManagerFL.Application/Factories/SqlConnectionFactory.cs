using System.Data;
using System.Data.SqlClient;

namespace PropertyManagerFL.Application.Factories
{
    public class SqlConnectionFactory : IConnectionFactory
    {
        readonly string _connectionString;
        public SqlConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
