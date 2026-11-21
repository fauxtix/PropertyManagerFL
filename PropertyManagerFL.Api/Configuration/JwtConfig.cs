using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PropertyManagerFL.Api.Middlewares;
using PropertyManagerFL.Application.Interfaces.Services.JWT;
using PropertyManagerFL.Infrastructure.Services.JWT;
using System.Text;

namespace PropertyManagerFL.Api.Configuration;

public static class JwtConfig
{
    /// <summary>
    ///  JWT configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddJwtTConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtService, JwtService>();

        var secretValue = configuration.GetSection("JWT:Secret")?.Value;
        if (string.IsNullOrWhiteSpace(secretValue) ||
            secretValue.StartsWith('<') ||
            secretValue.Length < 32)
        {
            throw new InvalidOperationException("JWT secret must be set in user-secrets or environment configuration.");
        }

        var chave = Encoding.UTF8.GetBytes(secretValue);

        services.AddAuthentication(p =>
        {
            p.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            p.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(p =>
        {
            p.RequireHttpsMetadata = true;
            p.SaveToken = true;
            p.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(chave),
                ValidateIssuer = true,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JWT:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
    }

    /// <summary>
    /// Jwt configuration
    /// </summary>
    /// <param name="app"></param>
    public static void UseJwtConfiguration(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<ApiKeyMiddleware>();
    }
}