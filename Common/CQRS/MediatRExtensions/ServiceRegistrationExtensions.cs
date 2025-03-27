using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace MediatRExtensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection RegisterMediatRWithLoggingAndValidation(this IServiceCollection services, Assembly registeringAssembly)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(registeringAssembly);
                cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}
