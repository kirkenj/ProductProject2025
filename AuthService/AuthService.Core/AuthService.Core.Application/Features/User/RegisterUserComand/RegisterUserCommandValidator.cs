using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.RegisterUserComand
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator(IUserRepository userRepository)
        {
            Include(new IUserInfoDtoValidator());

            Include(new IEmailUpdateDtoValidator(userRepository));

            RuleFor(r => r.Password).Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")
                .WithMessage("Password must contain minimum eight characters, at least one letter and one number");

            RuleFor(r => r.ConfirmPassword).Equal(r => r.Password);
        }
    }
}
