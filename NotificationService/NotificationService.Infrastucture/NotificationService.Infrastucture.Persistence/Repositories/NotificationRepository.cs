using MongoDB.Driver;
using NotificationService.Core.Application.Contracts.Persistence;
using NotificationService.Core.Application.Models.Filters;
using NotificationService.Core.Domain.Models;
using Repository.Models.MongoDb;

namespace NotificationService.Infrastucture.Persistence.Repositories
{
    public class NotificationRepository : GenericFiltrableRepository<Notification, string, NotificationFilter>, INotificationRepository
    {
        public NotificationRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase, Filter)
        {
        }

        private static IQueryable<Notification> Filter(IQueryable<Notification> set, NotificationFilter filter)
        {
            set = filter.DateOrderDescending ? set.OrderByDescending(x => x.Date) : set.OrderBy(x => x.Date);

            if (filter.Ids != null && filter.Ids.Any())
            {
                set = set.Where(obj => filter.Ids.Contains(obj.Id));
            }

            if (filter.UserIds != null && filter.UserIds.Any())
            {
                set = set.Where(obj => filter.UserIds.Contains(obj.UserId));
            }

            if (filter.NotificationTypes != null && filter.NotificationTypes.Any())
            {
                set = set.Where(obj => filter.NotificationTypes.Contains(obj.NotificationType));
            }

            if (filter.DateStart != null)
            {
                set = set.Where(obj => obj.Date >= filter.DateStart);
            }

            if (filter.DateEnd != null)
            {
                set = set.Where(obj => obj.Date <= filter.DateEnd);
            }

            if (filter.UnRead.HasValue)
            {
                set = set.Where(obj => obj.IsRead != filter.UnRead.Value);
            }

            return set;
        }
    }
}
