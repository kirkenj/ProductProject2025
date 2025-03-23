namespace ProductService.Core.Application.Models.UserClient
{
    public class AuthClientRole
    {

        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public int Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = default!;

    }
}
