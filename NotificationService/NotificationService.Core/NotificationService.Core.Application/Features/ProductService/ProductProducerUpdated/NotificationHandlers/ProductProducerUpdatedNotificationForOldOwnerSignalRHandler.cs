using System.Text.Json;
using NotificationService.Core.Application.Contracts.Infrastructure;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Models.TargetServicesModels;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated.NotificationHandlers
{
    public class ProductProducerUpdatedNotificationForOldOwnerSignalRHandler : SignalRNotificationHandler<ProductProducerUpdatedNotificationForOldOwner>
    {
        public ProductProducerUpdatedNotificationForOldOwnerSignalRHandler(ISignalRNotificationService signalRNotificationService) : base(signalRNotificationService)
        {
        }

        protected override Task<SignalRNotification> GetNotificatoinForTargetService(ProductProducerUpdatedNotificationForOldOwner notification, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SignalRNotification
            {
                UserId = notification.UserId,
                Subject = Resources.YouWereGivenAProductSubject,
                Body = JsonSerializer.Serialize(notification)
            });
        }
    }
}
