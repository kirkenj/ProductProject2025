using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.SendTokenToUpdateUserEmailComand
{
    public class SendTokenToUpdateUserEmailRequestValidator : AbstractValidator<SendTokenToUpdateUserEmailRequest>
    {
        public SendTokenToUpdateUserEmailRequestValidator(IUserRepository userRepository)
        {
            RuleFor(r => r.UpdateUserEmailDto).NotNull().SetValidator(new UpdateUserEmailDtoValidator(userRepository));
        }
    }
}
