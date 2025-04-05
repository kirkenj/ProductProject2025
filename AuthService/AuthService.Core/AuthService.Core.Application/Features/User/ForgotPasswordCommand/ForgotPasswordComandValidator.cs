using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.ForgotPasswordComand
{
    public class ForgotPasswordComandValidator : AbstractValidator<ForgotPasswordComand>
    {
        public ForgotPasswordComandValidator()
        {
            Include(new IEmailDtoValidator());
        }
    }
}
