using System.Net;
using System.Text;
using AutoMapper;
using Cache.Contracts;
using Clients.Adapters.AuthClient.Contracts;
using Clients.AuthApi;
using CustomResponse;
using Microsoft.Extensions.Logging;

namespace Clients.Adapters.AuthClient.Services
{
    public class AuthClientService : IAuthApiClientService
    {
        private readonly IAuthApiClient _authClient;
        private readonly ICustomMemoryCache _customMemoryCache;
        private readonly string CACHE_KEY_PREFIX = "AuthServiceAdapter";
        private readonly IMapper _mapper;
        private readonly ILogger<AuthClientService> _logger;
        private readonly int CACHE_TTL_MILLISECONDS = 10_000;

        public AuthClientService(IAuthApiClient authClient, ICustomMemoryCache customMemoryCache, IMapper mapper, ILogger<AuthClientService> logger)
        {
            _customMemoryCache = customMemoryCache;
            _authClient = authClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ICollection<AuthClientUser>>?> ListAsync(IEnumerable<Guid>? ids = null, string? accurateLogin = null, string? loginPart = null, string? accurateEmail = null, string? emailPart = null, string? address = null, string? name = null, IEnumerable<int>? roleIds = null, int? page = null, int? pageSize = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var parametersAsString = StringifyParameters(ids, accurateLogin, loginPart, accurateEmail, address, roleIds, page, pageSize);

                var cacheKey = CACHE_KEY_PREFIX + parametersAsString;

                _logger.LogInformation("Sending request for {typeName} with filter: {parametersAsString}", nameof(AuthClientUser), parametersAsString);

                ICollection<AuthClientUser> result;

                var cacheResult = await _customMemoryCache.GetAsync<ICollection<AuthClientUser>>(cacheKey, cancellationToken);
                if (cacheResult != null)
                {
                    _logger.LogInformation("Found it into cache");
                    return new Response<ICollection<AuthClientUser>> { Result = cacheResult, StatusCode = HttpStatusCode.OK };
                }

                _logger.LogInformation("Sending request to {serviceName}", nameof(AuthClientService));

                var qResult = await _authClient.ListAsync(ids, accurateLogin, loginPart, accurateEmail, emailPart, address, name, roleIds, page, pageSize, cancellationToken);

                result = _mapper.Map<List<AuthClientUser>>(qResult);

                var tasks = result.Select(r => _customMemoryCache.SetAsync(CACHE_KEY_PREFIX + "userId_" + r.Id, result, TimeSpan.FromMilliseconds(CACHE_TTL_MILLISECONDS), cancellationToken))
                    .Append(_customMemoryCache.SetAsync(cacheKey, result, TimeSpan.FromMilliseconds(CACHE_TTL_MILLISECONDS), cancellationToken));

                await Task.WhenAll(tasks);

                return new Response<ICollection<AuthClientUser>> { Result = result, StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new Response<ICollection<AuthClientUser>> { Message = ex.Message, StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        public async Task<Response<AuthClientUser?>> GetUser(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = CACHE_KEY_PREFIX + "userId_" + userId;

                _logger.LogInformation("Request for a {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                    nameof(AuthClientUser), nameof(AuthClientService), nameof(userId), userId);

                AuthClientUser result;

                _logger.LogInformation("Checking cache for a {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                                    nameof(AuthClientUser), nameof(AuthClientService), nameof(userId), userId);
                var cacheResult = await _customMemoryCache.GetAsync<AuthClientUser>(cacheKey, cancellationToken);
                if (cacheResult != null)
                {
                    _logger.LogInformation("Found the {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                                        nameof(AuthClientUser), nameof(AuthClientService), nameof(userId), userId);
                    return new Response<AuthClientUser?> { Result = cacheResult, StatusCode = HttpStatusCode.OK };
                }

                _logger.LogInformation("Sending a request for a {typeName} to {serviceName} with {propertyName} = {propertyVaue}",
                                        nameof(UserDto), nameof(AuthClientService), nameof(userId), userId);

                var response = await _authClient.UsersGETAsync(userId, cancellationToken);

                _logger.LogInformation("Succcess response for a {typeName} to {serviceName} with {propertyName} = {propertyVaue}. Starting mapping into {targetTypeName}",
                                        nameof(UserDto), nameof(AuthClientService), nameof(userId), userId, nameof(AuthClientUser));

                result = _mapper.Map<AuthClientUser>(response);

                _logger.LogInformation("Setting cache value {typeName} with {propertyName} = {propertyVaue} using a key {cacheKey}",
                                    nameof(AuthClientUser), nameof(userId), userId, cacheKey);

                await _customMemoryCache.SetAsync(cacheKey, result, TimeSpan.FromMilliseconds(CACHE_TTL_MILLISECONDS), cancellationToken);

                return new Response<AuthClientUser?> { Result = result, StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new Response<AuthClientUser?> { Message = ex.Message, StatusCode = HttpStatusCode.InternalServerError };
            }
        }

        private static string StringifyParameters(IEnumerable<Guid>? ids = null, string? accurateLogin = null, string? loginPart = null, string? email = null, string? address = null, IEnumerable<int>? roleIds = null, int? page = null, int? pageSize = null)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append(ids != null && ids.Any() ? $"Ids: {string.Join(',', ids)}; " : string.Empty);
            stringBuilder.Append(!string.IsNullOrEmpty(accurateLogin) ? $"AccurateLogin: {accurateLogin}; " : string.Empty);
            stringBuilder.Append(!string.IsNullOrEmpty(loginPart) ? $"LoginPart: {loginPart}; " : string.Empty);
            stringBuilder.Append(!string.IsNullOrEmpty(email) ? $"Email: {email}; " : string.Empty);
            stringBuilder.Append(!string.IsNullOrEmpty(address) ? $"Address: {address}; " : string.Empty);
            stringBuilder.Append(roleIds != null && roleIds.Any() ? $"roleIds: {string.Join(',', roleIds)}; " : string.Empty);
            stringBuilder.Append(page.HasValue ? $"page: {page.Value}; " : string.Empty);
            stringBuilder.Append(pageSize.HasValue ? $"pageSize: {pageSize.Value}; " : string.Empty);

            return stringBuilder.ToString();
        }
    }
}