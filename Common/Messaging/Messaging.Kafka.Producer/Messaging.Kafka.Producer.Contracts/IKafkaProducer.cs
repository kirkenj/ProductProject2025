using Confluent.Kafka;

namespace Messaging.Kafka.Producer.Contracts
{
    public interface IKafkaProducer<TMessage> : IDisposable
    {
        Task<DeliveryResult<string, TMessage>> ProduceAsync(TMessage message, CancellationToken cancellationToken);
    }
}
