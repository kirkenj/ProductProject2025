using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Producer
{
    public class KafkaProducer<TMessage> : IKafkaProducer<TMessage>
    {
        private readonly IProducer<string, TMessage> _producer;
        private readonly string? _topic;

        public KafkaProducer(IOptions<KafkaSettings> options)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = options.Value.BootStrapServers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, TMessage>(config)
                .SetValueSerializer(new KafkaJsonSerializer<TMessage>())
                .Build();
        
            _topic = typeof(TMessage).FullName;
        }

        public void Dispose() => _producer.Dispose();

        public async Task<DeliveryResult<string, TMessage>> ProduceAsync(string key, TMessage message, CancellationToken cancellationToken)
        {
            var messageToPush = new Message<string, TMessage>()
            {
                Key = key,
                Value = message
            };

            return await _producer.ProduceAsync(_topic, messageToPush, cancellationToken);
        }
    }
}
