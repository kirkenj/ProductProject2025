using System.Text.Json;
using MediatR;
using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Domain.Models;

namespace NotificationService.Core.Application.Models
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
            var notifications = GetNotifications(request);

            var addTasks = notifications.Select(notification =>
            {
                var notificationToAdd = new Notification
                {
                    NotificationJson = JsonSerializer.Serialize(notification),
                    NotificationType = notification.GetType().FullName!,
                    UserId = notification.UserId,
                };

                return _repository.AddAsync(notificationToAdd);
            });

            await Task.WhenAll(addTasks);

            return notifications;
        }

        protected abstract IEnumerable<IMediatRSendableNotification> GetNotifications(TRequest request);
    }
}