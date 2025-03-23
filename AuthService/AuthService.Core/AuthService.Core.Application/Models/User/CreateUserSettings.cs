using System.ComponentModel.DataAnnotations;

namespace Application.Models.User
{
    public class CreateUserSettings : IValidatableObject
    {
        public int DefaultRoleID { get; set; }
        public double EmailConfirmationTimeoutHours { get; set; }
        /// <summary>
        /// Must contain two placeholders. First - for user email, Second - for user password
        /// </summary>
        public string BodyMessageFormat { get; set; } = null!;
        /// <summary>
        /// Must contain one placeholder - for user email
        /// </summary>
        public string KeyForRegistrationCachingFormat { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new();
            var minimalValue = 1d / 240d;
            if (EmailConfirmationTimeoutHours < minimalValue)
            {
                validationResults.Add(new ValidationResult($"{nameof(EmailConfirmationTimeoutHours)} has to be bigger than {minimalValue}",
                    new List<string>() { nameof(EmailConfirmationTimeoutHours) }));
            }

            try
            {
                var res = string.Format(BodyMessageFormat, "hello", "world");
            }
            catch (FormatException ex)
            {
                validationResults.Add(new(ex.Message + " Must contain two placeholders.",
                    new List<string>() { nameof(BodyMessageFormat) }));
            }

            try
            {
                var res = string.Format(KeyForRegistrationCachingFormat, "hello");
            }
            catch (FormatException ex)
            {
                validationResults.Add(new(ex.Message + " Must contain one placeholder.",
                    new List<string>() { nameof(KeyForRegistrationCachingFormat) }));
            }

            return validationResults;
        }
    }
}
