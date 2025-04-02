using EmailSender.Contracts;
using EmailSender.Models;
using NotificationService.Core.Application.Models.Handlers;
using NotificationService.Core.Application.Properties;

namespace NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated
{
    public class ProductProducerUpdatedNotificationForNewOwnerEmailHandler : EmailNotificationHandler<ProductProducerUpdatedNotificationForNewOwner>
    {
        public ProductProducerUpdatedNotificationForNewOwnerEmailHandler(IEmailSender emailSender) : base(emailSender)
        {
        }

        protected override Task<Email> GetEmailAsync(ProductProducerUpdatedNotificationForNewOwner notification, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Email
            {
                To = notification.UserDto.Email,
                Subject = Resources.YourProductWasGivenToOtherUserSubject,
                Body = string.Format(Resources.YouWereGivenAProductSubject, notification.ProductDto.Name, notification.ProductId)
            });
        }
    }
}
