using FluentValidation;
using ProductService.Core.Application.Models.Product.Contracts;

namespace ProductService.Core.Application.Features.Products.Commands.UpdateProductCommand
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            Include(new IEditProductValidator());
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
