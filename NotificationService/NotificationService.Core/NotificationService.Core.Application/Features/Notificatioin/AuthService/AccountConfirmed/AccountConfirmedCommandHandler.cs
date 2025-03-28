using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.AccountConfirmed
{
    public class AccountConfirmedCommandHandler : IRequestHandler<AccountConfirmedCommand>
    {
        public Task Handle(AccountConfirmedCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
