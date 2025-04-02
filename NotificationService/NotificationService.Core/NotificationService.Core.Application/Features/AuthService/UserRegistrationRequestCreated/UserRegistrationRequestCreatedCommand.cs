using MediatR;

namespace NotificationService.Core.Application.Features.AuthService.UserRegistrationRequestCreated
{
    public class UserRegistrationRequestCreatedCommand : Messaging.Messages.AuthService.UserRegistrationRequestCreated, IRequest
    {
    }
}
