using FluentValidation;

namespace AuthService.Core.Application.Features.User.Interfaces.Validators
{
    public class IEmailDtoValidator : AbstractValidator<IEmailDto>
    {
        public IEmailDtoValidator()
        {
            RuleFor(p => p.Email).EmailAddress().NotEmpty();
        }
    }
}
