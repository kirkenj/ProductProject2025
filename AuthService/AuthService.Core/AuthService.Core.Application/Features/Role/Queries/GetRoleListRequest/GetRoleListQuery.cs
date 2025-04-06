using AuthService.Core.Application.Models.DTOs.Role;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.Queries.GetRoleListRequest
{
    public class GetRoleListQuery : IRequest<Response<List<RoleDto>>>
    {

    }
}
