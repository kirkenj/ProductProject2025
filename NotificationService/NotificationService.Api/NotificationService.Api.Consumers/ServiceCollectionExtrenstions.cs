using System.Reflection;
using Messaging.Kafka;
using Messaging.Kafka.Consumer;
using Messaging.Messages.AuthService;
using Messaging.Messages.ProductService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Api.Consumers.ConsumerMediatRActions;
using NotificationService.Core.Application.Features.AuthService.ChangeEmailRequest;
using NotificationService.Core.Application.Features.AuthService.ForgotPassword;
using NotificationService.Core.Application.Features.AuthService.UserRegistrationRequestCreated;
using NotificationService.Core.Application.Features.ProductService.ProductCreated;
using NotificationService.Core.Application.Features.ProductService.ProductDeleted;
using NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated;

namespace NotificationService.Api.Consumers
{
    public static class ServiceCollectionExtrenstions
    {
        public static IServiceCollection RegisterConsumers(this IServiceCollection services, IConfiguration configuration)
        {
            var kafkaSettingsSection = configuration.GetSection(nameof(KafkaSettings));
            var defaultConsumerSettingsSection = configuration.GetSection(nameof(KafkaConsumerSettings));

            services.AddConsumer<UserRegistrationRequestCreated, UserRegistrationRequestCreatedCommand>(kafkaSettingsSection, defaultConsumerSettingsSection, MediatorPublicationStrategies.AsRequest);
            services.AddConsumer<ForgotPassword, ForgotPasswordCommand>(kafkaSettingsSection, defaultConsumerSettingsSection, MediatorPublicationStrategies.AsRequest);
            services.AddConsumer<ChangeEmailRequest, ChangeEmailRequestCommand>(kafkaSettingsSection, defaultConsumerSettingsSection, MediatorPublicationStrategies.AsRequest);

            services.AddConsumer<ProductDeleted, ProductDeletedNotificationRequest>(kafkaSettingsSection, defaultConsumerSettingsSection, ConsumerMediatRAction.GetAndPublishNotificationFromRequest);
            services.AddConsumer<ProductCreated, ProductCreatedNotificationRequest>(kafkaSettingsSection, defaultConsumerSettingsSection, ConsumerMediatRAction.GetAndPublishNotificationFromRequest);
            services.AddConsumer<ProductProducerUpdated, ProductProducerUpdatedNotificationRequest>(kafkaSettingsSection, defaultConsumerSettingsSection, ConsumerMediatRAction.GetAndPublishNotificationFromRequest);

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
