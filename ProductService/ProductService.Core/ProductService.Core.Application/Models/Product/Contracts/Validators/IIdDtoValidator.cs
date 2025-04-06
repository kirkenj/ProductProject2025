using FluentValidation;
using Repository.Contracts;

namespace ProductService.Core.Application.Models.Product.Contracts.Validators
{
    public class IIdDtoValidator<T> : AbstractValidator<IIdObject<T>> where T : struct
    {
        public IIdDtoValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }
}
