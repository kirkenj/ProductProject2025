using AuthService.Core.Application.Models.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.GetUserDto
{
    public class GetUserDtoRequest : IRequest<Response<UserDto>>
    {
        public Guid Id { get; set; }
    }
}
