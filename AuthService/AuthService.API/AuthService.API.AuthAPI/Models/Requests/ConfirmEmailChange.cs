namespace AuthService.API.AuthAPI.Models.Requests
{
    public class ConfirmEmailChange
    {
        public string OtpToNewEmail { get; set; } = string.Empty;
        public string OtpToOldEmail { get; set; } = string.Empty;
    }
}
