using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        public Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
