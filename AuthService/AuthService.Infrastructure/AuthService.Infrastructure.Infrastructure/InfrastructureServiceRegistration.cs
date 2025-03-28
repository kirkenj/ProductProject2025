using Cache.Contracts;
using Cache.Models;
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
        private const string USE_DEFAULT_CACHE_ENVIRONMENT_VARIBALE_NAME = "UseDefaultCache";
        private const string REDIS_URL_ENVIRONMENT_VARIBALE_NAME = "RedisUri";

        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.Configure<HashProviderSettings>(configuration.GetSection(HASH_PROVIDER_SETTINGS_ENVIRONMENT_VARIBALE_NAME));

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

            services.RegisterProdusers(configuration);

            return services;
        }
    }
}
