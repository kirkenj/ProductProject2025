using System.Text;
using AutoMapper;
using Cache.Contracts;
using Clients.AuthApi;
using Microsoft.Extensions.Logging;
using ProductService.Core.Application.Contracts.AuthService;
using ProductService.Core.Application.Models.UserClient;

namespace ProductService.Infrastucture.Infrastucture.AuthClient
{
    public class AuthClientService : IAuthApiClientService
    {
        private readonly IAuthApiClient _authClient;
        private readonly ICustomMemoryCache _customMemoryCache;
        private const string CACHE_KEY_PREFIX = "ProductAPI_AuthService_";
        private readonly IMapper _mapper;
        private readonly ILogger<AuthClientService> _logger;

        public AuthClientService(IAuthApiClient authClient, ICustomMemoryCache customMemoryCache, IMapper mapper, ILogger<AuthClientService> logger)
        {
            _customMemoryCache = customMemoryCache;
            _authClient = authClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ClientResponse<ICollection<AuthClientUser>>?> ListAsync(IEnumerable<Guid>? ids = null, string? accurateLogin = null, string? loginPart = null, string? accurateEmail = null, string? emailPart = null, string? address = null, string? name = null, IEnumerable<int>? roleIds = null, int? page = null, int? pageSize = null)
        {
            try
            {
                var parametersAsString = StringifyParameters(ids, accurateLogin, loginPart, accurateEmail, address, roleIds, page, pageSize);

                var cacheKey = CACHE_KEY_PREFIX + parametersAsString;

                _logger.LogInformation($"Sending request for {nameof(AuthClientUser)} with filter: {parametersAsString}");

                ICollection<AuthClientUser> result;

                var cacheResult = await _customMemoryCache.GetAsync<ICollection<AuthClientUser>>(cacheKey);

                if (cacheResult != null)
                {
                    _logger.LogInformation("Found it into cache");
                    return new ClientResponse<ICollection<AuthClientUser>> { Result = cacheResult, Success = true };
                }

                _logger.LogInformation($"Sending request to {nameof(AuthClientService)}");

                var qResult = await _authClient.ListAsync(ids, accurateLogin, loginPart, accurateEmail, emailPart, address, name, roleIds, page, pageSize);

                result = _mapper.Map<List<AuthClientUser>>(qResult);

                var tasks = result.Select(r => _customMemoryCache.SetAsync(CACHE_KEY_PREFIX + "userId_" + r.Id, result, TimeSpan.FromMilliseconds(10_000)))
                .Append(_customMemoryCache.SetAsync(cacheKey, result, TimeSpan.FromMilliseconds(10_000)));

                await Task.WhenAll(tasks);

                return new ClientResponse<ICollection<AuthClientUser>> { Result = result, Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new ClientResponse<ICollection<AuthClientUser>> { Message = ex.Message, Success = false };
            }
        }

        public async Task<ClientResponse<AuthClientUser?>> GetUser(Guid userId)
        {
            try
            {
                var cacheKey = CACHE_KEY_PREFIX + "userId_" + userId;

                _logger.LogInformation("Request for a {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                    nameof(AuthClientUser), nameof(AuthClientService), nameof(userId), userId);

                AuthClientUser result;

                _logger.LogInformation("Checking cache for a {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                                    nameof(AuthClientUser), nameof(AuthClientService), nameof(userId), userId);
                var cacheResult = await _customMemoryCache.GetAsync<AuthClientUser>(cacheKey);

                if (cacheResult != null)
                {
                    _logger.LogInformation("Found the {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                                        nameof(AuthClientUser), nameof(AuthClientService), nameof(userId), userId);
                    return new ClientResponse<AuthClientUser?> { Result = cacheResult, Success = true };
                }

                _logger.LogInformation("Sending a request for a {typeName} to {serviceName} with {propertyName} = {propertyVaue}",
                                        nameof(UserDto), nameof(AuthClientService), nameof(userId), userId);

                var response = await _authClient.UsersGETAsync(userId);

                _logger.LogInformation("Succcess response for a {typeName} to {serviceName} with {propertyName} = {propertyVaue}. Starting mapping into {targetTypeName}",
                                        nameof(UserDto), nameof(AuthClientService), nameof(userId), userId, nameof(AuthClientUser));

                result = _mapper.Map<AuthClientUser>(response);

                _logger.LogInformation("Setting cache value {typeName} with {propertyName} = {propertyVaue} using a key {cacheKey}",
                                    nameof(AuthClientUser), nameof(userId), userId, cacheKey);

                await _customMemoryCache.SetAsync(cacheKey, result, TimeSpan.FromMilliseconds(10_000));

                return new ClientResponse<AuthClientUser?> { Result = result, Success = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new ClientResponse<AuthClientUser?> { Message = ex.Message, Success = false };
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