using System.Text.Json.Serialization;
using Clients.Adapters.AuthClient.Contracts;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Features.ProductService.ProductDeleted
{
    public class ProductDeletedNotification : IMediatRSendableNotification
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        [JsonIgnore]
        public AuthClientUser? UserDto { get; set; } = null!;
        public string ProductName { get; set; } = string.Empty;
    }
}
