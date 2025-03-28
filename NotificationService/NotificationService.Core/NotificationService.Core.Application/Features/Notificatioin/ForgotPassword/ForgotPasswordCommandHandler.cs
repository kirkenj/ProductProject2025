using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        public Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
