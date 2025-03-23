using AuthService.Core.Application.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.ConfirmEmailChangeComand
{
    public class ConfirmEmailChangeComand : IRequest<Response<string>>
    {
        public ConfirmEmailChangeDto ConfirmEmailChangeDto { get; set; } = null!;
    }
}

