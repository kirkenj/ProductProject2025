namespace AuthService.Core.Application.Models.User.Settings
{
    public class UpdateUserEmailSettings
    {
        public string UpdateUserEmailCacheKeyFormat { get; set; } = null!;
        public double EmailUpdateTimeOutHours { get; set; } = default;
    }
}
