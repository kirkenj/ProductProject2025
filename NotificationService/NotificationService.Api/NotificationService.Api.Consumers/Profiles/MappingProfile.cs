using AutoMapper;
using Messaging.Messages.AuthService;
using NotificationService.Core.Application.Features.Notificatioin.AccountConfirmed;
using NotificationService.Core.Application.Features.Notificatioin.ChangeEmailRequest;
using NotificationService.Core.Application.Features.Notificatioin.ForgotPassword;
using NotificationService.Core.Application.Features.Notificatioin.UserRegistrationRequestCreated;

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
