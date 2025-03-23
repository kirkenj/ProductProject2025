using Microsoft.AspNetCore.Http;

namespace HttpDelegatingHandlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authResult = _httpContextAccessor.HttpContext?.Request.Headers.FirstOrDefault(h => h.Key == "Authorization");

            if (authResult.HasValue && authResult.Value.Value.Count != 0)
            {
                var r = authResult.Value.Value.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (r.Length != 2)
                {
                    throw new ApplicationException();
                }

                request.Headers.Authorization = new(r[0], r[1]);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
