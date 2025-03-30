using System.Reflection;
using Messaging.Kafka;
using Messaging.Kafka.Consumer;
using Messaging.Messages.AuthService;
using Messaging.Messages.ProductService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Core.Application.Features.Notificatioin.AuthService.ChangeEmailRequest;
using NotificationService.Core.Application.Features.Notificatioin.AuthService.ForgotPassword;
using NotificationService.Core.Application.Features.Notificatioin.AuthService.UserRegistrationRequestCreated;
using NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductCreated;
using NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductDeleted;
using NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductProducerUpdated;

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

            services.AddConsumer<ProductDeleted, ProductDeletedCommand>(kafkaSettingsSection, defaultConsumerSettingsSection);
            services.AddConsumer<ProductCreated, ProductCreatedCommand>(kafkaSettingsSection, defaultConsumerSettingsSection);
            services.AddConsumer<ProductProducerUpdated, ProductProducerUpdatedCommand>(kafkaSettingsSection, defaultConsumerSettingsSection);

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
