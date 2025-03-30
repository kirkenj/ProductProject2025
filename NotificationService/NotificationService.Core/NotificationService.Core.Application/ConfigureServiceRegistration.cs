using System.Reflection;
using Clients.AuthApi;
using MediatRExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Core.Application
{
    public static class ConfigureServiceRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            var authApiAddress = Environment.GetEnvironmentVariable("AuthApiAddress") ?? throw new Exception();
            services.AddTransient<IAuthApiClient, AuthApiClient>(sp => 
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                return new AuthApiClient(authApiAddress, httpClient);
            });

            var currentAssembly = Assembly.GetExecutingAssembly();
            services.RegisterMediatRWithLoggingAndValidation(currentAssembly);
            return services;
        }
    }
}
