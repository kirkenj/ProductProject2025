using AuthService.Core.Application.DTOs.User.Interfaces;

namespace AuthService.Core.Application.DTOs.User
{
    public class ForgotPasswordDto : IEmailDto
    {
        public string Email { get; set; } = null!;
    }
}
