using AutoMapper;
using Messaging.Messages.AuthService;
using NotificationService.Core.Application.Features.Notificatioin.AuthService.AccountConfirmed;
using NotificationService.Core.Application.Features.Notificatioin.AuthService.ChangeEmailRequest;
using NotificationService.Core.Application.Features.Notificatioin.AuthService.ForgotPassword;
using NotificationService.Core.Application.Features.Notificatioin.AuthService.UserRegistrationRequestCreated;

namespace NotificationService.Api.Consumers.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationRequestCreated, UserRegistrationRequestCreatedCommand>();
            CreateMap<ForgotPassword, ForgotPasswordCommand>();
            CreateMap<AccountConfirmed, AccountConfirmedCommand>();
            CreateMap<ChangeEmailRequest, ChangeEmailRequestCommand>();
        }
    }
}
