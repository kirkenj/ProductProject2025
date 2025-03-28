using Messaging.Kafka.Producer.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Messaging.Kafka.Producer.Models
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterProducer<TMessage>(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.Get<KafkaSettings>()
                ?? throw new ApplicationException($"Couldn't get configuration from {nameof(configuration)} for {typeof(TMessage).Name} producer");
            services.AddSingleton<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>(sp => new KafkaProducer<TMessage>(options));
            return services;
        }
    }
}
