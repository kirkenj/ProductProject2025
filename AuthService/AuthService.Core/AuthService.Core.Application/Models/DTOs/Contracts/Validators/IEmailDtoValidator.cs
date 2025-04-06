using FluentValidation;

namespace AuthService.Core.Application.Models.DTOs.Contracts.Validators
{
    public class IEmailDtoValidator : AbstractValidator<IEmailDto>
    {
        public IEmailDtoValidator()
        {
            RuleFor(p => p.Email).EmailAddress().NotEmpty();
        }
    }
}
