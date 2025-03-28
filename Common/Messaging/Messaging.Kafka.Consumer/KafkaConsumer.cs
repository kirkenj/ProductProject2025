using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using AutoMapper;

namespace Messaging.Kafka.Consumer
{
    public class KafkaConsumer<TMessage, TCommand, TOptions> : BackgroundService where TCommand : IRequest where TOptions : KafkaConsumerSettings
    {
        private static string Topic => typeof(TMessage).FullName!;

        private readonly IConsumer<string, TMessage> _consumer;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<KafkaConsumer<TMessage, TCommand, TOptions>> _logger;

        public KafkaConsumer(IOptions<KafkaConsumerSettings> options,
            IMediator mediator,
            IMapper mapper,
            ILogger<KafkaConsumer<TMessage, TCommand, TOptions>> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;

            _consumer = new ConsumerBuilder<string, TMessage>(new ConsumerConfig()
            {
                BootstrapServers = options.Value.BootStrapServers,
                GroupId = options.Value.GroupId
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
                    var command = _mapper.Map<TCommand>(result.Message.Value);
                    await _mediator.Send(command, stoppingToken);
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
