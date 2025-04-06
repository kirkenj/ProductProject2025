using AuthService.Core.Application.Features.User.Commands.RegisterUserCommand;
using AuthService.Core.Application.Features.User.Commands.UpdateNotSensitiveUserInfoCommand;
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
            CreateMap<UpdateNotSensitiveUserInfoCommand, User>().ReverseMap();
            CreateMap<Role, RoleDto>().ReverseMap();
        }
    }
}
