using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedCommand : Messaging.Messages.ProductService.ProductProducerUpdated, IRequest
    {
    }
}
