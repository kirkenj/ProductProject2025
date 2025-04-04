using Repository.Contracts;

namespace AuthService.Core.Application.Models.DTOs.Role
{
    public class RoleDto : IIdObject<int>
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;


        public override bool Equals(object? obj)
        {
            if (obj is not RoleDto dto) return base.Equals(obj);

            return dto.Id == Id && dto.Name == Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Id);
        }
    }
}
