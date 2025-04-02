using AutoMapper;
using NotificationService.Core.Application.Models.Dtos;
using NotificationService.Core.Domain.Models;

namespace NotificationService.Core.Application.Profiles
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {
            CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DefaultBody))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.NotificationType));
        }
    }
}
