using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.ConfirmEmailChangeComand
{
    public class ConfirmEmailChangeComandValidator : AbstractValidator<ConfirmEmailChangeComand>
    {
        public ConfirmEmailChangeComandValidator()
        {
            RuleFor(p => p.Id).NotEmpty();
            RuleFor(u => u.OtpToNewEmail).NotEmpty();
            RuleFor(u => u.OtpToOldEmail).NotEmpty();
        }
    }
}