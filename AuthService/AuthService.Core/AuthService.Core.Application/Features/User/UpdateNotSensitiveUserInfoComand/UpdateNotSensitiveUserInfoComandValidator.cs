using AuthService.Core.Application.Features.User.Interfaces.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateNotSensitiveUserInfoComand
{
    public class UpdateNotSensitiveUserInfoComandValidator : AbstractValidator<UpdateNotSensitiveUserInfoComand>
    {
        public UpdateNotSensitiveUserInfoComandValidator()
        {
            Include(new IIdDtoValidator<Guid>());
            Include(new IUserInfoDtoValidator());
        }
    }
}
