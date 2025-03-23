using AuthService.Core.Application.DTOs.User.Interfaces;
using Repository.Contracts;

namespace AuthService.Core.Application.DTOs.User
{
    public class UpdateUserRoleDTO : IRoleDto, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public int RoleID { get; set; }
    }
}