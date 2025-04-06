using System.Reflection;
using Clients.Adapters.AuthClient.Contracts;
using Clients.AuthApi;
using Microsoft.Extensions.DependencyInjection;

namespace Clients.Adapters.AuthClient.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthClientService(this IServiceCollection services, AuthClientSettings authClientSettings)
        {
            services.AddTransient<IAuthApiClient, AuthApiClient>(sp =>
            {
                var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = string.IsNullOrWhiteSpace(authClientSettings.HttpClientName) ?
                    clientFactory.CreateClient()
                    : clientFactory.CreateClient(authClientSettings.HttpClientName);
                return new AuthApiClient(authClientSettings.ServiceUri, client);
            });

            services.AddTransient<IAuthApiClientService, AuthClientService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
