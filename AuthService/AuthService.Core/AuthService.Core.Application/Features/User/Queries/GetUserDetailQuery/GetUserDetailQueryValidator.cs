using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.User.Queries.GetUserDetailQuery
{
    public class GetUserDetailQueryValidator : AbstractValidator<GetUserDetailQuery>
    {
        public GetUserDetailQueryValidator()
        {
            Include(new IIdDtoValidator<Guid>());
        }
    }
}
