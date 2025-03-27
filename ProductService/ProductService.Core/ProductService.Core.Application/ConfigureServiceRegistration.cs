using System.Reflection;
using FluentValidation;
using MediatRExtensions;
using Microsoft.Extensions.DependencyInjection;

namespace ProductService.Core.Application
{
    public static class ConfigureServiceRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            var currentAssemnly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(currentAssemnly);
            services.AddValidatorsFromAssembly(currentAssemnly);
            services.RegisterMediatRWithLoggingAndValidation(currentAssemnly);

            return services;
        }
    }
}
