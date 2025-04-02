using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Models.Handlers
{
    public abstract class EmailNotificationHandler<T> : INotificationHandler<T> where T : IMediatRSendableNotification
    {
        private readonly IEmailSender _emailSender;

        public EmailNotificationHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Handle(T notification, CancellationToken cancellationToken)
        {
            var email = await GetEmailAsync(notification, cancellationToken);
            await _emailSender.SendEmailAsync(email, cancellationToken);
        }

        protected abstract Task<Email> GetEmailAsync(T notification, CancellationToken cancellationToken);
    }
}
