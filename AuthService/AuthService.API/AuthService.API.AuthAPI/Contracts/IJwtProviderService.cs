using AuthService.Core.Application.Models.DTOs.User;

namespace AuthService.API.AuthAPI.Contracts
{
    public interface IJwtProviderService
    {
        public string GetToken(UserDto user);
        public bool IsTokenValid(string token);
    }
}
