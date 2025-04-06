using System.Reflection;
using MediatRExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Core.Application
{
    public static class ConfigureServiceRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(currentAssembly);
            services.RegisterMediatRWithLoggingAndValidation(currentAssembly);
            return services;
        }
    }
}
