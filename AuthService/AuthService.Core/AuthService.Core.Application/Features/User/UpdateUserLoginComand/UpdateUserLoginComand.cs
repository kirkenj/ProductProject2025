using AuthService.Core.Application.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateUserLoginComand
{
    public class UpdateUserLoginComand : IRequest<Response<string>>
    {
        public UpdateUserLoginDto UpdateUserLoginDto { get; set; } = null!;
    }
}
