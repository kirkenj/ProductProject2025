using AuthService.Core.Application.Features.User.Interfaces;
using AuthService.Core.Application.Models.DTOs.User;
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
