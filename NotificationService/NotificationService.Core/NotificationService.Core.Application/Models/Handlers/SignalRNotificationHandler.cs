using System.Text.Json;
using MediatR;
using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Infrastructure;
using NotificationService.Core.Application.Models.TargetServicesModels;

namespace NotificationService.Core.Application.Models.Handlers
{
    public abstract class SignalRNotificationHandler<T> : INotificationHandler<T> where T : IMediatRSendableNotification
    {
        private readonly ISignalRNotificationService _signalRNotificationService;

        public SignalRNotificationHandler(ISignalRNotificationService signalRNotificationService)
        {
            _signalRNotificationService = signalRNotificationService;
        }

        public async Task Handle(T notification, CancellationToken cancellationToken)
        {
            var notificationToSend = await GetNotificatoinForTargetService(notification, cancellationToken);
            await _signalRNotificationService.Send(notificationToSend, cancellationToken);
        }

        protected virtual Task<SignalRNotification> GetNotificatoinForTargetService(T notification, CancellationToken cancellationToken) => Task.FromResult(new SignalRNotification
        {
            Subject = typeof(T).Name,
            Body = JsonSerializer.Serialize(notification, typeof(T)),
            UserId = notification.UserId,
        });
    }
}
