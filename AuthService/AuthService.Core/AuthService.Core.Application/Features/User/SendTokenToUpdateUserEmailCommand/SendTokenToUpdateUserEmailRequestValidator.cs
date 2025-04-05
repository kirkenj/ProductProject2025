using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.SendTokenToUpdateUserEmailCommand
{
    public class SendTokenToUpdateUserEmailRequestValidator : AbstractValidator<SendTokenToUpdateUserEmailRequest>
    {
        public SendTokenToUpdateUserEmailRequestValidator(IUserRepository userRepository)
        {
            Include(new IEmailUpdateDtoValidator(userRepository));
            Include(new IIdDtoValidator<Guid>());
        }
    }
}
