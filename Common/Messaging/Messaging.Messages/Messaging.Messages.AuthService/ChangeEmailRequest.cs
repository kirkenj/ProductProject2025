namespace Messaging.Messages.AuthService
{
    public class ChangeEmailRequest
    {
        public Guid UserId { get; set; }
        public string OtpToOldEmail {  get; set; } = string.Empty;
        public string NewEmail {  get; set; } = string.Empty;
        public string OtpToNewEmail {  get; set; } = string.Empty;
    }
}
