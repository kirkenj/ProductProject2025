using FluentValidation;
using ProductService.Core.Application.Models.Product.Contracts.Validators;

namespace ProductService.Core.Application.Features.Products.Commands.RemoveProductCommand
{
    public class RemoveProductCommandValidator : AbstractValidator<RemoveProductCommand>
    {
        public RemoveProductCommandValidator()
        {
            Include(new IIdDtoValidator<Guid>());
        }
    }
}
