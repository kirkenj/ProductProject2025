using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AccountConfirmed
{
    public class AccountConfirmedCommand : Messaging.Messages.AuthService.AccountConfirmed, IRequest
    {
    }
}
