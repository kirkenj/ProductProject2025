using Cache.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Cache.Models.Reddis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReddisCustomMemoryCache(this IServiceCollection services, string redisHost)
        {
            services.AddStackExchangeRedisCache(act =>
            {
                act.Configuration = redisHost;
            });
            services.AddTransient<ICustomMemoryCache, RedisCustomMemoryCache>();
            return services;
        }
    }
}
