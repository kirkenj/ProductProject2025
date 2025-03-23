using CustomResponse;
using MediatR;

namespace ProductService.Core.Application.Features.Products.RemoveProduct
{
    public class RemovePrductComand : IRequest<Response<string>>
    {
        public Guid ProductId { get; set; }
    }
}
