using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer
{
    public static class KafkaConsumerRegistrationExtension
    {
        public static IServiceCollection AddConsumer<TMessage, TCommandOrNotification>(
            this IServiceCollection services,
            IConfigurationSection kafkaConfigurationSection,
            IConfigurationSection consumerConfigurationSection,
            Func<IMediator, TCommandOrNotification, CancellationToken, Task> mediatorAction)
        {
            var kafkaSettings = kafkaConfigurationSection.Get<KafkaSettings>()
                ?? throw new ApplicationException($"Couldn't get configuration from {nameof(kafkaConfigurationSection)} for {typeof(TMessage).Name} consumer");

            var consumerSettings = consumerConfigurationSection.Get<KafkaConsumerSettings>()
                ?? throw new ApplicationException($"Couldn't get configuration from {nameof(consumerConfigurationSection)} for {typeof(TMessage).Name} consumer");

            services.AddHostedService(sp =>
            {
                var mapper = sp.GetRequiredService<IMapper>();
                var mediator = sp.GetRequiredService<IMediator>();
                var logger = sp.GetRequiredService<ILogger<KafkaConsumer<TMessage, TCommandOrNotification>>>();
                return new KafkaConsumer<TMessage, TCommandOrNotification>(kafkaSettings, consumerSettings, mediator, mapper, logger, mediatorAction);
            });
            return services;
        }
    }
}
