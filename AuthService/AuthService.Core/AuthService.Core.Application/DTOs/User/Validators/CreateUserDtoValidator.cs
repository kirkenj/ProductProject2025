using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator(IUserRepository userRepository)
        {
            Include(new IUserInfoDtoValidator());

            Include(new IEmailUpdateDtoValidator(userRepository));
        }
    }
}
