using AuthService.Core.Application.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateUserPasswordComandHandler
{
    public class UpdateUserPasswordComand : IRequest<Response<string>>
    {
        public UpdateUserPasswordDto UpdateUserPasswordDto { get; set; } = null!;
    }
}
