using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CentralizedJwtAuthentication
{
    public static class JwtAuthenticationRegistration
    {
        public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            services.AddScoped<CustomJwtBearerEvents>();
            services.AddScoped<IJwtValidatingService, JwtValidatingService>();

            var settings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>() ??
                throw new Exception(nameof(JwtSettings));

            services.Configure<JwtSettings>((configuration) =>
            {
                configuration.IssuerTokenValidatePostMethodUri = settings.IssuerTokenValidatePostMethodUri;
                configuration.Issuer = settings.Issuer;
                configuration.DurationInMinutes = settings.DurationInMinutes;
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
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = false,
                    SignatureValidator = (token, validationParameters) => new JsonWebToken(token),
                    ValidateLifetime = true,
                };

                o.EventsType = typeof(CustomJwtBearerEvents);
            });

            return services;
        }
    }
}
