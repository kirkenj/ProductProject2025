using NotificationService.Core.Domain.Contracts;

namespace NotificationService.Core.Application.Contracts.Application
{
    public interface IMediatRSendableNotification : INotification, MediatR.INotification
    {
    }
}
