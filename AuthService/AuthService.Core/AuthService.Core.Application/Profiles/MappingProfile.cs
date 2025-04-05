using AuthService.Core.Application.Features.User.RegisterUserCommand;
using AuthService.Core.Application.Features.User.UpdateNotSensitiveUserInfoCommand;
using AuthService.Core.Application.Models.DTOs.Role;
using AuthService.Core.Application.Models.DTOs.User;
using AuthService.Core.Domain.Models;
using AutoMapper;

namespace AuthService.Core.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<RegisterUserCommand, User>().ReverseMap();
            CreateMap<User, UpdateNotSensitiveUserInfoCommand>();
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
