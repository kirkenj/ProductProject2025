using EmailSender.Contracts;
using EmailSender.Models;
using Microsoft.Extensions.Logging;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated.NotificationHandlers
{
    public class ProductProducerUpdatedNotificationForOldOwnerEmailHandler : EmailNotificationHandler<ProductProducerUpdatedNotificationForOldOwner>
    {
        public ProductProducerUpdatedNotificationForOldOwnerEmailHandler(IEmailSender emailSender, ILogger<ProductProducerUpdatedNotificationForOldOwnerEmailHandler> logger) : base(emailSender, logger)
        {
        }

        protected override Task<Email?> GetEmailAsync(ProductProducerUpdatedNotificationForOldOwner notification, CancellationToken cancellationToken)
        {
            Email? emailToReturn = null;

            if (notification.UserDto != null && notification.ProductDto != null)
            {
                emailToReturn = new Email
                {
                    Subject = Resources.YouWereGivenAProductSubject,
                    To = notification.UserDto.Email,
                    Body = string.Format(Resources.YourProductWasGivenToOtherUserEmailBodyFormat, notification.ProductDto.Name, notification.ProductId)
                };
            }
            else
            {
                var nullArgs = new string[]
                {
                    notification.UserDto == null ? nameof(notification.UserDto) : string.Empty,
                    notification.ProductDto == null ? nameof(notification.ProductDto) : string.Empty,
                };

                _logger.LogWarning("One of the needed arguments is null: {args}", string.Join(", ", nullArgs));
            }

            return Task.FromResult(emailToReturn);
        }
    }
}