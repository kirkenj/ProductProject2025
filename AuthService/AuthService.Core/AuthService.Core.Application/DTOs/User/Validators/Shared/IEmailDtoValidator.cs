using AuthService.Core.Application.DTOs.User.Interfaces;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators.Shared
{
    public class IEmailDtoValidator : AbstractValidator<IEmailDto>
    {
        public IEmailDtoValidator()
        {
            RuleFor(p => p.Email).EmailAddress().NotEmpty();
        }
    }
}
