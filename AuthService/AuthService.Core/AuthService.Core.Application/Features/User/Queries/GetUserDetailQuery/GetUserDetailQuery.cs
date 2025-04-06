using AuthService.Core.Application.Models.DTOs.User;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.Queries.GetUserDetailQuery
{
    public class GetUserDetailQuery : IRequest<Response<UserDto>>, IIdObject<Guid>
    {
        public Guid Id { get; set; }
    }
}
