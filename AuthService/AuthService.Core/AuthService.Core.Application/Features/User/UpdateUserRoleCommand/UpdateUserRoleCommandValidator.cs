using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleCommandValidator(IRoleRepository roleRepository)
        {
            Include(new IIdDtoValidator<Guid>());
            Include(new IRoleDtoValidator(roleRepository));
        }
    }
}
