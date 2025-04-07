using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Infrastucture.Persistence.Models;
using NotificationService.Infrastucture.Persistence.Repositories;

namespace NotificationService.Infrastucture.Persistence
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<INotificationRepository, NotificationRepository>();

            services.Configure<MongoDbConfiguration>(configuration.GetSection(nameof(MongoDbConfiguration)));

            services.AddTransient(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MongoDbConfiguration>>().Value;
                var client = new MongoClient(options.ConnectionString);
                return client.GetDatabase(options.DatabaseName);
            });

            return services;
        }
    }
}