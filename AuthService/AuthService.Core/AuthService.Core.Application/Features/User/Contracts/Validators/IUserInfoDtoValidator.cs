using FluentValidation;

namespace AuthService.Core.Application.Features.User.Interfaces.Validators
{
    public class IUserInfoDtoValidator : AbstractValidator<IUserInfoDto>
    {
        public IUserInfoDtoValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Address).NotEmpty();
        }
    }
}
