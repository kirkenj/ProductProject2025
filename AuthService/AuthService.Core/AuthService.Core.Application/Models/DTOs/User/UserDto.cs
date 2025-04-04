using AuthService.Core.Application.Features.User.Interfaces;
using AuthService.Core.Application.Models.DTOs.Role;
using Repository.Contracts;

namespace AuthService.Core.Application.Models.DTOs.User
{
    public class UserDto : IIdObject<Guid>, IUserInfoDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public RoleDto Role { get; set; } = null!;


        public override bool Equals(object? obj)
        {
            if (obj is UserDto dto)
                return Id == dto.Id &&
                    Login == dto.Login &&
                    Email == dto.Email &&
                    Name == dto.Name &&
                    Address == dto.Address &&
                    Role.Equals(dto.Role);

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Login, Email, Name, Address, Role);
        }
    }
}
