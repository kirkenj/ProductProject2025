using HashProvider.Contracts;
using Microsoft.Extensions.Logging;

namespace Clients.AuthApi
{
    public interface ITokenValidationClient
    {
        public Task<bool> IsTokenValid(string token);
    }

    public class TokenValidationClient : ITokenValidationClient
    {
        private readonly IAuthApiClient _authApiClient;

        private static IHashProvider? HashProvider;
        private readonly ILogger _logger = null!;

        public TokenValidationClient(IAuthApiClient authApiClient, ILogger<TokenValidationClient> logger)
        {
            _authApiClient = authApiClient;
            _logger = logger;
        }

        private async Task UpdateEncodingAndHashAlgoritm()
        {
            _logger.LogInformation($"Sending request to auth client for hashDefaults.");

            var defaults = await _authApiClient.HashProviderSettingsAsync();

            _logger.LogInformation($"Request to auth client for hashDefaults - Success.");

            HashProvider = new HashProvider.Models.HashProvider(new HashProvider.Models.HashProviderSettings { EncodingName = defaults.EncodingName, HashAlgorithmName = defaults.HashAlgorithmName });
        }

        public async Task<bool> IsTokenValid(string token)
        {
            if (HashProvider == null)
            {
                await UpdateEncodingAndHashAlgoritm();
            }

            var tokenHash = HashProvider?.GetHash(token) ?? throw new ApplicationException($"Couldn't get hash");

            return await _authApiClient.IsValidAsync(tokenHash);
        }
    }
}