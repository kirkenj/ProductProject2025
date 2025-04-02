using EmailSender.Contracts;
using EmailSender.Models;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated.NotificationHandlers
{
    public class ProductProducerUpdatedNotificationForOldOwnerEmailHandler : EmailNotificationHandler<ProductProducerUpdatedNotificationForOldOwner>
    {
        public ProductProducerUpdatedNotificationForOldOwnerEmailHandler(IEmailSender emailSender) : base(emailSender)
        {
        }

        protected override Task<Email> GetEmailAsync(ProductProducerUpdatedNotificationForOldOwner notification, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Email
            {
                Subject = Resources.YouWereGivenAProductSubject,
                To = notification.UserDto.Email,
                Body = string.Format(Resources.YourProductWasGivenToOtherUserEmailBodyFormat, notification.ProductDto.Name, notification.ProductId)
            });
        }
    }
}
