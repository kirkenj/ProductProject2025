using AuthService.Core.Application.Features.User.DTOs;

namespace AuthService.API.AuthAPI.Contracts
{
    public interface IJwtProviderService
    {
        public string GetToken(UserDto user);
        public bool IsTokenValid(string token);
    }
}
