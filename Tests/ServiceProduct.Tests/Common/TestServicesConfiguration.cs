using Application.Contracts.AuthService;
using Application.Contracts.Persistence;
using Application.DTOs.Product;
using Application.Features.Product.Handlers.Commands;
using Application.MediatRBehaviors;
using Cache.Contracts;
using Clients.AuthApi;
using EmailSender.Contracts;
using FluentValidation;
using Infrastructure.AuthClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Persistence.Repositories;
using System.Reflection;
using Tools;

namespace ServiceProduct.Tests.Common
{
    public static class TestServicesConfiguration
    {
        public static IServiceCollection ConfigureTestServices(this IServiceCollection services)
        {
            services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });

            services.AddAutoMapper(typeof(ProductDto).Assembly);

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(UpdateProductComandHandler).Assembly);
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(RequestResponseLoggingBehavior<,>));
            });

            services.AddValidatorsFromAssembly(typeof(UpdateProductComandHandler).Assembly);

            services.AddLogging();
            services.AddSingleton<ICustomMemoryCache, RedisCustomMemoryCacheWithEvents>(sp => new("localhost:3300"));
            services.AddSingleton<IEmailSender, TestEmailSender>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAuthApiClient, TestAuthClient>();
            services.AddScoped<IAuthApiClientService, AuthClientService>();

            return services;
        }
    }
}
