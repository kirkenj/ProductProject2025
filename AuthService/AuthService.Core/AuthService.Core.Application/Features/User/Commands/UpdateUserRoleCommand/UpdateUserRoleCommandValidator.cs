using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleCommandValidator()
        {
            Include(new IIdDtoValidator<Guid>());
            RuleFor(r => r.RoleID).NotEmpty();
        }
    }
}
