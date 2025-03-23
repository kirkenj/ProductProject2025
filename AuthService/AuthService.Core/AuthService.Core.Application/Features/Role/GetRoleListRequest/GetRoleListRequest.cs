using AuthService.Core.Application.DTOs.Role;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.GetRoleListRequest
{
    public class GetRoleListRequest : IRequest<Response<List<RoleDto>>>
    {

    }
}
