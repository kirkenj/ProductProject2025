using System.Text;
using CustomResponse;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CentralizedJwtAuthentication
{
    public class JwtValidatingService : IJwtValidatingService
    {
        private readonly HttpClient _httpClient;
        private readonly JwtSettings _options;
        private readonly ILogger<JwtValidatingService> _logger;

        public JwtValidatingService(HttpClient httpClient, IOptions<JwtSettings> options, ILogger<JwtValidatingService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<Response<bool>> IsValid(string token)
        {
            try
            {
                HttpRequestMessage httpRequest = new()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(_options.IssuerTokenValidatePostMethodUri),
                    Content = new StringContent("\"" + token + "\"", Encoding.UTF8, "application/json")
                };

                var result = await _httpClient.SendAsync(httpRequest);
                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadAsStringAsync();
                    return Response<bool>.OkResponse(bool.Parse(response), string.Empty);
                }

                _logger.LogError(result.ReasonPhrase);
                return Response<bool>.ServerErrorResponse(result.ReasonPhrase!);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return Response<bool>.ServerErrorResponse(ex.Message);
            }

        }
    }
}
