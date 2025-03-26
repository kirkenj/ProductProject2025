namespace Messaging.Kafka.Consumer
{
    public class KafkaConsumerSettings : KafkaSettings
    {
        public string GroupId { get; set; } = string.Empty;
    }
}
