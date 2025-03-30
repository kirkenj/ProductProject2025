using EmailSender.Contracts;
using EmailSender.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Infrastucture.Infrastucture
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterInfrastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            var UseConsoleEmailSenderString = Environment.GetEnvironmentVariable("UseConsoleEmailSender")! ?? throw new Exception();
            if (!bool.Parse(UseConsoleEmailSenderString))
            {
                var section = configuration.GetSection(nameof(SmtpEmailSenderSettings));
                services.AddTransient<IEmailSender, SmtpEmailSender>();
                services.Configure<SmtpEmailSenderSettings>(section);
            }
            else
            {
                services.AddTransient<IEmailSender, ConsoleEmailSender>();
            }
            
            return services;
        }
    }
}
