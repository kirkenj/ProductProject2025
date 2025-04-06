using Cache.Models.InMemory;
using Cache.Models.Reddis;
using Clients.Adapters.AuthClient.Services;
using Exceptions;
using HttpDelegatingHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace ProductService.Infrastucture.Infrastucture
{
    public static class InfrastructureServiceRegistration
    {
        private const string HTTTP_CLIENT_NAME = "WithHandler";
        private const string AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME = "AuthApiUri";
        private const string USE_DEFAULT_CACHE_ENVIRONMENT_VARIBALE_NAME = "UseDefaultCache";
        private const string REDIS_URL_ENVIRONMENT_VARIBALE_NAME = "RedisUri";

        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureConsumers(configuration);

            services.AddScoped<AuthHeaderHandler>();

            services.AddHttpClient(HTTTP_CLIENT_NAME).AddHttpMessageHandler<AuthHeaderHandler>();

            var settings = new AuthClientSettings()
            {
                ServiceUri = Environment.GetEnvironmentVariable(AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME)
                    ?? throw new CouldNotGetEnvironmentVariableException(AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME, typeof(string).Name),
                HttpClientName = HTTTP_CLIENT_NAME
            };
            services.AddAuthClientService(settings);

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
