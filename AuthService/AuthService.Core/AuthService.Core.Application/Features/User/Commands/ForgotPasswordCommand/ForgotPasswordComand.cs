using AuthService.Core.Application.Models.DTOs.Contracts;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Commands.ForgotPasswordCommand
{
    public class ForgotPasswordComand : IRequest<Response<string>>, IEmailDto
    {
        public string Email { get; set; } = null!;
    }
}
