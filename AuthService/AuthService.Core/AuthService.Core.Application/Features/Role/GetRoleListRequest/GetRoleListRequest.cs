using AuthService.Core.Application.Features.Role.DTOs;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.GetRoleListRequest
{
    public class GetRoleListRequest : IRequest<Response<List<RoleDto>>>
    {

    }
}
