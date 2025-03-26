using Confluent.Kafka;

namespace Messaging.Kafka.Producer
{
    public interface IKafkaProducer<TMessage> : IDisposable
    {
        Task<DeliveryResult<string, TMessage>> ProduceAsync(string key, TMessage message, CancellationToken cancellationToken);
    }
}
