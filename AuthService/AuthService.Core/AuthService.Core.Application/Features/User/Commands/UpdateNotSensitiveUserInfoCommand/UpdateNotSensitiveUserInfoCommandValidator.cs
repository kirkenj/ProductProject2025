using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Commands.UpdateNotSensitiveUserInfoCommand
{
    public class UpdateNotSensitiveUserInfoCommandValidator : AbstractValidator<UpdateNotSensitiveUserInfoCommand>
    {
        public UpdateNotSensitiveUserInfoCommandValidator()
        {
            Include(new IUserInfoDtoValidator());
            Include(new IIdDtoValidator<Guid>());
        }
    }
}
