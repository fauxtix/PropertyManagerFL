using PropertyManagerFL.Application.ViewModels.Security.Models;
using System.Security.Cryptography;

namespace PropertyManagerFL.Infrastructure.Services.SecurityServices
{
    /// <summary>
    /// Hashes and verifies passwords with PBKDF2. New hashes use SHA-256 with a
    /// 16-byte salt and 100,000 iterations; verification still accepts the old
    /// SHA-1 / 8-byte / 10,000-iteration format so existing accounts keep working.
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int CurrentWorkFactor = 100_000;
        private const int CurrentSaltSize = 16;
        private const int CurrentHashSize = 32;

        private const int LegacyWorkFactor = 10_000;
        private const int LegacyHashSize = 20;

        public PasswordHash ComputeHash(string plainTextPassword)
        {
            var salt = RandomNumberGenerator.GetBytes(CurrentSaltSize);
            var hash = Derive(plainTextPassword, salt, CurrentWorkFactor, HashAlgorithmName.SHA256, CurrentHashSize);

            return new PasswordHash(Convert.ToBase64String(hash), Convert.ToBase64String(salt));
        }

        public bool VerifyHashMatch(string existingHashedPassword, string plainTextPassword, string salt)
        {
            try
            {
                var saltBytes = Convert.FromBase64String(salt);
                var expectedHash = Convert.FromBase64String(existingHashedPassword);

                var currentHash = Derive(plainTextPassword, saltBytes, CurrentWorkFactor, HashAlgorithmName.SHA256, expectedHash.Length);
                if (CryptographicOperations.FixedTimeEquals(currentHash, expectedHash))
                {
                    return true;
                }

                var legacyHash = Derive(plainTextPassword, saltBytes, LegacyWorkFactor, HashAlgorithmName.SHA1, expectedHash.Length);
                return CryptographicOperations.FixedTimeEquals(legacyHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static byte[] Derive(
            string password,
            byte[] salt,
            int iterations,
            HashAlgorithmName algorithm,
            int hashSize)
        {
            using var rfc = new Rfc2898DeriveBytes(password, salt, iterations, algorithm);
            return rfc.GetBytes(hashSize);
        }
    }
}
