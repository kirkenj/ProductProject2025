using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.UserRegistrationRequestCreated
{
    public class UserRegistrationRequestCreatedCommand : Messaging.Messages.AuthService.UserRegistrationRequestCreated, IRequest
    {
    }
}
