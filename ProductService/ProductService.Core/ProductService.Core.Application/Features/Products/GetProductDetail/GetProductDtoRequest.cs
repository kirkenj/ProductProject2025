using Application.DTOs.Product;
using CustomResponse;
using MediatR;

namespace ProductService.Core.Application.Features.Products.GetProductDetail
{
    public class GetProductDtoRequest : IRequest<Response<ProductDto>>
    {
        public Guid Id { get; set; }
    }
}
