using AuthService.Core.Application.Contracts.Persistence;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Interfaces.Validators
{
    public class IRoleDtoValidator : AbstractValidator<IRoleDto>
    {
        public IRoleDtoValidator(IRoleRepository roleRepository)
        {
            RuleFor(p => p.RoleID)
                .MustAsync(async (id, cancellationToken) =>
                    await roleRepository.GetAsync(id, cancellationToken) == null)
                .WithMessage("Role with this id does not exist");
        }
    }
}
