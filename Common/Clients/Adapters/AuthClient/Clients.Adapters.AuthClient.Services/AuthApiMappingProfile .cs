using AutoMapper;
using Clients.Adapters.AuthClient.Contracts;
using Clients.AuthApi;

namespace Clients.Adapters.AuthClient.Services
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<UserDto, AuthClientUser>();
        }
    }
}
