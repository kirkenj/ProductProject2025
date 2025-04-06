using FluentValidation;
using ProductService.Core.Application.Models.Product.Contracts.Validators;


namespace ProductService.Core.Application.Models.Product.Contracts
{
    public class IEditProductValidator : AbstractValidator<IEditProductDto>
    {
        public IEditProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Price).NotEmpty().GreaterThan(0);
            RuleFor(x => x.CreationDate).NotEmpty();
            RuleFor(x => x.ProducerId).NotEmpty();
        }
    }
}
