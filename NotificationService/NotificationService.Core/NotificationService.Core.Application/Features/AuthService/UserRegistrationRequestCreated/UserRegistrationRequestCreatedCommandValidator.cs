using FluentValidation;

namespace NotificationService.Core.Application.Features.AuthService.UserRegistrationRequestCreated
{
    public class UserRegistrationRequestCreatedCommandValidator : AbstractValidator<UserRegistrationRequestCreatedCommand>
    {
        public UserRegistrationRequestCreatedCommandValidator()
        {
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.Token).NotEmpty();
        }
    }
}
