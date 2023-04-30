using System.Data;

namespace PropertyManagerFL.Application.Factories
{
    public interface IConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}