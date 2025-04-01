﻿using NotificationService.Core.Application.Contracts.Application;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models;

namespace NotificationService.Core.Application.Features.Notificatioin.ProductService.ProductProducerUpdated
{
    internal class ProductProducerUpdatedNotificationRequestHandler : NotificationRequestHandler<ProductProducerUpdatedNotificationRequest>
    {
        public ProductProducerUpdatedNotificationRequestHandler(INotificationRepository repository) : base(repository)
        {
        }

        protected override IEnumerable<IMediatRSendableNotification> GetNotifications(ProductProducerUpdatedNotificationRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
