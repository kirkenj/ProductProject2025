using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.RegisterConfirmCommand
{
    public class RegisterConfirmCommand : IRequest<Response<string>>
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
