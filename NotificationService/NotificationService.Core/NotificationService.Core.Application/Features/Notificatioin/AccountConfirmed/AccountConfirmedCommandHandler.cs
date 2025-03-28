using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AccountConfirmed
{
    public class AccountConfirmedCommandHandler : IRequestHandler<AccountConfirmedCommand>
    {
        public Task Handle(AccountConfirmedCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
