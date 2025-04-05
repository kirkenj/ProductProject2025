using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateNotSensitiveUserInfoCommand
{
    public class UpdateNotSensitiveUserInfoCommandValidator : AbstractValidator<UpdateNotSensitiveUserInfoCommand>
    {
        public UpdateNotSensitiveUserInfoCommandValidator()
        {
            Include(new IIdDtoValidator<Guid>());
            Include(new IUserInfoDtoValidator());
        }
    }
}
