using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace ProductService.Core.Application.Features.Products.Commands.RemoveProductCommand
{
    public class RemoveProductCommand : IRequest<Response<string>>, IIdObject<Guid>
    {
        public Guid Id { get; set; }
    }
}
