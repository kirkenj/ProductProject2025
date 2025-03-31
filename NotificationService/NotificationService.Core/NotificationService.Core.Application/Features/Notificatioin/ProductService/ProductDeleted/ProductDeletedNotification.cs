using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductDeleted
{
    public class ProductDeletedNotification : Messaging.Messages.ProductService.ProductDeleted, INotification
    {
    }
}
