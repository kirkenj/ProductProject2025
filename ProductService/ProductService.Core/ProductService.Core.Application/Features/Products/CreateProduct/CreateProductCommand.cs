using Application.DTOs.Product;
using CustomResponse;
using MediatR;

namespace ProductService.Core.Application.Features.Products.CreateProduct
{
    public class CreateProductCommand : IRequest<Response<Guid>>
    {
        public CreateProductDto CreateProductDto { get; set; } = null!;
    }
}
