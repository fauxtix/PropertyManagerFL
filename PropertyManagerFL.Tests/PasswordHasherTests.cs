using System.Security.Cryptography;
using PropertyManagerFL.Infrastructure.Services.SecurityServices;

namespace PropertyManagerFL.Tests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _hasher = new();

    [Fact]
    public void ComputeHash_ProducesDifferentHashAndSaltEachTime()
    {
        var first = _hasher.ComputeHash("same-password");
        var second = _hasher.ComputeHash("same-password");

        Assert.NotEqual(first.Password, second.Password);
        Assert.NotEqual(first.Salt, second.Salt);
        Assert.Equal(32, Convert.FromBase64String(first.Password).Length);
        Assert.Equal(16, Convert.FromBase64String(first.Salt).Length);
    }

    [Fact]
    public void NewHash_VerifiesCorrectPasswordOnly()
    {
        var hash = _hasher.ComputeHash("correct-password");

        Assert.True(_hasher.VerifyHashMatch(hash.Password, "correct-password", hash.Salt));
        Assert.False(_hasher.VerifyHashMatch(hash.Password, "wrong-password", hash.Salt));
    }

    [Fact]
    public void LegacySha1Hash_StillVerifies()
    {
        const string password = "legacy-password";
        var salt = RandomNumberGenerator.GetBytes(8);
        using var rfc = new Rfc2898DeriveBytes(password, salt, 10_000, HashAlgorithmName.SHA1);
        var legacyHash = rfc.GetBytes(20);

        var verified = _hasher.VerifyHashMatch(
            Convert.ToBase64String(legacyHash),
            password,
            Convert.ToBase64String(salt));

        Assert.True(verified);
    }

    [Fact]
    public void InvalidBase64_ReturnsFalseInsteadOfThrowing()
    {
        Assert.False(_hasher.VerifyHashMatch("not-base64!", "password", "also-not-base64!"));
    }
}
