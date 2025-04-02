using System.Runtime.Serialization;
using Clients.AuthApi;
using Clients.ProductService.Clients.ProductServiceClient;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedNotificationForNewOwner : IMediatRSendableNotification
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        [IgnoreDataMember]
        public UserDto UserDto { get; set; } = null!;
        [IgnoreDataMember]
        public ProductDto ProductDto { get; set; } = null!;
    }
}
