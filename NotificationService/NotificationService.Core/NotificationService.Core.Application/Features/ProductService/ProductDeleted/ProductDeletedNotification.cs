using System.Runtime.Serialization;
using Clients.AuthApi;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Features.ProductService.ProductDeleted
{
    public class ProductDeletedNotification : IMediatRSendableNotification
    {
        public string UserId { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        [IgnoreDataMember]
        public UserDto UserDto { get; set; } = null!;
    }
}
