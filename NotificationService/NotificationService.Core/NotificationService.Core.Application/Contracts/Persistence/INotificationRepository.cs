using NotificationService.Core.Application.Models.Filters;
using NotificationService.Core.Domain.Models;
using Repository.Contracts;

namespace NotificationService.Core.Application.Contracts.Persistence
{
    public interface INotificationRepository : IGenericFiltrableRepository<Notification, string, NotificationFilter>
    {
    }
}
