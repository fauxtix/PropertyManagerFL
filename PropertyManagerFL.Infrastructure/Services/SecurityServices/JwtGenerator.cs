using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PropertyManagerFL.Infrastructure.Services.SecurityServices
{
    public class JwtGenerator : ITokenGenerator
    {
        private readonly string _jwtSecret;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtGenerator(string secret, string issuer, string audience)
        {
            _jwtSecret = secret;
            _issuer = issuer;
            _audience = audience;
        }

        public string GenerateToken(int userId, string username)
        {
            return GenerateToken(userId, username, string.Empty);
        }

        public string GenerateToken(int userId, string username, string role)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("sub", userId.ToString()),
                new Claim("name", username),
            };

            if (!string.IsNullOrWhiteSpace(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _issuer,
                Audience = _audience,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                    SecurityAlgorithms.HmacSha256),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
