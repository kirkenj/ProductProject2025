using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductDeleted
{
    public class ProductDeletedNotificationEmailHandler : INotificationHandler<ProductDeletedNotification>
    {
        public Task Handle(ProductDeletedNotification request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
