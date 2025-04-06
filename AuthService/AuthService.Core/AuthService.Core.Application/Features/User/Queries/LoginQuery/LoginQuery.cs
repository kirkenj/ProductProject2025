using AuthService.Core.Application.Models.DTOs.Contracts;
using AuthService.Core.Application.Models.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Queries.LoginQuery
{
    public class LoginQuery : IRequest<Response<UserDto>>, IPasswordDto, IEmailDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
