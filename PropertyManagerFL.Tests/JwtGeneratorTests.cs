using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PropertyManagerFL.Infrastructure.Services.SecurityServices;

namespace PropertyManagerFL.Tests;

public class JwtGeneratorTests
{
    private const string Secret = "test-secret-with-at-least-32-characters";
    private const string Issuer = "https://localhost:5001/";
    private const string Audience = "https://localhost:5001/";

    [Fact]
    public void GenerateToken_WritesExpectedIssuerAudienceAndClaims()
    {
        var generator = new JwtGenerator(Secret, Issuer, Audience);

        var token = generator.GenerateToken(42, "joao", "Admin");
        var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

        Assert.Equal(Issuer, parsed.Issuer);
        Assert.Contains(Audience, parsed.Audiences);
        Assert.Contains(parsed.Claims, c => c.Type == "sub" && c.Value == "42");
        Assert.Contains(parsed.Claims, c => c.Type == "name" && c.Value == "joao");
        Assert.Contains(parsed.Claims, c => c.Value == "Admin");
    }

    [Fact]
    public void GenerateToken_IsSignedWithConfiguredSecretAndPassesValidation()
    {
        var generator = new JwtGenerator(Secret, Issuer, Audience);
        var token = generator.GenerateToken(7, "maria");

        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);

        // Default inbound claim mapping converts the JWT "sub" claim to NameIdentifier.
        Assert.Equal("7", principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("maria", principal.FindFirst("name")?.Value);
    }
}
