using FluentValidation;
using Repository.Contracts;

namespace AuthService.Core.Application.Models.DTOs.Contracts.Validators
{
    public class IIdDtoValidator<T> : AbstractValidator<IIdObject<T>> where T : struct
    {
        public IIdDtoValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
        }
    }
}
