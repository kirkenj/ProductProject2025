using FluentValidation;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.ChangeEmailRequest
{
    public class ChangeEmailRequestCommandValidator : AbstractValidator<ChangeEmailRequestCommand>
    {
        public ChangeEmailRequestCommandValidator()
        {
            RuleFor(c => c.NewEmail).NotEmpty().EmailAddress();
            RuleFor(c => c.OtpToNewEmail).NotEmpty();
            RuleFor(c => c.OtpToOldEmail).NotEmpty();
        }
    }
}
