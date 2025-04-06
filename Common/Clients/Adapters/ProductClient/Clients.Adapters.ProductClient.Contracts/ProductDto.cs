namespace Clients.Adapters.ProductClient.Contracts
{
    public class ProductDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public Guid Id { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("price")]
        public double Price { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("isAvailable")]
        public bool IsAvailable { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("producerId")]
        public Guid ProducerId { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("creationDate")]
        public DateTimeOffset CreationDate { get; set; }
    }
}