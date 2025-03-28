using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.ForgotPassword
{
    public class ForgotPasswordCommand : Messaging.Messages.AuthService.ForgotPassword, IRequest
    {
    }
}
