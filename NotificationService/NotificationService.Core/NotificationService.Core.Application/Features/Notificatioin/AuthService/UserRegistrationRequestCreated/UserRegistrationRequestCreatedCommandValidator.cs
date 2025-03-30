using FluentValidation;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.UserRegistrationRequestCreated
{
    public class UserRegistrationRequestCreatedCommandValidator : AbstractValidator<UserRegistrationRequestCreatedCommand>
    {
        public UserRegistrationRequestCreatedCommandValidator()
        {
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.Password).NotEmpty();
        }
    }
}
