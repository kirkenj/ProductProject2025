using AuthService.Core.Application.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.SendTokenToUpdateUserEmailComand
{
    public class SendTokenToUpdateUserEmailRequest : IRequest<Response<string>>
    {
        public UpdateUserEmailDto UpdateUserEmailDto { get; set; } = null!;
    }
}
