using AuthService.Core.Application.Contracts.Persistence;
using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class UpdateUserRoleDTOValidator : AbstractValidator<UpdateUserRoleDTO>
    {
        public UpdateUserRoleDTOValidator(IRoleRepository roleRepository)
        {
            Include(new IIdDtoValidator<Guid>());
            Include(new IRoleDtoValidator(roleRepository));
        }
    }
}
