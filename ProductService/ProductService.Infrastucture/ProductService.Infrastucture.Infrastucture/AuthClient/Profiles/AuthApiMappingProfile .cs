using AutoMapper;
using Clients.AuthApi;
using ProductService.Core.Application.Models.UserClient;

namespace ProductService.Infrastucture.Infrastucture.AuthClient.Profiles
{
    public class AuthApiMappingProfile : Profile
    {
        public AuthApiMappingProfile()
        {
            CreateMap<UserDto, AuthClientUser>();
        }
    }
}
