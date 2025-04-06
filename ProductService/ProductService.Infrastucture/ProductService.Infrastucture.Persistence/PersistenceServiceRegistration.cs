using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Core.Application.Contracts.Persistence;
using ProductService.Infrastucture.Persistence.Repositories;

namespace ProductService.Infrastucture.Persistence
{
    public static class PersistenceServiceRegistration
    {
        private const string DATABASE_CONNECTION_STRING_ENVIRONMENT_VARIBALE_NAME = "ProductDbConnectionString";

        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();

            var cString = Environment.GetEnvironmentVariable(DATABASE_CONNECTION_STRING_ENVIRONMENT_VARIBALE_NAME)
                ?? throw new CouldNotGetEnvironmentVariableException(DATABASE_CONNECTION_STRING_ENVIRONMENT_VARIBALE_NAME, typeof(string).Name);

            services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseNpgsql(cString);
            });

            using var scope = services.BuildServiceProvider();
            using var context = scope.GetRequiredService<ProductDbContext>();
            context.Database.EnsureCreated();
            return services;
        }
    }
}
