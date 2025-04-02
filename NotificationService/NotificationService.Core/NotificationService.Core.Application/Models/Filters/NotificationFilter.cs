using System.Diagnostics.CodeAnalysis;

namespace NotificationService.Core.Application.Models.Filters
{
    public class NotificationFilter
    {
        [AllowNull]
        public IEnumerable<string>? Ids { get; set; }
        [AllowNull]
        public IEnumerable<string>? UserIds { get; set; }
        [AllowNull]
        public IEnumerable<string>? NotificationTypes { get; set; }
        public bool DateOrderDescending { get; set; } = true;
        [AllowNull]
        public DateTime? DateStart { get; set; }
        [AllowNull]
        public DateTime? DateEnd { get; set; }
    }
}
