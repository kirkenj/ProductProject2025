using Clients.CustomGateway;
using Front.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Front.Services
{
    public class UserService(IAuthGatewayClient AuthHttpClient, LocalStorageAccessor authenticationDataMemoryStorage)
    {
        private readonly IAuthGatewayClient _authHttpClient = AuthHttpClient;
        private readonly LocalStorageAccessor _authenticationDataMemoryStorage = authenticationDataMemoryStorage;


        public async Task<User> SendAuthenticateRequestAsync(string email, string password)
        {
            var response = await _authHttpClient.LoginPOSTAsync(new LoginDto { Email = email, Password = password });
            string token = response.Token;
            var claimPrincipal = CreateClaimsPrincipalFromToken(token);
            var user = User.FromClaimsPrincipal(claimPrincipal);
            await PersistUserToBrowser(token);
            return user;
        }

        private static ClaimsPrincipal CreateClaimsPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var identity = new ClaimsIdentity();

            if (tokenHandler.CanReadToken(token))
            {
                var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
                identity = new(jwtSecurityToken.Claims, Consts.AuthTypeName);
            }

            return new(identity);
        }

        public async Task<User?> FetchUserFromBrowser()
        {
            var savedAuthToken = await GetAuthTokenAsync();
            if (string.IsNullOrEmpty(savedAuthToken))
            {
                return null;
            }

            var claimsPrincipal = CreateClaimsPrincipalFromToken(savedAuthToken);
            return User.FromClaimsPrincipal(claimsPrincipal);
        }

        private async Task PersistUserToBrowser(string token) => await SetAuthTokenAsync(token);

        public async Task ClearBrowserUserData() => await RemoveAuthTokenAsync();

        public async Task<string?> GetAuthTokenAsync()
        {
            return await _authenticationDataMemoryStorage.GetValueAsync<string?>(Consts.TokenStoragePath);
        }

        public async Task SetAuthTokenAsync(string token)
        {
            ArgumentNullException.ThrowIfNull(token);
            await _authenticationDataMemoryStorage.SetValueAsync<string?>(Consts.TokenStoragePath, token);
        }

        public async Task RemoveAuthTokenAsync()
        {
            await _authenticationDataMemoryStorage.RemoveAsync(Consts.TokenStoragePath);
        }
    }
}
