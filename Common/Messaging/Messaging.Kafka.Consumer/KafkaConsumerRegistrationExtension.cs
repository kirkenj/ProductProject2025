using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Consumer
{
    public static class KafkaConsumerRegistrationExtension
    {
        public static IServiceCollection AddConsumer<TMessage, TCommand, TOptions>(
            this IServiceCollection services, 
            IConfigurationSection configurationSection) 
            where TCommand : IRequest
            where TOptions : KafkaConsumerSettings
        {
            services.Configure<TOptions>(configurationSection);
            services.AddHostedService<KafkaConsumer<TMessage, TCommand, TOptions>>();
            return services;
        }
    }
}
