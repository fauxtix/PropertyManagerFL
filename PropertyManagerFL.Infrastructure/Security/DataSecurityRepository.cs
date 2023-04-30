using PropertyManagerFL.Application.Security;
using System.Security.Cryptography;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Security
{
    /// <summary>
    /// Class to be used for providing security to sensitive data by Encryption/Decryption
    /// </summary>
    public class DataSecurityRepository : IDataSecurityRepository
    {
        public DataSecurityRepository()
        { }

        static byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroCool");

        /// <summary>
        /// Encripta string normal.
        /// </summary>
        /// <param name="originalString">String a encriptar.</param>
        /// <returns>string encripta</returns>
        public string Encrypt(string originalString)
		{
			if (String.IsNullOrEmpty(originalString))
				return string.Empty;

			using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
			{
				MemoryStream memoryStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(memoryStream,
					cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);
				StreamWriter writer = new StreamWriter(cryptoStream);
				writer.Write(originalString);
				writer.Flush();
				cryptoStream.FlushFinalBlock();
				writer.Flush();
				return Convert.ToBase64String(memoryStream.GetBuffer(), 0, Convert.ToInt32(memoryStream.Length));
			}
		}

		/// <summary>
		/// Decrypt encrypted string to the normal string
		/// </summary>
		/// <param name="cryptedString">Decrypted string</param>
		/// <returns>Normal (Decrypted) string</returns>
		public string Decrypt(string cryptedString, string sCampo, int id)
		{
			if (String.IsNullOrEmpty(cryptedString))
			{
				throw new ArgumentNullException
				   ("O texto a ser encriptado não pode ser nulo.");
			}
			using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
			{
				MemoryStream memoryStream = new MemoryStream
								(Convert.FromBase64String(cryptedString));
				CryptoStream cryptoStream = new CryptoStream(memoryStream,
					cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
				StreamReader reader = new StreamReader(cryptoStream);
				return reader.ReadToEnd();
			}
		}
		/// <summary>
		/// Decrypt encrypted string to the normal string
		/// </summary>
		/// <param name="cryptedString">Decrypted string</param>
		/// <returns>Normal (Decrypted) string</returns>
		public string Decrypt(string cryptedString)
        {
            if (String.IsNullOrEmpty(cryptedString))
            {
                throw new ArgumentNullException
                   ("O texto a ser encriptado não pode estar vazio.");
            }
            try
			{
				using (DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider())
				{
					MemoryStream memoryStream = new MemoryStream
										(Convert.FromBase64String(cryptedString));
					CryptoStream cryptoStream = new CryptoStream(memoryStream,
						cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
					StreamReader reader = new StreamReader(cryptoStream);
					return reader.ReadToEnd();
				}
			}
			catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }
    }
}
