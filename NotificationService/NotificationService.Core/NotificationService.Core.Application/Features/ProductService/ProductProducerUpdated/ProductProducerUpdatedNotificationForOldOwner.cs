using System.Text.Json.Serialization;
using Clients.Adapters.AuthClient.Contracts;
using Clients.Adapters.ProductClient.Contracts;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedNotificationForOldOwner : IMediatRSendableNotification
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        [JsonIgnore]
        public AuthClientUser? UserDto { get; set; } = null!;
        [JsonIgnore]
        public ProductDto? ProductDto { get; set; } = null!;
    }
}
