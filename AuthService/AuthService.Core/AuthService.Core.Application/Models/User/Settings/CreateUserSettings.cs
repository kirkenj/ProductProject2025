namespace AuthService.Core.Application.Models.User.Settings
{
    public class CreateUserSettings
    {
        public int DefaultRoleID { get; set; }
        public string KeyForRegistrationCachingFormat { get; set; } = null!;
        public int ConfirmationTimeout { get; set; }
    }
}
