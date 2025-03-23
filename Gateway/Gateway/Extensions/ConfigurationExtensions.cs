using Gateway.Models;

namespace Gateway.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureOcelot(this IConfigurationBuilder configuration, ServicesSettings openApiServiceSettings)
        {
            var fileName = $"{Guid.NewGuid()}.json";
            FileInfo fileInfo = new(fileName);
            using var stream = fileInfo.Exists ? fileInfo.Open(FileMode.Truncate) : fileInfo.Create();

            using var sb = new StreamWriter(stream);
            sb.Write("{\"Routes\": [");

            var serviceConfigs = openApiServiceSettings.ServiceConfigs.Select(config =>
                "{" +
                    "\"UpstreamPathTemplate\": \"/" + config.Name + "/{everything}\"," +
                    "\"UpstreamHttpMethod\": [ \"Get\", \"Put\", \"Post\", \"Patch\", \"Delete\" ]," +
                    "\"DownstreamPathTemplate\": \"/" + config.Name + "/{everything}\"," +
                    $"\"DownstreamScheme\": \"{config.DownstreamScheme}\"," +
                    "\"DownstreamHostAndPorts\":" +
                    "[" +
                        "{" +
                            $"\"Host\": \"{config.Host}\"," +
                            $"\"Port\": {config.Port}" +
                        "}" +
                    "]" +
                "}");

            var joinedServiceConfigs = string.Join(", ", serviceConfigs);
            sb.Write(joinedServiceConfigs + "]}");

            sb.Close();
            stream.Close();

            configuration.AddJsonFile(fileInfo.FullName);
            fileInfo.Delete();
        }
    }
}
