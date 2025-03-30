using Confluent.Kafka;
using Messaging.Kafka.Producer.Contracts;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Producer.Models
{
    public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
    {
        private readonly IProducer<string, TMessage> _producer;
        private readonly ILogger<KafkaProducer<TMessage>> _logger;

        private static string Topic => typeof(TMessage).FullName!;

        public KafkaProducer(KafkaSettings options, ILogger<KafkaProducer<TMessage>> logger)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = options.BootStrapServers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, TMessage>(config)
                .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
                .Build();
            _logger = logger;
        }

        public void Dispose()
        {
            _producer.Dispose();
        }

        public async Task<DeliveryResult<string, TMessage>> ProduceAsync(TMessage message, CancellationToken cancellationToken)
        {
            var messageToPush = new Message<string, TMessage>()
            {
                Key = Guid.NewGuid().ToString(),
                Value = message
            };

            _logger.LogInformation("Sending message {msg}", messageToPush);
            return await _producer.ProduceAsync(Topic, messageToPush, cancellationToken);
        }
    }
}
