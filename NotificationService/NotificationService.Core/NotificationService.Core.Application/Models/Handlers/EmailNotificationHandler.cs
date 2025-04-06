using EmailSender.Contracts;
using EmailSender.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using NotificationService.Core.Application.Contracts.Application;

namespace NotificationService.Core.Application.Models.Handlers
{
    public abstract class EmailNotificationHandler<T> : INotificationHandler<T> where T : IMediatRSendableNotification
    {
        private readonly IEmailSender _emailSender;
        protected readonly ILogger<EmailNotificationHandler<T>> _logger;

        public EmailNotificationHandler(IEmailSender emailSender, ILogger<EmailNotificationHandler<T>> logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task Handle(T notification, CancellationToken cancellationToken)
        {
            var email = await GetEmailAsync(notification, cancellationToken);
            if (email == null) 
            {
                _logger.LogWarning("Could not get email from notification. Sending canceled.");
                return;
            }
            await _emailSender.SendEmailAsync(email, cancellationToken);
        }

        protected abstract Task<Email?> GetEmailAsync(T notification, CancellationToken cancellationToken);
    }
}
