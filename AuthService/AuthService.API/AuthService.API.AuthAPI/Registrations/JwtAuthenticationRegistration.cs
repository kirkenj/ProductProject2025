using System.Text;
using System.Text.Json;
using AuthService.API.AuthAPI.Models.Jwt;
using Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthService.API.AuthAPI.Registrations
{
    public static class JwtAuthenticationRegistration
    {
        private const string JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME = "JwtSettings";

        public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services)
        {
            var settingsJson = Environment.GetEnvironmentVariable(JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME) ?? throw new CouldNotGetEnvironmentVariableException(JWT_SETTINGS_ENVIRONMENT_VARIBALE_NAME);
            JwtSettings settings = JsonSerializer.Deserialize<JwtSettings>(settingsJson) ?? throw new InvalidOperationException("Couldn't deserialize JwtSettings from environment");

            services.Configure<JwtSettings>(configuration =>
            {
                configuration.Issuer = settings.Issuer;
                configuration.Audience = settings.Audience;
                configuration.DurationInMinutes = settings.DurationInMinutes;
                configuration.SecurityAlgorithm = settings.SecurityAlgorithm;
                configuration.Key = settings.Key;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = settings.ToTokenValidationParameters();
            });

            return services;
        }
    }
}
