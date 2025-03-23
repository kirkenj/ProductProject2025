namespace Application.DTOs.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public Guid ProducerId { get; set; }
        public DateTime CreationDate { get; set; }



        public override bool Equals(object? obj)
        {
            if (obj is ProductDto dto)
            {
                return
                    dto.Id == Id &&
                    dto.Name == Name &&
                    dto.Description == Description &&
                    dto.Price == Price &&
                    dto.IsAvailable == IsAvailable &&
                    dto.ProducerId == ProducerId &&
                    dto.CreationDate == CreationDate;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Description, Price, IsAvailable, ProducerId, CreationDate);
        }
    }
}
