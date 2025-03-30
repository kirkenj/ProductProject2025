using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.API.AuthAPI.Models.Jwt
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public double DurationInMinutes { get; set; }
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public string SecurityAlgorithm { get; set; } = null!;

        public TokenValidationParameters ToTokenValidationParameters() => new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudience = Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
            ValidAlgorithms = [SecurityAlgorithm]
        };
    }
}
