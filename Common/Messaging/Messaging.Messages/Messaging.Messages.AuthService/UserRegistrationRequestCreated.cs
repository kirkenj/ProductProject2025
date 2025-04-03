namespace Messaging.Messages.AuthService
{
    public class UserRegistrationRequestCreated
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
