namespace Clients.Adapters.AuthClient.Contracts
{
    public class AuthClientUser
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public Guid Id { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("login")]
        public string Login { get; set; } = default!;

        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; } = default!;
    }
}
