using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Messaging.Kafka.Consumer
{
    public class KafkaConsumer<TMessage> : BackgroundService
    {
        private static string Topic => typeof(TMessage).FullName!;

        private readonly IConsumer<string, TMessage> _consumer;
        private readonly IMessageHandler<Message<string, TMessage>> _messageHandler;
        private readonly ILogger<KafkaConsumer<TMessage>> _logger;

        public KafkaConsumer(IOptions<KafkaConsumerSettings> options,
            IMessageHandler<Message<string, TMessage>> messageHandler,
            ILogger<KafkaConsumer<TMessage>> logger)
        {
            _messageHandler = messageHandler;
            _logger = logger;
            var config = new ConsumerConfig
            {
                BootstrapServers = options.Value.BootStrapServers,
                GroupId = options.Value.GroupId
            };

            _consumer = new ConsumerBuilder<string, TMessage>(config)
                .SetValueDeserializer(new KafkaJsonDeserializer<TMessage>())
                .Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
        }

        private async Task ConsumeAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(Topic);
            try
            {
                _logger.LogInformation($"Consume started {Topic}");
                while (!stoppingToken.IsCancellationRequested)
                {   
                    var result = _consumer.Consume(stoppingToken);
                    await _messageHandler.HandleAsync(result.Message, stoppingToken);
                }

                _consumer.Unsubscribe();
                _consumer.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _consumer.Unsubscribe();
            _consumer.Close();
            return base.StopAsync(cancellationToken);
        }
    }
}
