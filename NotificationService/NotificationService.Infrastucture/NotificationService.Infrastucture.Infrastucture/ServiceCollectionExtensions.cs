using Cache.Models.InMemory;
using Cache.Models.Reddis;
using Clients.Adapters.AuthClient.Services;
using Clients.Adapters.ProductClient.Services;
using EmailSender.Contracts;
using EmailSender.Services;
using Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Infrastucture.Infrastucture
{
    public static class ServiceCollectionExtensions
    {
        private const string REDIS_URL_ENVIRONMENT_VARIBALE_NAME = "RedisUri";
        private const string USE_DEFAULT_CACHE_ENVIRONMENT_VARIBALE_NAME = "UseDefaultCache";

        public static IServiceCollection RegisterInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            var UseConsoleEmailSenderString = Environment.GetEnvironmentVariable("UseConsoleEmailSender")! ?? throw new Exception();
            if (!bool.Parse(UseConsoleEmailSenderString))
            {
                var section = configuration.GetSection(nameof(SmtpEmailSenderSettings));
                services.AddTransient<IEmailSender, SmtpEmailSender>();
                services.Configure<SmtpEmailSenderSettings>(section);
            }
            else
            {
                services.AddTransient<IEmailSender, ConsoleEmailSender>();
            }

            var authApiAddress = Environment.GetEnvironmentVariable("AuthApiAddress") ?? throw new CouldNotGetEnvironmentVariableException("AuthApiAddress", typeof(string).Name);
            services.AddAuthClientService(new AuthClientSettings { ServiceUri = authApiAddress });

            var productApiAddress = Environment.GetEnvironmentVariable("ProductApiAddress") ?? throw new CouldNotGetEnvironmentVariableException("ProductApiAddress", typeof(string).Name);
            services.AddProductClientService(new ProductClientSettings { ServiceUri = productApiAddress });

            var useDefaultCacheStr = Environment.GetEnvironmentVariable(USE_DEFAULT_CACHE_ENVIRONMENT_VARIBALE_NAME);
            if (useDefaultCacheStr != null && bool.TryParse(useDefaultCacheStr, out bool result) && result)
            {
                services.AddInMemoruCustomMemoryCache();
            }
            else
            {
                var redisConString = Environment.GetEnvironmentVariable(REDIS_URL_ENVIRONMENT_VARIBALE_NAME)
                    ?? throw new CouldNotGetEnvironmentVariableException(REDIS_URL_ENVIRONMENT_VARIBALE_NAME, typeof(string).Name);
                services.AddReddisCustomMemoryCache(redisConString);
            }

            return services;
        }
    }
}