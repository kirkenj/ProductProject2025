using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Queries.LoginQuery
{
    public class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        public LoginQueryValidator()
        {
            Include(new IEmailDtoValidator());
            Include(new IPasswordDtoValidator());
        }
    }
}
