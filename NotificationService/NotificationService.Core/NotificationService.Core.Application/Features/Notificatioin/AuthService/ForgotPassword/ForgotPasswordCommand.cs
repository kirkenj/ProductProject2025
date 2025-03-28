using MediatR;

namespace NotificationService.Core.Application.Features.Notificatioin.AuthService.ForgotPassword
{
    public class ForgotPasswordCommand : Messaging.Messages.AuthService.ForgotPassword, IRequest
    {
    }
}
