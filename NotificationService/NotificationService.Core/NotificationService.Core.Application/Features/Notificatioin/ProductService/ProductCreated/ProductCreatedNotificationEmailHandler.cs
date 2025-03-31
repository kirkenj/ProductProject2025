using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductCreated
{
    public class ProductCreatedNotificationEmailHandler : INotificationHandler<ProductCreatedNotification>
    {
        public Task Handle(ProductCreatedNotification request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
