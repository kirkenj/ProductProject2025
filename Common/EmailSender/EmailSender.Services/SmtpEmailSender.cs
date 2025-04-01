using EmailSender.Contracts;
using EmailSender.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailSender.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private SmtpEmailSenderSettings Settings { get; }
        private ILogger<SmtpEmailSender> Logger { get; }

        public SmtpEmailSender(IOptions<SmtpEmailSenderSettings> emailSettings, ILogger<SmtpEmailSender> logger) : this(emailSettings.Value, logger)
        {
        }

        public SmtpEmailSender(SmtpEmailSenderSettings emailSettings, ILogger<SmtpEmailSender> logger)
        {
            Settings = emailSettings;
            Logger = logger;
        }

        public async Task<bool> SendEmailAsync(Email email, CancellationToken cancellationToken)
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
                await client.ConnectAsync(Settings.ApiAdress, Settings.ApiPort, true, cancellationToken);
                await client.AuthenticateAsync(Settings.ApiLogin, Settings.ApiPassword, cancellationToken);
                await client.SendAsync(emailMessage, cancellationToken);

                _ = Task.Run(() => client.DisconnectAsync(true), cancellationToken);

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
