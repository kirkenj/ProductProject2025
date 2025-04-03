using CustomResponse;
using MediatR;
using ProductService.Core.Application.DTOs.Product;

namespace ProductService.Core.Application.Features.Products.GetProductDetail
{
    public class GetProductDtoRequest : IRequest<Response<ProductDto>>
    {
        public Guid Id { get; set; }
    }
}
