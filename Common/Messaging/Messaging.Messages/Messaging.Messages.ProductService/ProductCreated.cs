namespace Messaging.Messages.ProductService
{
    public class ProductCreated
    {
        public Guid ProductId { get; set; }
        public Guid ProducerId { get; set; }
    }
}
