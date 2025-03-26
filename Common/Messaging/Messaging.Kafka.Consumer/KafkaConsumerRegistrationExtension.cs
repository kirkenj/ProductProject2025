using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Consumer
{
    public static class KafkaConsumerRegistrationExtension
    {
        public static IServiceCollection AddConsumer<TMessage, THandler>(this IServiceCollection services, IConfigurationSection configurationSection) where THandler : class, IMessageHandler<Message<string, TMessage>>
        {
            services.Configure<KafkaConsumerSettings>(configurationSection);
            services.AddHostedService<KafkaConsumer<TMessage>>();
            services.AddSingleton<IMessageHandler<Message<string,TMessage>>, THandler>();
            return services;
        } 
    }
}
