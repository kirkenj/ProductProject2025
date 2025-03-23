using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleCommandValidator(IRoleRepository roleRepository)
        {
            RuleFor(r => r.UpdateUserRoleDTO).NotNull().SetValidator(new UpdateUserRoleDTOValidator(roleRepository));
        }
    }
}
