using AutoMapper;
using Messaging.Messages.AuthService;
using NotificationService.Core.Application.Features.Notificatioin.UserRegistrationRequestCreated;

namespace NotificationService.Api.Consumers.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationRequestCreated, UserRegistrationRequestCreatedCommand>();
        }
    }
}
