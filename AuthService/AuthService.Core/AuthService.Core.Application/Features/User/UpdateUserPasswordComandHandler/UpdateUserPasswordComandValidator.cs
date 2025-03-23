using AuthService.Core.Application.DTOs.User.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateUserPasswordComandHandler
{
    public class UpdateUserPasswordComandValidator : AbstractValidator<UpdateUserPasswordComand>
    {
        public UpdateUserPasswordComandValidator()
        {
            RuleFor(r => r.UpdateUserPasswordDto).NotNull().SetValidator(new UpdateUserPasswordDTOValidator());
        }
    }
}
