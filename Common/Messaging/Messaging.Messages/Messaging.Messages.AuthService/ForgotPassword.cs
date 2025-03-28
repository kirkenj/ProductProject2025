namespace Messaging.Messages.AuthService
{
    public class ForgotPassword
    {
        public Guid UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
