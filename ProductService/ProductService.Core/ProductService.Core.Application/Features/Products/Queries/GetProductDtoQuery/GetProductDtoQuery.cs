using CustomResponse;
using MediatR;
using ProductService.Core.Application.DTOs.Product;
using Repository.Contracts;

namespace ProductService.Core.Application.Features.Products.Queries.GetProductDtoQuery
{
    public class GetProductDtoQuery : IRequest<Response<ProductDto>>, IIdObject<Guid>
    {
        public Guid Id { get; set; }
    }
}
