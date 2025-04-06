using EmailSender.Contracts;
using EmailSender.Models;
using Microsoft.Extensions.Logging;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductCreated.NotificatoinHandlers
{
    public class ProductCreatedNotificationEmailHandler : EmailNotificationHandler<ProductCreatedNotification>
    {
        public ProductCreatedNotificationEmailHandler(IEmailSender emailSender, ILogger<ProductCreatedNotificationEmailHandler> logger) : base(emailSender, logger)
        {
        }

        protected override Task<Email?> GetEmailAsync(ProductCreatedNotification notification, CancellationToken cancellationToken)
        {
            Email? emailToReturn = null;

            if (notification.UserDto != null && notification.ProductDto != null)
            {
                emailToReturn = new Email
                {
                    Subject = Resources.ProductCreatedNotificationType,
                    Body = string.Format(Resources.ProductCreatedEmailNotificationBodyFormat, notification.ProductDto.Name, notification.ProductId),
                    To = notification.UserDto.Email
                };
            }
            else
            {
                var nullArgs = new string[]
                {
                    notification.ProductDto == null ? nameof(notification.ProductDto) : string.Empty,
                    notification.UserDto == null ? nameof(notification.UserDto) : string.Empty,
                };

                _logger.LogWarning("One of the needed arguments is null: {args}", string.Join(", ", nullArgs));
            }


            return Task.FromResult(emailToReturn);
        }
    }
}
