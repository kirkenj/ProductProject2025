using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.RegisterConfirmCommand
{
    public class RegisterConfirmCommandValidator : AbstractValidator<RegisterConfirmCommand>
    {
        public RegisterConfirmCommandValidator()
        {
            Include(new IEmailDtoValidator());
            RuleFor(c => c.Token).NotEmpty();
        }
    }
}
