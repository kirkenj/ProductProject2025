using Front.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Front.Services
{
    public class CustomAuthStateProvider :
    AuthenticationStateProvider, IDisposable
    {
        private readonly UserService _blazorSchoolUserService;
        private readonly Guid _Id;

        public CustomAuthStateProvider(UserService blazorSchoolUserService, ILogger<CustomAuthStateProvider> logger)
        {
            _Id = Guid.NewGuid();
            _blazorSchoolUserService = blazorSchoolUserService;
            AuthenticationStateChanged += OnAuthenticationStateChangedAsync;
            logger.LogWarning($"Created {nameof(CustomAuthStateProvider)} with id {_Id}");
        }

        public User? CurrentUser { get; private set; }

        public async Task LoginAsync(string email, string password)
        {
            Console.WriteLine($"{nameof(CustomAuthStateProvider)} {_Id} {nameof(LoginAsync)}");
            var user = await _blazorSchoolUserService.SendAuthenticateRequestAsync(email, password);

            var principal = user.ToClaimsPrincipal();

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            var principal = new ClaimsPrincipal();
            var user = await _blazorSchoolUserService.FetchUserFromBrowser();
            bool notifyAuthStateChanged = false;

            if (user != null)
            {
                principal = user.ToClaimsPrincipal();
                notifyAuthStateChanged = !user.Equals(CurrentUser);
            }

            AuthenticationState authState = new(principal);

            Console.WriteLine($"{nameof(CustomAuthStateProvider)} {_Id} {nameof(GetAuthenticationStateAsync)} notifyStateChanged {notifyAuthStateChanged}");

            if (notifyAuthStateChanged)
            {
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
            }

            return authState;
        }


        public async void Logout()
        {
            Console.WriteLine($"{nameof(CustomAuthStateProvider)} {_Id} {nameof(Logout)}");
            await _blazorSchoolUserService.ClearBrowserUserData();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new())));
        }

        private async void OnAuthenticationStateChangedAsync(Task<AuthenticationState> task)
        {
            var authenticationState = await task;
            Console.WriteLine("Auth state changed");

            if (authenticationState is not null)
            {
                try
                {
                    CurrentUser = User.FromClaimsPrincipal(authenticationState.User);
                }
                catch (ArgumentException)
                {
                    CurrentUser = null;
                }
            }
        }

        public void Dispose()
        {
            Console.WriteLine($"{nameof(CustomAuthStateProvider)} {_Id} {nameof(Dispose)}");
            AuthenticationStateChanged -= OnAuthenticationStateChangedAsync;
            GC.SuppressFinalize(this);
        }
    }
}
