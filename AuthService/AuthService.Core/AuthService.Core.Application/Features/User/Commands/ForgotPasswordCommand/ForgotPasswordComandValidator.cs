using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.ForgotPasswordCommand
{
    public class ForgotPasswordComandValidator : AbstractValidator<ForgotPasswordComand>
    {
        public ForgotPasswordComandValidator()
        {
            Include(new IEmailDtoValidator());
        }
    }
}
