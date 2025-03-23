using Front.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Front.Configuration
{
    public static class AuthenticationServicesRegistration
    {
        public static IServiceCollection ConfigureAuthenticationServices(this IServiceCollection services)
        {
            services.AddScoped<IAccessTokenProvider, CustomTokenAccessor>();
            services.AddScoped<LocalStorageAccessor>();
            services.AddScoped<UserService>();
            services.AddScoped<CustomAuthStateProvider>();
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(s => s.GetRequiredService<CustomAuthStateProvider>());
            return services;
        }
    }
}
