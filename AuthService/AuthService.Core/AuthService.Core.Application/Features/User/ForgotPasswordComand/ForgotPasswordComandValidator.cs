using AuthService.Core.Application.DTOs.User.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.ForgotPasswordComand
{
    public class ForgotPasswordComandValidator : AbstractValidator<ForgotPasswordComand>
    {
        public ForgotPasswordComandValidator()
        {
            RuleFor(r => r.ForgotPasswordDto).NotNull().SetValidator(new ForgotPasswordDtoValidator());
        }
    }
}
