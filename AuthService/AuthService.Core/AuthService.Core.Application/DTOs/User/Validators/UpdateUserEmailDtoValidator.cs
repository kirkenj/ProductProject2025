using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class UpdateUserEmailDtoValidator : AbstractValidator<UpdateUserEmailDto>
    {
        public UpdateUserEmailDtoValidator(IUserRepository userRepository)
        {
            Include(new IEmailUpdateDtoValidator(userRepository));
        }
    }
}
