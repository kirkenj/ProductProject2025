using Microsoft.AspNetCore.SignalR;
using NotificationService.Api.NotificationApi.Contracts;
using NotificationService.Api.NotificationApi.Models;

namespace NotificationService.Api.NotificationApi.Services
{
    public class SignalRNotificationService<T> : ISignalRNotificationService where T : Hub
    {
        private readonly IHubContext<T> _hubContext;

        public SignalRNotificationService(IHubContext<T> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Send(SignalRNotification message, Guid targetUserId)
        {
            IClientProxy targetConnections = targetUserId == default ?
                _hubContext.Clients.All :
                _hubContext.Clients.Group(targetUserId.ToString());

            await targetConnections.SendAsync("Receive", message, CancellationToken.None);
        }
    }
}
