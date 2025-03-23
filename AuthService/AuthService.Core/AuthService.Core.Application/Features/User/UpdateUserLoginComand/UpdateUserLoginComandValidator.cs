using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateUserLoginComand
{
    public class UpdateUserLoginComandValidator : AbstractValidator<UpdateUserLoginComand>
    {
        public UpdateUserLoginComandValidator(IUserRepository userRepository)
        {
            RuleFor(r => r.UpdateUserLoginDto).NotNull().SetValidator(new UpdateUserLoginDtoValidator(userRepository));
        }
    }
}
