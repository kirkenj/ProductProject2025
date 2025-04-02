using AutoMapper;
using Messaging.Messages.AuthService;
using NotificationService.Core.Application.Features.AuthService.ChangeEmailRequest;
using NotificationService.Core.Application.Features.AuthService.ForgotPassword;
using NotificationService.Core.Application.Features.AuthService.UserRegistrationRequestCreated;

namespace NotificationService.Api.Consumers.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationRequestCreated, UserRegistrationRequestCreatedCommand>();
            CreateMap<ForgotPassword, ForgotPasswordCommand>();
            CreateMap<ChangeEmailRequest, ChangeEmailRequestCommand>();
        }
    }
}
