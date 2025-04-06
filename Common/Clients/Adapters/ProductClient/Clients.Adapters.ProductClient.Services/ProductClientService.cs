using System.Net;
using AutoMapper;
using Cache.Contracts;
using Clients.Adapters.ProductClient.Contracts;
using Clients.ProductApi;
using CustomResponse;
using Microsoft.Extensions.Logging;

namespace Clients.Adapters.ProductClient.Services
{
    public class ProductClientService : IProductClientService
    {
        private readonly IProductApiClient _authClient;
        private readonly ICustomMemoryCache _customMemoryCache;
        private readonly string CACHE_KEY_PREFIX = "ProductServiceAdapter";
        private readonly IMapper _mapper;
        private readonly ILogger<ProductClientService> _logger;
        private readonly int CACHE_TTL_MILLISECONDS = 10_000;

        public ProductClientService(IProductApiClient authClient, ICustomMemoryCache customMemoryCache, IMapper mapper, ILogger<ProductClientService> logger)
        {
            _customMemoryCache = customMemoryCache;
            _authClient = authClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<Contracts.ProductDto?>> GetProduct(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var cacheKey = CACHE_KEY_PREFIX + "userId_" + userId;

                _logger.LogInformation("Request for a {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                    nameof(Contracts.ProductDto), nameof(ProductClientService), nameof(userId), userId);

                Contracts.ProductDto result;

                _logger.LogInformation("Checking cache for a {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                                    nameof(Contracts.ProductDto), nameof(ProductClientService), nameof(userId), userId);
                var cacheResult = await _customMemoryCache.GetAsync<Contracts.ProductDto>(cacheKey, cancellationToken);

                if (cacheResult != null)
                {
                    _logger.LogInformation("Found the {typeName} from {serviceName} with {propertyName} = {propertyVaue}",
                                        nameof(Contracts.ProductDto), nameof(ProductClientService), nameof(userId), userId);
                    return new Response<Contracts.ProductDto?> { Result = cacheResult, StatusCode = HttpStatusCode.OK };
                }

                _logger.LogInformation("Sending a request for a {typeName} to {serviceName} with {propertyName} = {propertyVaue}",
                                        nameof(Contracts.ProductDto), nameof(ProductClientService), nameof(userId), userId);

                var response = await _authClient.ProductGETAsync(userId, cancellationToken);

                _logger.LogInformation("Succcess response for a {typeName} to {serviceName} with {propertyName} = {propertyVaue}. Starting mapping into {targetTypeName}",
                                        nameof(Contracts.ProductDto), nameof(ProductClientService), nameof(userId), userId, nameof(Contracts.ProductDto));

                result = _mapper.Map<Contracts.ProductDto>(response);

                _logger.LogInformation("Setting cache value {typeName} with {propertyName} = {propertyVaue} using a key {cacheKey}",
                                    nameof(Contracts.ProductDto), nameof(userId), userId, cacheKey);

                await _customMemoryCache.SetAsync(cacheKey, result, TimeSpan.FromMilliseconds(CACHE_TTL_MILLISECONDS), cancellationToken);

                return new Response<Contracts.ProductDto?> { Result = result, StatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new Response<Contracts.ProductDto?> { Message = ex.Message, StatusCode = HttpStatusCode.InternalServerError };
            }
        }
    }
}