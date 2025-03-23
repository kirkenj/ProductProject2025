using EmailSender.Contracts;
using Microsoft.Extensions.Logging;

namespace EmailSender.Models
{
    public class ConsoleEmailSender : IEmailSender
    {
        private ILogger<EmailSender> Logger { get; }

        public ConsoleEmailSender(ILogger<EmailSender> logger)
        {
            Logger = logger;
        }

        public virtual Task<bool> SendEmailAsync(Email email)
        {
            Logger.LogInformation($"Sending email to {email.To}");

            Logger.LogInformation($"Message to {email.To}\nSubject: {email.Subject}.\nBody: {email.Body}");
            Thread.Sleep(80);
            return Task.FromResult(true);
        }
    }
}
