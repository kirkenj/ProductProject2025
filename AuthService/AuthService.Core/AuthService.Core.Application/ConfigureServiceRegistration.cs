using System.Reflection;
using AuthService.Core.Application.Contracts.Application;
using AuthService.Core.Application.Models.User.Settings;
using AuthService.Core.Application.Services;
using FluentValidation;
using MediatRExtensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AuthService.Core.Application
{
    public static class ConfigureServiceRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(currentAssembly);
            services.AddValidatorsFromAssembly(currentAssembly);
            services.RegisterMediatRWithLoggingAndValidation(currentAssembly);

            services.AddScoped<IPasswordSetter, PasswordSetter>();

            services.Configure<CreateUserSettings>(configuration.GetSection("CreateUserSettings"));
            services.Configure<UpdateUserEmailSettings>(configuration.GetSection("UpdateUserEmailSettings"));

            return services;
        }
    }
}
