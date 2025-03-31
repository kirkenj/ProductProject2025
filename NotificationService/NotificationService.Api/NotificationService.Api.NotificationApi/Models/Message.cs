namespace NotificationService.Api.NotificationApi.Models
{
    public class Message
    {
        public Guid UserId { get; set; }
        public string Body { get; set; } = null!;
    }
}
