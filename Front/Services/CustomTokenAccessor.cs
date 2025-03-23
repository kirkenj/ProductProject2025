using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Front.Services
{
    public class CustomTokenAccessor : IAccessTokenProvider
    {
        private readonly CustomAuthStateProvider _stateProvider;
        private readonly UserService _userService;

        public CustomTokenAccessor(CustomAuthStateProvider customAuthState, UserService userService)
        {
            _stateProvider = customAuthState;
            _userService = userService;
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken()
        {
            Console.WriteLine("Access token. Trying to get the token");
            var aToken = new AccessToken();

            if (_stateProvider.CurrentUser == null)
            {
                await _stateProvider.GetAuthenticationStateAsync();
            }

            var getResult = _stateProvider.CurrentUser == null ? null : await _userService.GetAuthTokenAsync();
            aToken.Value = getResult ?? string.Empty;
            var status = string.IsNullOrEmpty(getResult) ? AccessTokenResultStatus.RequiresRedirect : AccessTokenResultStatus.Success;
            Console.WriteLine($"Access token. Ended up. Token is null: {string.IsNullOrEmpty(aToken.Value)}");
            return new AccessTokenResult(status, aToken, null, null);
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) => await RequestAccessToken();
    }
}
