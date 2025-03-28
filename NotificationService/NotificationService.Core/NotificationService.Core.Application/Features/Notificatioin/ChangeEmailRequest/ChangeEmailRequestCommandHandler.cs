using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ChangeEmailRequest
{
    public class ChangeEmailRequestCommandHandler : IRequestHandler<ChangeEmailRequestCommand>
    {
        public Task Handle(ChangeEmailRequestCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
