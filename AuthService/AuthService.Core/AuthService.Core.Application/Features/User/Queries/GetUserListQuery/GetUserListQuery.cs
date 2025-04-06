using Application.Models.User;
using AuthService.Core.Application.Models.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Queries.GetUserListQuery
{
    public class GetUserListQuery : IRequest<Response<List<UserDto>>>
    {
        public UserFilter UserFilter { get; set; } = null!;
        public int? PageSize { get; set; }
        public int? Page { get; set; }
    }
}
