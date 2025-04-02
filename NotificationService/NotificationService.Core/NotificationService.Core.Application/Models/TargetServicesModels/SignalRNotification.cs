namespace NotificationService.Core.Application.Models.TargetServicesModels
{
    public class SignalRNotification
    {
        public string UserId { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
