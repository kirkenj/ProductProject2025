using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Infrastructure.Persistence.Repositories;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Persistence
{
    public static class PersistenceServiceRegistration
    {
        private const string DATABASE_CONNECTION_STRING_ENVIRONMENT_VARIBALE_NAME = "AuthDbConnectionString";

        public static IServiceCollection ConfigurePersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            var cString = Environment.GetEnvironmentVariable(DATABASE_CONNECTION_STRING_ENVIRONMENT_VARIBALE_NAME)
                ?? throw new CouldNotGetEnvironmentVariableException(DATABASE_CONNECTION_STRING_ENVIRONMENT_VARIBALE_NAME);

            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql(cString);
            });

            var cont = services.BuildServiceProvider().GetRequiredService<AuthDbContext>();
            cont.Database.EnsureCreated();
            cont.SaveChanges();

            return services;
        }
    }
}
