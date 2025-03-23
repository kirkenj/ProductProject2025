using AuthService.Core.Application.DTOs.User;
using AuthService.Core.Application.Features.User.GetUserDto;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Login
{
    public class LoginRequest : IRequest<Response<UserDto>>
    {
        public LoginDto LoginDto { get; set; } = null!;
    }
}
