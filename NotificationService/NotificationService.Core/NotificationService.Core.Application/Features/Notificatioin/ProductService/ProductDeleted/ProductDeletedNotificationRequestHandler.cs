using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductDeleted
{
    public class ProductDeletedNotificationRequestHandler : NotificationRequestHandler<ProductDeletedNotificationRequest>
    {
        public ProductDeletedNotificationRequestHandler(INotificationRepository repository) : base(repository)
        {
        }

        protected override IEnumerable<IMediatRSendableNotification> GetNotifications(ProductDeletedNotificationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
