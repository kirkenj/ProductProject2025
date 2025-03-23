using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.ConfirmEmailChangeComand
{
    public class ConfirmEmailChangeComandValidator : AbstractValidator<ConfirmEmailChangeComand>
    {
        public ConfirmEmailChangeComandValidator(IUserRepository userRepository)
        {
            RuleFor(r => r.ConfirmEmailChangeDto).NotNull().SetValidator(new ConfirmEmailChangeDtoValidator(userRepository));
        }
    }
}