using AuthService.Core.Application.DTOs.User.Interfaces;
using Repository.Contracts;

namespace AuthService.Core.Application.DTOs.User
{
    public class UpdateUserPasswordDto : IPasswordDto, IIdObject<Guid>
    {
        public Guid Id { get; set; }
        public string Password { get; set; } = null!;
    }
}