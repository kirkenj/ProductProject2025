using System.Text;
using Gateway.Models;

namespace Gateway.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureOcelot(this IConfigurationBuilder configuration, ServicesSettings openApiServiceSettings)
        {
            var sb = new StringBuilder();
            sb.Append("{\"Routes\": [");

            var serviceConfigs = openApiServiceSettings.ServiceConfigs.Select(config =>
                "{" +
                    "\"UpstreamPathTemplate\": \"/" + config.Name + "/{everything}\"," +
                    "\"UpstreamHttpMethod\": [ \"Get\", \"Put\", \"Post\", \"Patch\", \"Delete\", \"HEAD\", \"CONNECT\", \"OPTIONS\", \"TRACE\" ]," +
                    "\"DownstreamPathTemplate\": \"/" + config.Name + "/{everything}\"," +
                    $"\"DownstreamScheme\": \"{config.DownstreamScheme}\"," +
                    "\"DownstreamHostAndPorts\":" +
                    "[" +
                        "{" +
                            $"\"Host\": \"{config.Host}\"," +
                            $"\"Port\": \"{config.Port}\"" +
                        "}" +
                    "]" +
                "}");

            var joinedServiceConfigs = string.Join(", ", serviceConfigs);
            sb.Append(joinedServiceConfigs + "]}");
            Console.WriteLine(sb.ToString());
            var str = sb.ToString();

            using var memStream = new MemoryStream();

            var bytes = Encoding.UTF8.GetBytes(str);

            memStream.Write(bytes, 0, bytes.Length);

            memStream.Position = 0;

            configuration.AddJsonStream(memStream);
        }
    }
}
