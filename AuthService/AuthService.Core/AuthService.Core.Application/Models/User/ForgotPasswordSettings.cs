using System.ComponentModel.DataAnnotations;

namespace Application.Models.User
{
    public class ForgotPasswordSettings : IValidatableObject
    {
        /// <summary>
        /// Has to contain one placeholder - for new user's password
        /// </summary>
        public string EmailBodyFormat { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> list = new();

            try
            {
                _ = string.Format(EmailBodyFormat, 112);
            }
            catch (FormatException ex)
            {
                list.Add(new(ex.Message, new List<string> { nameof(EmailBodyFormat) }));
            }

            return list;
        }
    }
}
