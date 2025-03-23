using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class ConfirmEmailChangeDtoValidator : AbstractValidator<ConfirmEmailChangeDto>
    {
        public ConfirmEmailChangeDtoValidator(IUserRepository userRepository)
        {
            Include(new IIdDtoValidator<Guid>());
            RuleFor(u => u.Token).NotEmpty().NotNull();
        }
    }
}
