using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class ForgotPasswordDtoValidator : AbstractValidator<ForgotPasswordDto>
    {
        public ForgotPasswordDtoValidator()
        {
            Include(new IEmailDtoValidator());
        }
    }
}
