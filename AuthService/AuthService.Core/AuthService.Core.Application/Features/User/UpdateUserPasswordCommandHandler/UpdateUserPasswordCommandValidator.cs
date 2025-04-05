using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateUserPasswordCommandHandler
{
    public class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
    {
        public UpdateUserPasswordCommandValidator()
        {
            Include(new IPasswordDtoValidator());
            Include(new IIdDtoValidator<Guid>());
        }
    }
}
