using Clients.CustomGateway;
using Front.Services.MessageHandlers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Front.Configuration
{
    public static class HttpClientsRegistration
    {
        private const string BACKEND_URL_SECTION_NAME = "URL_BACKEND";

        public static IServiceCollection ConfigureClients(this IServiceCollection services, IConfiguration configuration)
        {
            var backendUrl = configuration[BACKEND_URL_SECTION_NAME]?.ToString() ?? throw new InvalidOperationException("Couldn't get backend url from configuration");

            services.AddScoped<HeadersMessageHandler>();
            services.AddScoped<TokenDelegatingHandler>(sp =>
            {
                var tokenProvider = sp.GetRequiredService<IAccessTokenProvider>();
                var navManager = sp.GetRequiredService<NavigationManager>();
                return new(tokenProvider, navManager, backendUrl);
            });

            services.AddHttpClient<IAuthGatewayClient, GatewayClient>(nameof(IAuthGatewayClient), a => a = new HttpClient());

            services.AddHttpClient<IGatewayClient, GatewayClient>(nameof(IGatewayClient), a => a = new HttpClient())
                .AddHttpMessageHandler<HeadersMessageHandler>()
                .AddHttpMessageHandler<TokenDelegatingHandler>();

            services.AddScoped<IAuthGatewayClient, GatewayClient>(sp =>
            {
                var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = clientFactory.CreateClient(nameof(IAuthGatewayClient));
                return new GatewayClient(backendUrl, client);
            });

            services.AddScoped<IGatewayClient, GatewayClient>(sp =>
            {
                var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var client = clientFactory.CreateClient(nameof(IGatewayClient));
                return new GatewayClient(backendUrl, client);
            });
            return services;
        }
    }
}
