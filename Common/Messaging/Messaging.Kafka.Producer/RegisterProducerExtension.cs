using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Producer
{
    public static class RegisterProducerExtension
    {
        public static void AddProducer<TMessage>(this IServiceCollection services,
            IConfigurationSection settings)
        {
            services.Configure<KafkaSettings>(settings);
            services.AddSingleton<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>();
        }
    }
}
