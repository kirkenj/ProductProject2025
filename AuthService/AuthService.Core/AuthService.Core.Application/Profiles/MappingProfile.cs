using AuthService.Core.Application.Features.Role.DTOs;
using AuthService.Core.Application.Features.User.CreateUserComand;
using AuthService.Core.Application.Features.User.DTOs;
using AuthService.Core.Application.Features.User.UpdateNotSensitiveUserInfoComand;
using AuthService.Core.Domain.Models;
using AutoMapper;

namespace AuthService.Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserCommand>().ReverseMap();
            CreateMap<User, UpdateNotSensitiveUserInfoComand>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
