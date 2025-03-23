using Application.Models.User;
using AuthService.Core.Application.Features.User.GetUserDto;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.GetUserList
{
    public class GetUserListRequest : IRequest<Response<List<UserDto>>>
    {
        public UserFilter UserFilter { get; set; } = null!;
        public int? PageSize { get; set; }
        public int? Page { get; set; }
    }
}
