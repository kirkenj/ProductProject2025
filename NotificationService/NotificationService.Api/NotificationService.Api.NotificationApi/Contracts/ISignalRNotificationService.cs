using NotificationService.Api.NotificationApi.Models;

namespace NotificationService.Api.NotificationApi.Contracts
{
    public interface ISignalRNotificationService
    {
        public Task Send(SignalRNotification message, Guid targetUserId);
    }
}
