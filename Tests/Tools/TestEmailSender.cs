using EmailSender.Models;
using Microsoft.Extensions.Logging;

namespace Tools
{
    public class TestEmailSender : ConsoleEmailSender
    {
        public Email? LastSentEmail => Emails.LastOrDefault();

        public readonly List<Email> Emails = new();

        public TestEmailSender(ILogger<EmailSender.Models.EmailSender> logger) : base(logger)
        {
        }

        public override async Task<bool> SendEmailAsync(Email email)
        {
            var emailSent = await base.SendEmailAsync(email);
            if (emailSent)
            {
                Emails.Add(email);
            }

            return emailSent;
        }
    }
}
