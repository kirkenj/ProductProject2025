using AuthService.Core.Application.Models.DTOs.Contracts;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Commands.RegisterUserCommand
{
    public class RegisterUserCommand : IRequest<Response<string>>, IUserInfoDto, IPasswordDto, IEmailDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
