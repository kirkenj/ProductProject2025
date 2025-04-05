using AuthService.Core.Application.Features.User.Interfaces;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.RegisterUserCommand
{
    public class RegisterUserCommand : IRequest<Response<string>>, IEmailUpdateDto, IUserInfoDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
