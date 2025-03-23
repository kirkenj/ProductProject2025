using AuthService.Core.Application.Features.User.GetUserDto;

namespace AuthAPI.Contracts
{
    public interface IJwtProviderService
    {
        public string GetToken(UserDto user);
    }
}
