using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.RegisterUserCommand
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            Include(new IUserInfoDtoValidator());

            Include(new IEmailDtoValidator());

            Include(new IPasswordDtoValidator());

            RuleFor(r => r.ConfirmPassword).Equal(r => r.Password);
        }
    }
}
