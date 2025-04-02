using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models.Handlers;

namespace NotificationService.Core.Application.Features.ProductService.ProductCreated
{
    public class ProductDeletedNotificationRequestHandler : NotificationRequestHandler<ProductCreatedNotificationRequest>
    {
        public ProductDeletedNotificationRequestHandler(INotificationRepository repository) : base(repository) { }

        protected override IEnumerable<IMediatRSendableNotification> GetNotifications(ProductCreatedNotificationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
