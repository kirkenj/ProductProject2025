using System.Text.Json;
using MediatR;
using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Domain.Models;

namespace NotificationService.Core.Application.Models.Handlers
{
    public abstract class NotificationRequestHandler<TRequest> : IRequestHandler<TRequest, IEnumerable<INotification>> where TRequest : IRequest<IEnumerable<IMediatRSendableNotification>>
    {
        private readonly INotificationRepository _repository;

        protected NotificationRequestHandler(INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<INotification>> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var notifications = await GetNotificationsAsync(request);

            var addTasks = notifications.Select(notification =>
            {
                var notificationToAdd = new Notification
                {
                    NotificationJson = JsonSerializer.Serialize(notification, notification.GetType()),
                    NotificationType = notification.GetType().FullName!,
                    UserId = notification.UserId,
                    Date = DateTime.Now, 
                };

                return _repository.AddAsync(notificationToAdd);
            });

            await Task.WhenAll(addTasks);

            return notifications;
        }

        protected abstract Task<IEnumerable<IMediatRSendableNotification>> GetNotificationsAsync(TRequest request);
    }
}