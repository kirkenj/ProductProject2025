using Messaging.Kafka;
using Messaging.Kafka.Producer.Models;
using Messaging.Messages.ProductService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductService.Infrastucture.Infrastucture
{
    public static class ConsumersServiceExtension
    {
        public static IServiceCollection ConfigureConsumers(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultSection = configuration.GetSection(nameof(KafkaSettings));

            services.RegisterProducer<ProductCreated>(defaultSection);
            services.RegisterProducer<ProductDeleted>(defaultSection);
            services.RegisterProducer<ProductProducerUpdated>(defaultSection);

            return services;
        }
    }
}
