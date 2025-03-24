using AuthAPI.Models.TokenTracker;
using AuthAPI.Services;
using AuthService.API.AuthAPI.Contracts;
using AuthService.API.AuthAPI.Services;


namespace AuthService.API.AuthAPI.Registrations
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
