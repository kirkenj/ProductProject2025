using Application.DTOs.Product;
using CustomResponse;
using MediatR;

namespace ProductService.Core.Application.Features.Products.UpdateProduct
{
    public class UpdateProductCommand : IRequest<Response<string>>
    {
        public UpdateProductDto UpdateProductDto { get; set; } = null!;
    }
}
