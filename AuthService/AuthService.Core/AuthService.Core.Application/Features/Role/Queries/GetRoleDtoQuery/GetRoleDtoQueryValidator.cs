using AuthService.Core.Application.Models.DTOs.Contracts.Validators;
using FluentValidation;

namespace AuthService.Core.Application.Features.Role.Queries.GetRoleDtoQuery
{
    public class GetRoleDtoQueryValidator : AbstractValidator<GetRoleDtoQuery>
    {
        public GetRoleDtoQueryValidator()
        {
            Include(new IIdDtoValidator<int>());
        }
    }
}
