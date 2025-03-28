using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductCreated
{
    public class ProductCreatedCommand : Messaging.Messages.ProductService.ProductCreated, IRequest
    {
    }
}
