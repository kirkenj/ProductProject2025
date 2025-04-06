using System.Text.Json;
using NotificationService.Core.Application.Contracts.Infrastructure;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Models.TargetServicesModels;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductDeleted.NotificationHandlers
{
    internal class ProductDeletedNotificationSignalRHanler : SignalRNotificationHandler<ProductDeletedNotification>
    {
        public ProductDeletedNotificationSignalRHanler(ISignalRNotificationService signalRNotificationService) : base(signalRNotificationService)
        {
        }

        protected override Task<SignalRNotification> GetNotificatoinForTargetService(ProductDeletedNotification notification, CancellationToken cancellationToken) => Task.FromResult(new SignalRNotification
        {
            UserId = notification.UserId,
            Body = JsonSerializer.Serialize(notification),
            Subject = Resources.ProductCreatedNotificationType
        });
    }
}

