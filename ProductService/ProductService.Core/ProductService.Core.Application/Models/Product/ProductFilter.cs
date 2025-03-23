using System.Diagnostics.CodeAnalysis;

namespace ProductService.Core.Application.Models.Product
{
    public class ProductFilter
    {
        [AllowNull]
        public IEnumerable<Guid>? Ids { get; set; }
        [AllowNull]
        public string? NamePart { get; set; }
        [AllowNull]
        public string? DescriptionPart { get; set; }
        [AllowNull]
        public decimal? PriceStart { get; set; }
        [AllowNull]
        public decimal? PriceEnd { get; set; }
        [AllowNull]
        public bool? IsAvailable { get; set; }
        [AllowNull]
        public IEnumerable<Guid>? ProducerIds { get; set; }
        [AllowNull]
        public DateTime? CreationDateStart { get; set; }
        [AllowNull]
        public DateTime? CreationDateEnd { get; set; }
    }
}
