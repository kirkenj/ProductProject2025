using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.SendTokenToUpdateUserEmailRequest
{
    public class SendTokenToUpdateUserEmailCommandValidator : AbstractValidator<SendTokenToUpdateUserEmailCommand>
    {
        public SendTokenToUpdateUserEmailCommandValidator()
        {
            Include(new IIdDtoValidator<Guid>());
            Include(new IEmailDtoValidator());
        }
    }
}
