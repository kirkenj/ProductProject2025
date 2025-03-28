namespace Messaging.Messages.AuthService
{
    public class UserRegistrationRequestCreated
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
