using AuthService.Core.Application.DTOs.User.Interfaces;

namespace AuthService.Core.Application.DTOs.User
{
    public class CreateUserDto : IEmailUpdateDto, IUserInfoDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
    }
}
