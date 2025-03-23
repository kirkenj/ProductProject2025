using AuthService.Core.Application.DTOs.Role;
using AuthService.Core.Application.DTOs.User;
using AuthService.Core.Application.Features.User.GetUserDto;
using AuthService.Core.Domain.Models;
using AutoMapper;

namespace Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, UpdateUserInfoDto>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
