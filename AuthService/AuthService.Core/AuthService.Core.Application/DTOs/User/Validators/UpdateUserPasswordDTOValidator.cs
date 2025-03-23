using AuthService.Core.Application.DTOs.User.Validators.Shared;
using FluentValidation;

namespace AuthService.Core.Application.DTOs.User.Validators
{
    public class UpdateUserPasswordDTOValidator : AbstractValidator<UpdateUserPasswordDto>
    {
        public UpdateUserPasswordDTOValidator()
        {
            Include(new IPasswordDtoValidator());
            Include(new IIdDtoValidator<Guid>());
        }
    }
}
