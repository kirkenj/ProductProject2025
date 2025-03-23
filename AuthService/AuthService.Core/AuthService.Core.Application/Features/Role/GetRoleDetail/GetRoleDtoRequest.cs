using AuthService.Core.Application.DTOs.Role;
using CustomResponse;
using MediatR;

namespace AuthService.Core.Application.Features.Role.GetRoleDetail
{
    public class GetRoleDtoRequest : IRequest<Response<RoleDto>>
    {
        public int Id { get; set; }
    }
}
