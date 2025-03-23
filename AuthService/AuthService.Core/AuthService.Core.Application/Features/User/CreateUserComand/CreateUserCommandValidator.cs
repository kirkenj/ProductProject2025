using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.CreateUserComand
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IUserRepository userRepository)
        {
            RuleFor(r => r.CreateUserDto).NotNull().SetValidator(new CreateUserDtoValidator(userRepository));
        }
    }
}
