using MediatR;

namespace NotificationService.Api.Consumers.ConsumerMediatRActions
{
    public static class ConsumerMediatRAction
    {
        public static Func<IMediator, IRequest<IEnumerable<INotification>>, CancellationToken, Task> GetAndPublishNotificationFromRequest = async (mediator, request, cancellationToken) =>
        {
            var requestResult = await mediator.Send(request, cancellationToken);
            var notificationTasks = requestResult.Select(notification => mediator.Publish(notification, cancellationToken));
            await Task.WhenAll(notificationTasks);
        };
    }
}
