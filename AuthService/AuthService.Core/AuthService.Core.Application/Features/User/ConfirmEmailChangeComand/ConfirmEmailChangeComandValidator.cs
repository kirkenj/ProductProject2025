using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.ConfirmEmailChangeComand
{
    public class ConfirmEmailChangeComandValidator : AbstractValidator<ConfirmEmailChangeComand>
    {
        public ConfirmEmailChangeComandValidator(IUserRepository userRepository)
        {
            Include(new IIdDtoValidator<Guid>());
            RuleFor(u => u.OtpToNewEmail).NotEmpty().NotNull();
            RuleFor(u => u.OtpToOldEmail).NotEmpty().NotNull();
        }
    }
}