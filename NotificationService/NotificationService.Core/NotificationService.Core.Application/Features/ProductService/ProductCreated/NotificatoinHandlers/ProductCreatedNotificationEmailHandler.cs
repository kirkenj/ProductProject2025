using EmailSender.Contracts;
using EmailSender.Models;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductCreated.NotificatoinHandlers
{
    public class ProductCreatedNotificationEmailHandler : EmailNotificationHandler<ProductCreatedNotification>
    {
        public ProductCreatedNotificationEmailHandler(IEmailSender emailSender) : base(emailSender)
        {
        }

        protected override Task<Email> GetEmailAsync(ProductCreatedNotification notification, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Email
            {
                Subject = Resources.ProductCreatedNotificationType,
                Body = string.Format(Resources.ProductCreatedEmailNotificationBodyFormat, notification.ProductDto.Name, notification.ProductId),
                To = notification.UserDto.Email
            });
        }
    }
}
