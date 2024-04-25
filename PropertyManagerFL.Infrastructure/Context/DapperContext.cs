using Microsoft.Extensions.Configuration;
using PropertyManagerFL.Application.Interfaces.DapperContext;
using System.Data;
using System.Data.SqlClient;

namespace PropertyManagerFL.Infrastructure.Context
{
	public class DapperContext : IDapperContext
	{
		private readonly IConfiguration _configuration;
		private readonly string? _connectionString;
		public DapperContext(IConfiguration configuration)
		{

			_configuration = configuration;
			_connectionString = _configuration.GetConnectionString("PMConnection");
		}

		public void Execute(Action<IDbConnection> @event)
		{
			using (var connection = CreateConnection())
			{
				connection.Open();
				@event(connection);
			}
		}
		public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
	}
}
