using FluentValidation;
using ProductService.Core.Application.Features.Products.Contracts.Validators;


namespace ProductService.Core.Application.Features.Products.Contracts
{
    public class IEditProductValidator : AbstractValidator<IEditProductDto>
    {
        public IEditProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Price).NotEqual(default(decimal));
            RuleFor(x => x.CreationDate).NotEqual(default(DateTime));
            RuleFor(x => x.ProducerId).NotEqual(default(Guid));
        }
    }
}
