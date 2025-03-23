using AuthService.Core.Application.DTOs.User.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.UpdateNotSensitiveUserInfoComand
{
    public class UpdateNotSensitiveUserInfoComandValidator : AbstractValidator<UpdateNotSensitiveUserInfoComand>
    {
        public UpdateNotSensitiveUserInfoComandValidator()
        {
            RuleFor(r => r.UpdateUserInfoDto).NotNull().SetValidator(new UpdateUserInfoDtoValidator());
        }
    }
}
