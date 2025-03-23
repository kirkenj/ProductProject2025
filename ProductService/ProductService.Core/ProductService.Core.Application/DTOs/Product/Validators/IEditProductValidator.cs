using FluentValidation;
using ProductService.Core.Application.DTOs.Product.Contracts;


namespace ProductService.Core.Application.DTOs.Product.Validators
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
