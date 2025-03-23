using AuthService.Core.Application.DTOs.User.Interfaces;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators.Shared
{
    public class IPasswordDtoValidator : AbstractValidator<IPasswordDto>
    {
        public IPasswordDtoValidator()
        {
            RuleFor(o => o.Password)
                .NotEmpty()
                .NotNull()
                .MinimumLength(8)
                .Matches("^[a-zA-Z0-9]+$").WithMessage("{PropertyName} can contaim A-Z, a-z, 0-9 symbols");
        }
    }
}
