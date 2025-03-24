using FluentValidation;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.Interfaces.Validators
{
    public class IIdDtoValidator<T> : AbstractValidator<IIdObject<T>> where T : struct
    {
        public IIdDtoValidator()
        {
            RuleFor(p => p.Id).NotEqual(default(T));
        }
    }
}
