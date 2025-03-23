using Gateway.Models;
using Gateway.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Gateway.Services
{
    public class OpenApiService : IOpenApiService
    {
        private readonly ICollection<ServiceConfig> _urlNamePairs;
        private readonly HttpClient _httpClient;

        public OpenApiService(IOptions<OpenApiServiceSettings> options, HttpClient httpClient) : this(options.Value, httpClient) { }

        public OpenApiService(OpenApiServiceSettings options, HttpClient httpClient)
        {
            var urlNamePairs = options?.ServiceConfigs;
            if (urlNamePairs == null || urlNamePairs.Count == 0) 
            {
                throw new ArgumentException($"{nameof(options)} can not be null or empty");
            }

            _urlNamePairs = urlNamePairs;

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GetDocumentationForService(string serviceName)
        {
            var urlNamePair = _urlNamePairs.FirstOrDefault(u => u.Name == serviceName) 
                ?? throw new KeyNotFoundException(serviceName);

            var url = $"{urlNamePair.DownstreamScheme}://{urlNamePair.Host}:{urlNamePair.Port}/{urlNamePair.SwaggerUrl}";

            var result = await _httpClient.GetAsync(url);
            if (!result.IsSuccessStatusCode)
            {
                throw new KeyNotFoundException(url + $" StatusCode: {result.StatusCode}");
            }

            var str = await result.Content.ReadAsStringAsync();

            str = str[0] + "\r\n\"swagger\": \"2.0\",\r\n" + str[1..];

            return str;
        }
    }
}
