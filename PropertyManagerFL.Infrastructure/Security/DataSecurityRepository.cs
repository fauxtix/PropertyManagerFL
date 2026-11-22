using Microsoft.AspNetCore.DataProtection;
using PropertyManagerFL.Application.Security;
using System.Security.Cryptography;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Security
{
    /// <summary>
    /// Encrypts sensitive strings with ASP.NET Core DataProtection.
    /// DES remains only as a decryption fallback for values already stored in the database.
    /// </summary>
    public class DataSecurityRepository : IDataSecurityRepository
    {
        private const string Purpose = "PropertyManagerFL.DataSecurity.v1";
        private static readonly byte[] LegacyKey = ASCIIEncoding.ASCII.GetBytes("ZeroCool");

        private readonly IDataProtector _protector;

        public DataSecurityRepository(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(Purpose);
        }

        // Kept for any legacy code that constructs this class directly.
        public DataSecurityRepository()
            : this(DataProtectionProvider.Create("PropertyManagerFL"))
        {
        }

        public string Encrypt(string originalString)
        {
            if (string.IsNullOrEmpty(originalString))
            {
                return string.Empty;
            }

            return _protector.Protect(originalString);
        }

        public string Decrypt(string cryptedString, string sCampo, int id)
        {
            return Decrypt(cryptedString);
        }

        public string Decrypt(string cryptedString)
        {
            if (string.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException(nameof(cryptedString), "O texto a ser desencriptado não pode estar vazio.");
            }

            try
            {
                return _protector.Unprotect(cryptedString);
            }
            catch (CryptographicException)
            {
                return DecryptLegacy(cryptedString);
            }
        }

        private static string DecryptLegacy(string cryptedString)
        {
            try
            {
                using var cryptoProvider = new DESCryptoServiceProvider();
                using var memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
                using var cryptoStream = new CryptoStream(memoryStream,
                    cryptoProvider.CreateDecryptor(LegacyKey, LegacyKey), CryptoStreamMode.Read);
                using var reader = new StreamReader(cryptoStream);
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                throw new ApplicationException("Invalid encrypted value.");
            }
        }
    }
}
