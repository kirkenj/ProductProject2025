using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Core.Domain.Contracts;
using Repository.Contracts;

namespace NotificationService.Core.Domain.Models
{
    public class Notification : IIdObject<string>, INotification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string NotificationJson { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
