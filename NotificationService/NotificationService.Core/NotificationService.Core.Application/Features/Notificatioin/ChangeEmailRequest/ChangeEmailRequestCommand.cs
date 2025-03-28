using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ChangeEmailRequest
{
    public class ChangeEmailRequestCommand : Messaging.Messages.AuthService.ChangeEmailRequest, IRequest
    {
    }
}
