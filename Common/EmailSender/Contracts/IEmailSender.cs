using EmailSender.Models;

namespace EmailSender.Contracts
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(Email email);
    }
}
