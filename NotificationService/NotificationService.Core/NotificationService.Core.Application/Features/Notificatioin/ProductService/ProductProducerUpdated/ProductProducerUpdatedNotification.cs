using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedNotification : Messaging.Messages.ProductService.ProductProducerUpdated, INotification
    {
    }
}
