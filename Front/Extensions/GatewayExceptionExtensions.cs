using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;

namespace Front.Extensions
{

    public static class GatewayExceptionExtensions
    {
        public static bool IsValidationException(this Clients.CustomGateway.GatewayException ex, out IEnumerable<ValidationFailure>? failures)
        {
            ArgumentNullException.ThrowIfNull(ex, nameof(ex));

            if (!ex.Response.StartsWith(nameof(ValidationException)))
            {
                failures = null;
                return false;
            }

            var notDeserializedErrors = ex.Response.Remove(0, $"{nameof(ValidationException)}: ".Length);

            failures = JsonSerializer.Deserialize<IEnumerable<ValidationFailure>>(notDeserializedErrors);

            return true;
        }
    }
}
