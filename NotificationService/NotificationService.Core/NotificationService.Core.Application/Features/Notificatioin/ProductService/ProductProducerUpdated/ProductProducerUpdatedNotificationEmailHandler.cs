using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedNotificationEmailHandler : INotificationHandler<ProductProducerUpdatedNotification>
    {
        public Task Handle(ProductProducerUpdatedNotification request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
