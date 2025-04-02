using System.Text.Json;
using NotificationService.Core.Application.Contracts.Infrastructure;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Models.TargetServicesModels;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductCreated.NotificatoinHandlers
{
    public class ProductCreatedNotificationSignalRHandler : SignalRNotificationHandler<ProductCreatedNotification>
    {
        public ProductCreatedNotificationSignalRHandler(ISignalRNotificationService signalRNotificationService) : base(signalRNotificationService)
        {
        }

        protected override Task<SignalRNotification> GetNotificatoinForTargetService(ProductCreatedNotification notification, CancellationToken cancellationToken) => Task.FromResult(new SignalRNotification
        {
            Subject = Resources.ProductCreatedNotificationType,
            Body = JsonSerializer.Serialize(notification),
            UserId = notification.UserId,
        });
    }
}