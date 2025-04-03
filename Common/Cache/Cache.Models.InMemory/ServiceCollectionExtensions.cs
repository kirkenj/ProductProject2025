using Cache.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Cache.Models.InMemory
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoruCustomMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddTransient<ICustomMemoryCache, InMemoryCache>();
            return services;
        }
    }
}
