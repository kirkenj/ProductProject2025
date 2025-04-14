using System.Text;
using Microsoft.Extensions.Configuration;

namespace ConfigurationExtensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static void AddEnvironmentVariablesCustom(this IConfigurationBuilder configuration, params string[] environmentVariableNames)
        {

            var keyValuePairStrings = environmentVariableNames.Select(name =>
            {
                var value = Environment.GetEnvironmentVariable(name) ?? throw new ArgumentException($"Couldn't get environment variable from {name}");
                return $"\"{name}\":{value}";
            });

            var result = "{" + string.Join(",", keyValuePairStrings) + "}";

            using var memStream = new MemoryStream();

            var bytes = Encoding.UTF8.GetBytes(result);

            memStream.Write(bytes, 0, bytes.Length);
            memStream.Position = 0;

            configuration.AddJsonStream(memStream);
        }
    }
}
