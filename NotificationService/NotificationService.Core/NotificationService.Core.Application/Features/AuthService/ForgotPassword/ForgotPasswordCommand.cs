using MediatR;

namespace NotificationService.Core.Application.Features.AuthService.ForgotPassword
{
    public class ForgotPasswordCommand : Messaging.Messages.AuthService.ForgotPassword, IRequest
    {
    }
}
