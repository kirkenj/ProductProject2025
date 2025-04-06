using FluentValidation;

namespace AuthService.Core.Application.Models.DTOs.Contracts.Validators
{
    public class IPasswordDtoValidator : AbstractValidator<IPasswordDto>
    {
        public IPasswordDtoValidator()
        {
            RuleFor(x => x.Password)
                .Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$")
                .WithMessage("Password must contain minimum eight characters, at least one letter and one number");
        }
    }
}
