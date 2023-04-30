using PropertyManagerFL.Application.Interfaces.Services.Security;
using PropertyManagerFL.Application.Security;

namespace PropertyManagerFL.Infrastructure.Services.SecurityServices
{
    public class DataSecurityService : IDataSecurityService
    {
        IDataSecurityRepository _repoDataSecurity;

        public DataSecurityService(IDataSecurityRepository repoDataSecurity)
        {
            _repoDataSecurity = repoDataSecurity;
        }

        public string Encrypt(string originalString)
        {
            return _repoDataSecurity.Encrypt(originalString);
        }

        public string Decrypt(string cryptedString, string sCampo, int Id)
        {
            return _repoDataSecurity.Decrypt(cryptedString, sCampo, Id);
        }

        public string Decrypt(string cryptedString)
        {
            return _repoDataSecurity.Decrypt(cryptedString);
        }
    }
}
