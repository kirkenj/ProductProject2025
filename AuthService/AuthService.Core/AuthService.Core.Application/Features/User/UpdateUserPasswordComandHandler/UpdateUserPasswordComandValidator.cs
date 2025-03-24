using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateUserPasswordComandHandler
{
    public class UpdateUserPasswordComandValidator : AbstractValidator<UpdateUserPasswordComand>
    {
        public UpdateUserPasswordComandValidator()
        {
            Include(new IPasswordDtoValidator());
            Include(new IIdDtoValidator<Guid>());
        }
    }
}
