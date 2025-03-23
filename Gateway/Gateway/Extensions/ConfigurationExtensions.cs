using System.Text;
using System.Text.Json.Nodes;
using Gateway.Models;

namespace Gateway.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void ConfigureOcelot(this IConfigurationBuilder configuration, OpenApiServiceSettings openApiServiceSettings)
        {
            var fileName = "ocelot.json";
            FileInfo fileInfo = new (fileName);
            using var stream = fileInfo.Exists ? fileInfo.Open(FileMode.Truncate) : fileInfo.Create();
            
            using var sb = new StreamWriter(stream);
            sb.Write("{\"Routes\": [");


        //    var f = "{
        //    "UpstreamPathTemplate": "/Product/{everything}",
        //    "UpstreamHttpMethod": ["Get", "Put", "Post", "Patch", "Delete"],
        //    "DownstreamPathTemplate": "/Product/{everything}",
        //    "DownstreamScheme": "https",
        //    "DownstreamHostAndPorts": [
        //        {
        //        "Host": "localhost",
        //            "Port": 7198
        //        }
        //    ]
        //}""";


            var strs = openApiServiceSettings.ServiceConfigs.Select(config =>
                "{\r\n" +
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

            var strs0 = string.Join(", ", strs);
            sb.Write(strs0);
            sb.Write("]}");


            sb.Close();
            stream.Close();


            configuration.AddJsonFile(fileInfo.FullName);
        }

        private static bool IsValidJSON(string str)
        {
            try
            {
                var tmpObj = JsonValue.Parse(str);
                return true;
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
