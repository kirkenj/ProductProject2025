using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.CreateUserComand
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator(IUserRepository userRepository)
        {
            Include(new IUserInfoDtoValidator());

            Include(new IEmailUpdateDtoValidator(userRepository));
        }
    }
}
