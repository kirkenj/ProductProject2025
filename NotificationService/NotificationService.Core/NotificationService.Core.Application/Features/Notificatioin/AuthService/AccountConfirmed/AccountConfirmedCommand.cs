using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.AccountConfirmed
{
    public class AccountConfirmedCommand : Messaging.Messages.AuthService.AccountConfirmed, IRequest
    {
    }
}
