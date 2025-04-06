using AuthService.Core.Application.Models.DTOs.Contracts;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.Commands.RegisterConfirmCommand
{
    public class RegisterConfirmCommand : IRequest<Response<string>>, IEmailDto
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
