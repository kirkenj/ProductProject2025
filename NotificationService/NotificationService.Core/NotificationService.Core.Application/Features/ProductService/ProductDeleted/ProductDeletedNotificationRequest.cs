using MediatR;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Features.ProductService.ProductDeleted
{
    public class ProductDeletedNotificationRequest : Messaging.Messages.ProductService.ProductDeleted, IRequest<IEnumerable<IMediatRSendableNotification>>
    {
    }
}
