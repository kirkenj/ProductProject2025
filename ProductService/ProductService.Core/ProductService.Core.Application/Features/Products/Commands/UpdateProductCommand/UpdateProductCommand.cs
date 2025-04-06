using CustomResponse;
using MediatR;
using ProductService.Core.Application.Models.Product.Contracts.Validators;

namespace ProductService.Core.Application.Features.Products.Commands.UpdateProductCommand
{
    public class UpdateProductCommand : IRequest<Response<string>>, IEditProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public Guid ProducerId { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
