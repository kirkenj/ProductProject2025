﻿using System.Reflection;
using Cache.Contracts;
using Cache.Models;
using Clients.AuthApi;
using Exceptions;
using HttpDelegatingHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Infrastucture.Infrastucture.AuthClient;


namespace ProductService.Infrastucture.Infrastucture
{
    public static class InfrastructureServiceRegistration
    {
        private const string HTTTP_CLIENT_NAME = "WithHandler";
        private const string AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME = "AuthApiUri";
        private const string USE_DEFAULT_CACHE_ENVIRONMENT_VARIBALE_NAME = "UseDefaultCache";
        private const string REDIS_URL_ENVIRONMENT_VARIBALE_NAME = "RedisUri";

        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
        {
            services.ConfigureConsumers(configuration);

            services.AddScoped<AuthHeaderHandler>();

            services.AddHttpClient(HTTTP_CLIENT_NAME).AddHttpMessageHandler<AuthHeaderHandler>();

            services.AddScoped<IAuthApiClient, AuthApiClient>(sp =>
            {
                var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = clientFactory.CreateClient(HTTTP_CLIENT_NAME);
                var url = Environment.GetEnvironmentVariable(AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME)
                    ?? throw new CouldNotGetEnvironmentVariableException(AUTH_API_URI_ENVIRONMENT_VARIBALE_NAME);
                return new AuthApiClient(url, client);
            });

            services.AddScoped<IAuthApiClientService, AuthClientService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

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
