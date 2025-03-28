using System.Reflection;
using Messaging.Kafka;
using Messaging.Kafka.Consumer;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Core.Application.Features.Notificatioin.AccountConfirmed;
using NotificationService.Core.Application.Features.Notificatioin.ChangeEmailRequest;
using NotificationService.Core.Application.Features.Notificatioin.ForgotPassword;
using NotificationService.Core.Application.Features.Notificatioin.UserRegistrationRequestCreated;

namespace NotificationService.Api.Consumers
{
    public static class ServiceCollectionExtrenstions
    {
        public static IServiceCollection RegisterConsumers(this IServiceCollection services, IConfiguration configuration)
        {
            var kafkaSettingsSection = configuration.GetSection(nameof(KafkaSettings));
            var defaultConsumerSettingsSection = configuration.GetSection(nameof(KafkaConsumerSettings));

            services.AddConsumer<UserRegistrationRequestCreated, UserRegistrationRequestCreatedCommand>(kafkaSettingsSection, defaultConsumerSettingsSection);
            services.AddConsumer<ForgotPassword, ForgotPasswordCommand>(kafkaSettingsSection, defaultConsumerSettingsSection);
            services.AddConsumer<ChangeEmailRequest, ChangeEmailRequestCommand>(kafkaSettingsSection, defaultConsumerSettingsSection);
            services.AddConsumer<AccountConfirmed, AccountConfirmedCommand>(kafkaSettingsSection, defaultConsumerSettingsSection);

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
