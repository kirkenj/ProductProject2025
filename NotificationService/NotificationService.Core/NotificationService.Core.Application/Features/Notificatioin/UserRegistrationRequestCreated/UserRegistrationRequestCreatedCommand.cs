using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.UserRegistrationRequestCreated
{
    public class UserRegistrationRequestCreatedCommand : Messaging.Messages.AuthService.UserRegistrationRequestCreated, IRequest
    {
    }
}
