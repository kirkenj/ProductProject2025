using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class SendTokenToUpdateUserEmailDtoValidator : AbstractValidator<UpdateUserEmailDto>
    {
        public SendTokenToUpdateUserEmailDtoValidator()
        {
            Include(new IEmailDtoValidator());
            Include(new IIdDtoValidator<Guid>());
        }
    }
}
