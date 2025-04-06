using FluentValidation;
using ProductService.Core.Application.Models.Product.Contracts;

namespace ProductService.Core.Application.Features.Products.Commands.CreateProductCommand
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            Include(new IEditProductValidator());
        }
    }
}
