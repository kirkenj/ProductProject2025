using MediatR;

namespace Messaging.Kafka.Consumer
{
    public class MediatorPublicationStrategies
    {
        public static async Task AsRequest<TCommandOrNotification>(IMediator mediator, TCommandOrNotification request, CancellationToken cancellationToken) where TCommandOrNotification : IRequest
        {
            await mediator.Send(request, cancellationToken);
        }

        public static async Task AsNotification<TCommandOrNotification>(IMediator mediator, TCommandOrNotification request, CancellationToken cancellationToken) where TCommandOrNotification : INotification
        {
            await mediator.Publish(request, cancellationToken);
        }
    }
}
