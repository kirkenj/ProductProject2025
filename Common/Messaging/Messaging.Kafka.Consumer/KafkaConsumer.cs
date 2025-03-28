using AutoMapper;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer
{
    public class KafkaConsumer<TMessage, TCommand> : BackgroundService where TCommand : IRequest
    {
        private static string Topic => typeof(TMessage).FullName!;

        private readonly IConsumer<string, TMessage> _consumer;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<KafkaConsumer<TMessage, TCommand>> _logger;

        public KafkaConsumer(KafkaSettings kafkaSettings,
            KafkaConsumerSettings consumerSettings,
            IMediator mediator,
            IMapper mapper,
            ILogger<KafkaConsumer<TMessage, TCommand>> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;

            _consumer = new ConsumerBuilder<string, TMessage>(new ConsumerConfig()
            {
                BootstrapServers = kafkaSettings.BootStrapServers,
                GroupId = consumerSettings.GroupId, 
                AllowAutoCreateTopics = true
            })
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
                _logger.LogInformation("Consume started {Topic}", Topic);
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(stoppingToken);
                    _logger.LogInformation("Got message: {message}", result);
                    var command = _mapper.Map<TCommand>(result.Message.Value);
                    try
                    {
                        await _mediator.Send(command, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Handler for message {message} produced exception: {ex}", result, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            finally
            {
                CloseConsumer();
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            CloseConsumer();
            return base.StopAsync(cancellationToken);
        }

        private void CloseConsumer()
        {
            _consumer.Unsubscribe();
            _consumer.Close();
            _logger.LogInformation("Consume stoped {Topic}", Topic);
        }
    }
}
