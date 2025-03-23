using System.Reflection;
using System.Text.Json;
using Cache.Contracts;
using Cache.Models;
using EmailSender.Contracts;
using EmailSender.Models;
using Exceptions;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Infrastucture.Infrastucture.AuthClient;


namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        private const string HTTTP_CLIENT_NAME = "WithHandler";
        private const string AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME = "AuthApiUri";
        private const string USE_DEFAULT_CACHE_ENVIRONMENT_VARIBALE_NAME = "UseDefaultCache";
        private const string REDIS_URL_ENVIRONMENT_VARIBALE_NAME = "RedisUri";
        private const string EMAIL_SETTINGS_ENVIRONMENT_VARIBALE_NAME = "EmailSettings";

        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, bool isDevelopment)
        {
            //services.AddScoped<AuthHeaderHandler>();

            //services.AddHttpClient(HTTTP_CLIENT_NAME).AddHttpMessageHandler<AuthHeaderHandler>();

            //services.AddScoped<IAuthApiClient, AuthApiClient>(sp =>
            //{
            //    var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
            //    var client = clientFactory.CreateClient(HTTTP_CLIENT_NAME);
            //    var url = Environment.GetEnvironmentVariable(AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME)
            //        ?? throw new CouldNotGetEnvironmentVariableException(AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME);
            //    return new AuthApiClient(url, client);
            //});

            services.AddScoped<IAuthApiClientService, AuthClientService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

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

            return services;
        }
    }
}
