using System.Data;

namespace PropertyManagerFL.Application.Interfaces.DapperContext
{
	public interface IDapperContext
	{
		public IDbConnection CreateConnection();
		public void Execute(Action<IDbConnection> @event);

	}
}
