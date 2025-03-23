using AuthAPI.Contracts;
using AuthAPI.Models.TokenTracker;
using AuthAPI.Services;


namespace AuthAPI.Registrations
{
    public static class APIServicesRegistration
    {
        public static IServiceCollection ConfigureApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IJwtProviderService, JwtProviderService>();

            services.Configure<TokenTrackingSettings>(configuration.GetSection("TokenTrackingSettings"));
            services.AddScoped<ITokenTracker<Guid>, TokenTracker<Guid>>();

            return services;
        }
    }
}
