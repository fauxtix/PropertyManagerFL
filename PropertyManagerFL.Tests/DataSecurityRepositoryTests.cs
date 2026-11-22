using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using PropertyManagerFL.Infrastructure.Security;

namespace PropertyManagerFL.Tests;

public class DataSecurityRepositoryTests
{
    [Fact]
    public void Encrypt_Decrypt_RoundTripsWithDataProtection()
    {
        var root = Path.Combine(Path.GetTempPath(), "pmfl-dp-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            var provider = DataProtectionProvider.Create(new DirectoryInfo(root),
                builder => builder.SetApplicationName("PropertyManagerFL.Tests"));
            var repository = new DataSecurityRepository(provider);

            const string plain = "roundtrip-ok";
            var encrypted = repository.Encrypt(plain);

            Assert.NotEqual(plain, encrypted);
            Assert.Equal(plain, repository.Decrypt(encrypted));
            Assert.Equal(string.Empty, repository.Encrypt(string.Empty));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Decrypt_FallsBackToLegacyDesValue()
    {
        var root = Path.Combine(Path.GetTempPath(), "pmfl-dp-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);

        try
        {
            var provider = DataProtectionProvider.Create(new DirectoryInfo(root),
                builder => builder.SetApplicationName("PropertyManagerFL.Tests"));
            var repository = new DataSecurityRepository(provider);
            const string plain = "legacy-value";

            Assert.Equal(plain, repository.Decrypt(LegacyEncrypt(plain)));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string LegacyEncrypt(string value)
    {
        var key = ASCIIEncoding.ASCII.GetBytes("ZeroCool");
        using var crypto = new DESCryptoServiceProvider();
        using var memory = new MemoryStream();
        using (var stream = new CryptoStream(memory, crypto.CreateEncryptor(key, key), CryptoStreamMode.Write))
        {
            using var writer = new StreamWriter(stream);
            writer.Write(value);
        }

        return Convert.ToBase64String(memory.ToArray());
    }
}
