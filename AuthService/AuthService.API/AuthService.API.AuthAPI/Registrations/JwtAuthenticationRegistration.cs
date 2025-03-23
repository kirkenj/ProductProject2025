using AuthAPI.Models.Jwt;
using Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace AuthService.API.AuthAPI.Registrations
{
    public static class JwtAuthenticationRegistration
    {
        private const string JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME = "JwtSettings";

        public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services)
        {
            var settingsJson = Environment.GetEnvironmentVariable(JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME) ?? throw new CouldNotGetEnvironmentVariableException(JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME);
            var settings = JsonSerializer.Deserialize<JwtSettings>(settingsJson) ?? throw new InvalidOperationException("Couldn't deserialize JwtSettings from environment");

            services.Configure<JwtSettings>((configuration) =>
            {
                configuration.Issuer = settings.Issuer ?? throw new ArgumentException($"JwtSettings: {nameof(settings.Issuer)}  is null");
                configuration.Audience = settings.Audience ?? throw new ArgumentException($"JwtSettings: {nameof(settings.Audience)} is null");
                if (settings.DurationInMinutes == default) throw new ArgumentException($"JwtSettings: {nameof(settings.DurationInMinutes)} is default");
                configuration.DurationInMinutes = settings.DurationInMinutes;
                configuration.SecurityAlgorithm = settings.SecurityAlgorithm ?? throw new ArgumentException($"JwtSettings: {nameof(settings.SecurityAlgorithm)} is null");
                configuration.Key = settings.Key ?? throw new ArgumentException($"JwtSettings: {nameof(settings.Key)} is null");
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key))
                };
            });

            return services;
        }
    }
}
