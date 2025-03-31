using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Api.NotificationApi.Services
{
    public class CustomUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Sid)?.Value;
        }
    }
}
