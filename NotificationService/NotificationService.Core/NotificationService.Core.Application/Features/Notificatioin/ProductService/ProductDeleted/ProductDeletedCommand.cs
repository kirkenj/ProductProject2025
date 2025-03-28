using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductDeleted
{
    public class ProductDeletedCommand : Messaging.Messages.ProductService.ProductDeleted, IRequest
    {
    }
}
