namespace AuthService.API.AuthAPI.Models.Requests
{
    public class ConfirmEmailChangeAdmin : ConfirmEmailChange
    {
        public Guid UserId { get; set; }
    }
}
