using AuthService.Core.Application.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.ForgotPasswordComand
{
    public class ForgotPasswordComand : IRequest<Response<string>>
    {
        public ForgotPasswordDto ForgotPasswordDto { get; set; } = null!;
    }
}
