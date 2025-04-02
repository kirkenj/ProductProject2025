using Microsoft.AspNetCore.SignalR;
using NotificationService.Core.Application.Contracts.Infrastructure;
using NotificationService.Core.Application.Models.TargetServicesModels;

namespace NotificationService.Api.NotificationApi.Services
{
    public class SignalRNotificationService<T> : ISignalRNotificationService where T : Hub
    {
        private readonly IHubContext<T> _hubContext;

        public SignalRNotificationService(IHubContext<T> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Send(SignalRNotification message, CancellationToken cancellationToken = default)
        {
            IClientProxy targetConnections = string.IsNullOrWhiteSpace(message.UserId) ?
                _hubContext.Clients.All :
                _hubContext.Clients.Group(message.UserId);

            await targetConnections.SendAsync("Receive", message, cancellationToken);
        }
    }
}
