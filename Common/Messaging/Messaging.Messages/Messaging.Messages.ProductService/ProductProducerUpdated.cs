namespace Messaging.Messages.ProductService
{
    public class ProductProducerUpdated
    {
        public Guid ProductId { get; set; }
        public Guid NewProducerId { get; set; }
        public Guid OldProducerId { get; set; }
    }
}
