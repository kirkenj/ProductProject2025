using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;


namespace Front.Services.MessageHandlers
{
    public class TokenDelegatingHandler : AuthorizationMessageHandler
    {
        private readonly IAccessTokenProvider _accessTokenProvider;

        public TokenDelegatingHandler(IAccessTokenProvider provider, NavigationManager navigation, string authorizedUrl) : base(provider, navigation)
        {
            _accessTokenProvider = provider;
            ConfigureHandler(["http://240.0.0.0"]);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _accessTokenProvider.RequestAccessToken();
            if (token.Status == AccessTokenResultStatus.Success && token.TryGetToken(out AccessToken? accessToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Value);
                Console.WriteLine("Set access token");
            }
            else
            {
                Console.WriteLine("Couldn't set access token");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
