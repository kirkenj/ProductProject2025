using Messaging.Kafka;
using Messaging.Kafka.Producer.Models;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthService.Infrastructure.Infrastructure
{
    public static class ProducersRegistration
    {
        public static IServiceCollection RegisterProdusers(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultSection = configuration.GetSection(nameof(KafkaSettings));

            services.RegisterProducer<UserRegistrationRequestCreated>(defaultSection);
            services.RegisterProducer<ForgotPassword>(defaultSection);
            services.RegisterProducer<AccountConfirmed>(defaultSection);
            services.RegisterProducer<ChangeEmailRequest>(defaultSection);

            return services;
        }
    }
}
