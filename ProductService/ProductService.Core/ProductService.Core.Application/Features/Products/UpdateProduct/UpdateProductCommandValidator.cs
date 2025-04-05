using FluentValidation;
using ProductService.Core.Application.Features.Products.Contracts;

namespace ProductService.Core.Application.Features.Products.UpdateProduct
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
