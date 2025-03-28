using System.Reflection;
using Messaging.Kafka.Consumer;
using Messaging.Messages.AuthService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Api.Consumers.ConsumerSettings;
using NotificationService.Core.Application.Features.Notificatioin.UserRegistrationRequestCreated;

namespace NotificationService.Api.Consumers
{
    public static class ServiceCollectionExtrenstions
    {
        public static IServiceCollection RegisterConsumers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddConsumer<UserRegistrationRequestCreated, UserRegistrationRequestCreatedCommand, UserRegistrationRequestCreatedConsumerOptions>(configuration.GetSection(nameof(UserRegistrationRequestCreatedConsumerOptions)));
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
