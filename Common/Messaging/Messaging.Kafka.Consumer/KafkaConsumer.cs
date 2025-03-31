using AutoMapper;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messaging.Kafka.Consumer
{
    public class KafkaConsumer<TMessage, TCommandOrNotification> : BackgroundService
    {
        private static string Topic => typeof(TMessage).FullName!;

        private readonly IConsumer<string, TMessage> _consumer;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<KafkaConsumer<TMessage, TCommandOrNotification>> _logger;
        private readonly Func<IMediator, TCommandOrNotification, CancellationToken, Task> _mediatorAction;

        public KafkaConsumer(KafkaSettings kafkaSettings,
            KafkaConsumerSettings consumerSettings,
            IMediator mediator,
            IMapper mapper,
            ILogger<KafkaConsumer<TMessage, TCommandOrNotification>> logger,
            Func<IMediator, TCommandOrNotification, CancellationToken, Task> mediatorAction)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _mediatorAction = mediatorAction;

            SeedTopic(kafkaSettings).Wait();

            _consumer = new ConsumerBuilder<string, TMessage>(new ConsumerConfig()
            {
                BootstrapServers = kafkaSettings.BootStrapServers,
                GroupId = consumerSettings.GroupId,
            })
            .SetValueDeserializer(new KafkaJsonDeserializer<TMessage>())
            .Build();
        }

        private async Task SeedTopic(KafkaSettings kafkaSettings)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = kafkaSettings.BootStrapServers,
            }).Build();

            try
            {
                await adminClient.CreateTopicsAsync([new TopicSpecification
                {
                    Name = Topic
                }]);
                _logger.LogInformation("Topic \'{topic}\' seeded", Topic);
            }
            catch (CreateTopicsException ex) when (ex.Results.All(r => !r.Error.IsError || r.Error.Reason == $"Topic '{r.Topic}' already exists."))
            {
                _logger.LogInformation("Topic \'{topic}\' already exists", Topic);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
        }

        private async Task ConsumeAsync(CancellationToken stoppingToken)
        {
            try
            {
                _consumer.Subscribe(Topic);
                _logger.LogInformation("Consume started {Topic}", Topic);
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(stoppingToken);
                    _logger.LogInformation("Got message: {message}", result);
                    var command = _mapper.Map<TCommandOrNotification>(result.Message.Value);
                    try
                    {
                        await _mediatorAction(_mediator, command, stoppingToken);
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
