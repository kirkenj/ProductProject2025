using AuthService.Core.Application.Features.User.Interfaces;
using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommand : IRequest<Response<string>>, IRoleDto, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public int RoleID { get; set; }
    }
}
