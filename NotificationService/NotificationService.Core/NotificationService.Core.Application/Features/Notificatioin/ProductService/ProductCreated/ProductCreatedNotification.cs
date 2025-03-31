using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductCreated
{
    public class ProductCreatedNotification : Messaging.Messages.ProductService.ProductCreated, INotification
    {
    }
}
