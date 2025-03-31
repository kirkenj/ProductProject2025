using Microsoft.AspNetCore.SignalR;
using NotificationService.Api.NotificationApi.Contracts;

namespace NotificationService.Api.NotificationApi.Services
{
    public class SignalRNotificationService<T> : ISignalRNotificationService where T : Hub
    {
        private readonly IHubContext<T> _hubContext;

        public SignalRNotificationService(IHubContext<T> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Send(Models.Message message, string senderId)
        {
            IClientProxy targetConnections = message.UserId == default ?
                _hubContext.Clients.All :
                _hubContext.Clients.Group(message.UserId.ToString());

            await targetConnections.SendAsync("Receive", message.Body, senderId, CancellationToken.None);
        }
    }
}
