using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthAPI.Models.Jwt;
using AuthService.API.AuthAPI.Contracts;
using AuthService.Core.Application.Features.User.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthAPI.Services
{
    public class JwtProviderService : IJwtProviderService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtProviderService(IOptions<JwtSettings> options)
        {
            if (options == null || options.Value == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _jwtSettings = options.Value;
        }

        public string GetToken(UserDto user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Role == null)
            {
                throw new ArgumentNullException(nameof(user), "User's role object is null");
            }

            var claims = new Claim[]
            {
                new (ClaimTypes.Role, user.Role.Name),
                new (ClaimTypes.Name, user.Login),
                new (ClaimTypes.Sid, user.Id.ToString()),
                new (ClaimTypes.Email, user.Email ?? "null")
            };


            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            SigningCredentials signingCredentials = new(key, _jwtSettings.SecurityAlgorithm);

            var jwtToken = new JwtSecurityToken
                (
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                    signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
