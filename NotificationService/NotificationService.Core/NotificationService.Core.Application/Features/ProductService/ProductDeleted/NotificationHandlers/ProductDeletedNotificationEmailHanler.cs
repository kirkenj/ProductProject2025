using System.Text.Json;
using EmailSender.Contracts;
using EmailSender.Models;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductDeleted.NotificationHandlers
{
    public class ProductDeletedNotificationEmailHanler : EmailNotificationHandler<ProductDeletedNotification>
    {
        public ProductDeletedNotificationEmailHanler(IEmailSender emailSender) : base(emailSender)
        {
        }

        protected override Task<Email> GetEmailAsync(ProductDeletedNotification notification, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Email
            {
                To = notification.UserDto.Email,
                Subject = Resources.ProductDeletedNotificationType,
                Body = JsonSerializer.Serialize(notification)
            });
        }
    }
}
