using AuthService.Core.Application.DTOs.User.Interfaces;
using Repository.Contracts;

namespace AuthService.Core.Application.DTOs.User
{
    public class UpdateUserEmailDto : IEmailUpdateDto, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;

        public override bool Equals(object? obj)
        {
            if (obj is UpdateUserEmailDto dto)
            {
                return dto.Email == Email && dto.Id.Equals(Id);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Email);
        }
    }
}
