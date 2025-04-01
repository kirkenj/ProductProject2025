using EmailSender.Contracts;
using EmailSender.Models;
using Microsoft.Extensions.Logging;

namespace EmailSender.Services
{
    public class ConsoleEmailSender : IEmailSender
    {
        private ILogger<ConsoleEmailSender> Logger { get; }

        public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
        {
            Logger = logger;
        }

        public virtual async Task<bool> SendEmailAsync(Email email, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Message to {To}\nSubject: {Subject}.\nBody: {Body}", email.To, email.Subject, email.Body);
            await Task.Delay(30, cancellationToken);
            return true;
        }
    }
}
