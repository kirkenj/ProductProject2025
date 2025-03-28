using System.Text.Json;
using AuthService.Core.Application.Contracts.Infrastructure;
using AuthService.Infrastructure.Infrastructure.Producers;
using Cache.Contracts;
using Cache.Models;
using EmailSender.Contracts;
using EmailSender.Models;
using Exceptions;
using HashProvider.Contracts;
using HashProvider.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        private const string HASH_PROVIDER_SETTINGS_ENVIRONMENT_VARIBALE_NAME = "HashProviderSettings";
        private const string EMAIL_SETTINGS_ENVIRONMENT_VARIBALE_NAME = "EmailSettings";
        private const string USE_DEFAULT_CACHE_ENVIRONMENT_VARIBALE_NAME = "UseDefaultCache";
        private const string REDIS_URL_ENVIRONMENT_VARIBALE_NAME = "RedisUri";

        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.Configure<HashProviderSettings>(configuration.GetSection(HASH_PROVIDER_SETTINGS_ENVIRONMENT_VARIBALE_NAME));

            if (isDevelopment)
            {
                services.AddScoped<IEmailSender, ConsoleEmailSender>();
            }
            else
            {
                var EmailSettingsJson = Environment.GetEnvironmentVariable(EMAIL_SETTINGS_ENVIRONMENT_VARIBALE_NAME) ?? throw new CouldNotGetEnvironmentVariableException(EMAIL_SETTINGS_ENVIRONMENT_VARIBALE_NAME);

                var settings = JsonSerializer.Deserialize<EmailSettings>(EmailSettingsJson) ?? throw new InvalidOperationException("Couldn't deserialize Emailsettings from environment");

                services.Configure<EmailSettings>((a) =>
                {
                    a.FromName = settings.FromName ?? throw new ArgumentException($"EmailSettings: Value can not be null ({nameof(settings.FromName)})");
                    if (settings.ApiPort == default) throw new ArgumentException($"EmailSettings: Value can not be default ({nameof(settings.FromName)})");
                    a.ApiPort = settings.ApiPort;
                    a.ApiPassword = settings.ApiPassword ?? throw new ArgumentException($"EmailSettings: Value can not be null ({nameof(settings.ApiPassword)})");
                    a.ApiLogin = settings.ApiLogin ?? throw new ArgumentException($"EmailSettings: Value can not be null ({nameof(settings.ApiLogin)})");
                    a.ApiAdress = settings.ApiAdress ?? throw new ArgumentException($"EmailSettings: Value can not be null ({nameof(settings.ApiAdress)})");
                });
                services.AddScoped<IEmailSender, EmailSender.Models.EmailSender>();
            }

            var useDefaultCacheStr = Environment.GetEnvironmentVariable(USE_DEFAULT_CACHE_ENVIRONMENT_VARIBALE_NAME);

            if (useDefaultCacheStr != null && bool.TryParse(useDefaultCacheStr, out bool result) && result)
            {
                services.AddMemoryCache();
                services.AddScoped<ICustomMemoryCache, CustomMemoryCache>();
            }
            else
            {
                var redisConString = Environment.GetEnvironmentVariable(REDIS_URL_ENVIRONMENT_VARIBALE_NAME)
                    ?? throw new CouldNotGetEnvironmentVariableException(REDIS_URL_ENVIRONMENT_VARIBALE_NAME);
                services.AddScoped<ICustomMemoryCache, RedisCustomMemoryCache>(sp => new RedisCustomMemoryCache(redisConString));
            }

            services.AddTransient<IHashProvider, HashProvider.Models.HashProvider>();
            services.AddTransient<IPasswordGenerator, PasswordGenerator.PasswordGenerator>();

            services.RegisterProdusers(configuration);

            return services;
        }
    }
}
