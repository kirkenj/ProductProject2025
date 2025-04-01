using MediatR;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductCreated
{
    public class ProductCreatedNotificationRequest : Messaging.Messages.ProductService.ProductCreated, IRequest<IEnumerable<IMediatRSendableNotification>>
    {
    }
}
