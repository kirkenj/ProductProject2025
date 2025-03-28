using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer
{
    public static class KafkaConsumerRegistrationExtension
    {
        public static IServiceCollection AddConsumer<TMessage, TCommand>(
            this IServiceCollection services, 
            IConfigurationSection kafkaConfigurationSection, 
            IConfigurationSection consumerConfigurationSection) 
            where TCommand : IRequest
        {
            var kafkaSettings = kafkaConfigurationSection.Get<KafkaSettings>()
                ?? throw new ApplicationException($"Couldn't get configuration from {nameof(kafkaConfigurationSection)} for {typeof(TMessage).Name} consumer");

            var consumerSettings = consumerConfigurationSection.Get<KafkaConsumerSettings>()
                ?? throw new ApplicationException($"Couldn't get configuration from {nameof(consumerConfigurationSection)} for {typeof(TMessage).Name} consumer");

            services.AddHostedService(sp =>
            {
                var mapper = sp.GetRequiredService<IMapper>();
                var mediator = sp.GetRequiredService<IMediator>();
                var logger = sp.GetRequiredService<ILogger<KafkaConsumer<TMessage, TCommand>>>();
                return new KafkaConsumer<TMessage, TCommand>(kafkaSettings, consumerSettings, mediator, mapper, logger);
            });
            return services;
        }
    }
}
