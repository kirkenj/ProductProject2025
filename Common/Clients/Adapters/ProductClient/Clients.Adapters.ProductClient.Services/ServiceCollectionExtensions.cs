using System.Reflection;
using Clients.Adapters.ProductClient.Contracts;
using Clients.ProductApi;
using Microsoft.Extensions.DependencyInjection;

namespace Clients.Adapters.ProductClient.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProductClientService(this IServiceCollection services, ProductClientSettings authClientSettings)
        {
            services.AddTransient<IProductApiClient, ProductApiClient>(sp =>
            {
                var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = string.IsNullOrWhiteSpace(authClientSettings.HttpClientName) ?
                    clientFactory.CreateClient()
                    : clientFactory.CreateClient(authClientSettings.HttpClientName);
                return new ProductApiClient(authClientSettings.ServiceUri, client);
            });

            services.AddTransient<IProductClientService, ProductClientService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
