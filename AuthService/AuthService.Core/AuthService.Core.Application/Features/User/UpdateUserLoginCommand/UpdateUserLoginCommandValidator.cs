using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateUserLoginCommand
{
    public class UpdateUserLoginCommandValidator : AbstractValidator<UpdateUserLoginCommand>
    {
        public UpdateUserLoginCommandValidator(IUserRepository userRepository)
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
