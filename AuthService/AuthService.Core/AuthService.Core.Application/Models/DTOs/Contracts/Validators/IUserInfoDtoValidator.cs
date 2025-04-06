using FluentValidation;

namespace AuthService.Core.Application.Models.DTOs.Contracts.Validators
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
