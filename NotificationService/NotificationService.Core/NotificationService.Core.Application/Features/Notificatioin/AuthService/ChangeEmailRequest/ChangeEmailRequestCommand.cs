using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.ChangeEmailRequest
{
    public class ChangeEmailRequestCommand : Messaging.Messages.AuthService.ChangeEmailRequest, IRequest
    {
    }
}
