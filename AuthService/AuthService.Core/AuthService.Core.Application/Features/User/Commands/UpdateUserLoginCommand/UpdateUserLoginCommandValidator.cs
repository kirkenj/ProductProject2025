using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.UpdateUserLoginCommand
{
    public class UpdateUserLoginCommandValidator : AbstractValidator<UpdateUserLoginCommand>
    {
        public UpdateUserLoginCommandValidator()
        {
            Include(new IIdDtoValidator<Guid>());
            RuleFor(u => u.NewLogin).NotEmpty();
        }
    }
}
