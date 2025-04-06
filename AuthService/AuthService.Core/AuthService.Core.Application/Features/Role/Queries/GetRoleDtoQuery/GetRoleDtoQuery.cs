using AuthService.Core.Application.Models.DTOs.Role;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.Role.Queries.GetRoleDtoQuery
{
    public class GetRoleDtoQuery : IRequest<Response<RoleDto>>, IIdObject<int>
    {
        public int Id { get; set; }
    }
}
