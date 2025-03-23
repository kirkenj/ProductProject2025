using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class UpdateUserLoginDtoValidator : AbstractValidator<UpdateUserLoginDto>
    {
        public UpdateUserLoginDtoValidator(IUserRepository userRepository)
        {
            Include(new IIdDtoValidator<Guid>());

            RuleFor(u => u.NewLogin).NotEmpty();

            RuleFor(u => u.NewLogin).MustAsync(async (login, token) =>
            {
                var result = await userRepository.GetAsync(new() { AccurateLogin = login });
                return result == null;
            }).WithMessage("This login is already taken.");
        }
    }
}
