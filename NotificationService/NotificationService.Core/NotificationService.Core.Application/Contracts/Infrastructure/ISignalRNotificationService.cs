using NotificationService.Core.Application.Models.TargetServicesModels;

namespace NotificationService.Core.Application.Contracts.Infrastructure
{
    public interface ISignalRNotificationService
    {
        public Task Send(SignalRNotification message, CancellationToken cancellationToken = default);
    }
}
