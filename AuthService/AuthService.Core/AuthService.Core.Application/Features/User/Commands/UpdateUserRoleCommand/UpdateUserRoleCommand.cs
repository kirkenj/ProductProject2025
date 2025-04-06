using CustomResponse;
using MediatR;
using Repository.Contracts;

namespace AuthService.Core.Application.Features.User.Commands.UpdateUserRoleCommand
{
    public class UpdateUserRoleCommand : IRequest<Response<string>>, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public int RoleID { get; set; }
    }
}
