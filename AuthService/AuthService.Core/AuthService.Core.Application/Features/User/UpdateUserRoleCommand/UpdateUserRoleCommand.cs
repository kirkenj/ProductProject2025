using AuthService.Core.Application.DTOs.User;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.User.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommand : IRequest<Response<string>>
    {
        public UpdateUserRoleDTO UpdateUserRoleDTO { get; set; } = null!;
    }
}
