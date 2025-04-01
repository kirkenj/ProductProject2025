using MediatR;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedNotificationRequest : Messaging.Messages.ProductService.ProductProducerUpdated, IRequest<IEnumerable<IMediatRSendableNotification>>
    {
    }
}
