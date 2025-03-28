using Confluent.Kafka;
using Messaging.Kafka.Producer.Contracts;

namespace Messaging.Kafka.Producer.Models
{
    public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
    {
        private readonly IProducer<string, TMessage> _producer;
        private static string Topic => typeof(TMessage).FullName!;

        public KafkaProducer(KafkaSettings options)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = options.BootStrapServers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, TMessage>(config)
                .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
                .Build();
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

            return await _producer.ProduceAsync(Topic, messageToPush, cancellationToken);
        }
    }
}
