using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Api.NotificationApi.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"NEW CONNECTION: ConnectionId:{Context.ConnectionId}; UserId:{Context.UserIdentifier}");
            if (!string.IsNullOrEmpty(Context.UserIdentifier))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
                _logger.LogInformation($"ConnectionId:{Context.ConnectionId}; was ADDED TO GROUP: {Context.UserIdentifier}");
            }

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"CONNECTION LOST: Connection:{Context.ConnectionId}; UserId:{Context.UserIdentifier}");
            if (!string.IsNullOrEmpty(Context.UserIdentifier))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
                _logger.LogInformation($"ConnectionId:{Context.ConnectionId}; was REMOVED FROM GROUP: {Context.UserIdentifier}");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
