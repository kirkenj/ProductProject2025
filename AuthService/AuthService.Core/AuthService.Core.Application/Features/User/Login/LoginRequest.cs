using AuthService.Core.Application.Features.User.DTOs;
using AuthService.Core.Application.Features.User.Interfaces;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Login
{
    public class LoginRequest : IRequest<Response<UserDto>>, IEmailDto, IPasswordDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
