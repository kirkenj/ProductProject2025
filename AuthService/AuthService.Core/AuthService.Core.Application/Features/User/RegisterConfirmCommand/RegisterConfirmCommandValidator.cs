using FluentValidation;

namespace AuthService.Core.Application.Features.User.RegisterConfirmCommand
{
    public class RegisterConfirmCommandValidator : AbstractValidator<RegisterConfirmCommand>
    {
        public RegisterConfirmCommandValidator()
        {
            RuleFor(c => c.Email).EmailAddress();
            RuleFor(c => c.Token).NotEmpty();
        }
    }
}
