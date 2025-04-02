namespace NotificationService.Core.Domain.Contracts
{
    public interface INotification
    {
        public string UserId { get; }
        public string DefaultBody { get; }
    }
}
