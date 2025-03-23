namespace AuthAPI.Models.Requests
{
    public class LoginResultModel
    {
        public string Token { get; set; } = null!;
        public Guid UserId { get; set; }
    }
}
