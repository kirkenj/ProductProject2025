using EmailSender.Contracts;
using EmailSender.Models;
using Microsoft.Extensions.Logging;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedNotificationForNewOwnerEmailHandler : EmailNotificationHandler<ProductProducerUpdatedNotificationForNewOwner>
    {
        public ProductProducerUpdatedNotificationForNewOwnerEmailHandler(IEmailSender emailSender, ILogger<ProductProducerUpdatedNotificationForNewOwnerEmailHandler> logger) : base(emailSender, logger)
        {
        }

        protected override Task<Email?> GetEmailAsync(ProductProducerUpdatedNotificationForNewOwner notification, CancellationToken cancellationToken)
        {
            Email? emailToReturn = null;

            if (notification.UserDto != null && notification.ProductDto != null)
            {
                emailToReturn = new Email
                {
                    To = notification.UserDto.Email,
                    Subject = Resources.YourProductWasGivenToOtherUserSubject,
                    Body = string.Format(Resources.YouWereGivenAProductSubject, notification.ProductDto.Name, notification.ProductId)
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
