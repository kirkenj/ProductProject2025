using MediatR;

namespace NotificationService.Core.Application.Features.AuthService.ChangeEmailRequest
{
    public class ChangeEmailRequestCommand : Messaging.Messages.AuthService.ChangeEmailRequest, IRequest
    {
    }
}
