using System.ComponentModel.DataAnnotations;

namespace Application.Models.User
{
    public class UpdateUserEmailSettings : IValidatableObject
    {
        public string UpdateUserEmailMessageBodyFormat { get; set; } = null!;
        public string UpdateUserEmailCacheKeyFormat { get; set; } = null!;
        public double EmailUpdateTimeOutHours { get; set; } = default;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> result = new();

            try
            {
                _ = string.Format(UpdateUserEmailCacheKeyFormat, EmailUpdateTimeOutHours);
            }
            catch (FormatException ex)
            {
                result.Add(new(ex.Message, new List<string> { nameof(UpdateUserEmailMessageBodyFormat) }));
            }
            try
            {
                _ = string.Format(UpdateUserEmailMessageBodyFormat, EmailUpdateTimeOutHours);
            }
            catch (FormatException ex)
            {
                result.Add(new(ex.Message, new List<string> { nameof(UpdateUserEmailCacheKeyFormat) }));
            }

            var timeoutMinimalValue = 1d / 240d;
            if (EmailUpdateTimeOutHours < timeoutMinimalValue)
            {
                result.Add(new($"EmailUpdateTimeOutHours has to be more than {timeoutMinimalValue}", new List<string> { nameof(UpdateUserEmailCacheKeyFormat) }));
            }

            return result;
        }
    }
}
