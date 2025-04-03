using AutoMapper;
using Messaging.Messages.AuthService;
using Messaging.Messages.ProductService;
using NotificationService.Core.Application.Features.AuthService.ChangeEmailRequest;
using NotificationService.Core.Application.Features.AuthService.ForgotPassword;
using NotificationService.Core.Application.Features.AuthService.UserRegistrationRequestCreated;
using NotificationService.Core.Application.Features.ProductService.ProductCreated;
using NotificationService.Core.Application.Features.ProductService.ProductDeleted;
using NotificationService.Core.Application.Features.ProductService.ProductProducerUpdated;

namespace NotificationService.Api.Consumers.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationRequestCreated, UserRegistrationRequestCreatedCommand>();
            CreateMap<ForgotPassword, ForgotPasswordCommand>();
            CreateMap<ChangeEmailRequest, ChangeEmailRequestCommand>();

            CreateMap<ProductCreated, ProductCreatedNotificationRequest>();
            CreateMap<ProductDeleted, ProductDeletedNotificationRequest>();
            CreateMap<ProductProducerUpdated, ProductProducerUpdatedNotificationRequest>();
        }
    }
}
