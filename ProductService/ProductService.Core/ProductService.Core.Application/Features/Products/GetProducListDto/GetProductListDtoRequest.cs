using CustomResponse;
using MediatR;
using ProductService.Core.Application.DTOs.Product;
using ProductService.Core.Application.Models.Product;

namespace ProductService.Core.Application.Features.Products.GetProducListDto
{
    public class GetProductListDtoRequest : IRequest<Response<IEnumerable<ProductListDto>>>
    {
        public ProductFilter ProductFilter { get; set; } = null!;
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
