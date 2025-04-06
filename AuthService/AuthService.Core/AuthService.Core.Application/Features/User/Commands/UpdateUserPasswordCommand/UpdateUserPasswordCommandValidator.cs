using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.UpdateUserPasswordCommand
{
    public class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
    {
        public UpdateUserPasswordCommandValidator()
        {
            Include(new IIdDtoValidator<Guid>());
            Include(new IPasswordDtoValidator());
        }
    }
}
