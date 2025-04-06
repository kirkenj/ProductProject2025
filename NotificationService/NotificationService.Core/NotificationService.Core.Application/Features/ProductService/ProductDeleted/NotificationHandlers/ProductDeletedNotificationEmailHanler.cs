using EmailSender.Contracts;
using EmailSender.Models;
using Microsoft.Extensions.Logging;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductDeleted.NotificationHandlers
{
    public class ProductDeletedNotificationEmailHanler : EmailNotificationHandler<ProductDeletedNotification>
    {
        public ProductDeletedNotificationEmailHanler(IEmailSender emailSender, ILogger<ProductDeletedNotificationEmailHanler> logger) : base(emailSender, logger)
        {
        }

        protected override Task<Email?> GetEmailAsync(ProductDeletedNotification notification, CancellationToken cancellationToken)
        {
            Email? emailToReturn = null;

            if (notification.UserDto != null)
            {
                emailToReturn = new Email
                {
                    To = notification.UserDto.Email,
                    Subject = Resources.ProductDeletedNotificationType,
                    Body = $"Product {notification.ProductName} ({notification.ProductId}) was removed."
                };
            }
            else
            {
                var nullArgs = new string[]
                {
                    notification.UserDto == null ? nameof(notification.UserDto) : string.Empty,
                };

                _logger.LogWarning("One of the needed arguments is null: {args}", string.Join(", ", nullArgs));
            }

            return Task.FromResult(emailToReturn);
        }
    }
}
