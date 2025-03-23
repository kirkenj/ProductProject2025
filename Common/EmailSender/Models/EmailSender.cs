using EmailSender.Contracts;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailSender.Models
{
    public class EmailSender : IEmailSender
    {
        private EmailSettings Settings { get; }
        private ILogger<EmailSender> Logger { get; }

        public EmailSender(IOptions<EmailSettings> emailSettings, ILogger<EmailSender> logger) : this(emailSettings.Value, logger)
        {
        }
        public EmailSender(EmailSettings emailSettings, ILogger<EmailSender> logger)
        {
            Settings = emailSettings;
            Logger = logger;
        }

        public async Task<bool> SendEmailAsync(Email email)
        {
            Logger.LogInformation($"Sending email to {email.To}");
            using var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(Settings.FromName, Settings.ApiLogin));
            emailMessage.To.Add(new MailboxAddress("User", email.To));
            emailMessage.Subject = email.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = email.Body
            };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(Settings.ApiAdress, Settings.ApiPort, true);
                await client.AuthenticateAsync(Settings.ApiLogin, Settings.ApiPassword);
                await client.SendAsync(emailMessage);

                _ = Task.Run(() => client.DisconnectAsync(true));

                Logger.LogInformation($"Message to {email.To}. Success");
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogInformation($"Message to {email.To}. Fail: {ex}");
                return false;
            }
        }
    }
}
