using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            Include(new IEmailDtoValidator());
        }
    }
}
