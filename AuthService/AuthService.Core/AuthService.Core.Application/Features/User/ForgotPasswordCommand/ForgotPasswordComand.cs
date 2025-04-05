using AuthService.Core.Application.Features.User.Interfaces;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.ForgotPasswordComand
{
    public class ForgotPasswordComand : IRequest<Response<string>>, IEmailDto
    {
        public string Email { get; set; } = null!;
    }
}
