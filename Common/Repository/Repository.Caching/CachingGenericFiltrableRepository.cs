using System.Text.Json;
using Cache.Contracts;
using Microsoft.Extensions.Logging;
using Repository.Contracts;

namespace Repository.Caching
{
    public class CachingGenericFiltrableRepository<T, TIdType, TFilter> :
        CachingGenericRepository<T, TIdType>,
        IGenericFiltrableRepository<T, TIdType, TFilter>
        where T : class, IIdObject<TIdType> where TIdType : struct
    {
        private readonly IGenericFiltrableRepository<T, TIdType, TFilter> _filtrableRepository;
        private readonly ILogger<CachingGenericFiltrableRepository<T, TIdType, TFilter>> _logger;

        public CachingGenericFiltrableRepository(
            IGenericFiltrableRepository<T, TIdType, TFilter> genericRepository,
            ICustomMemoryCache customMemoryCache,
            ILogger<CachingGenericFiltrableRepository<T, TIdType, TFilter>> logger
            ) : base(genericRepository, customMemoryCache, logger)
        {
            _filtrableRepository = genericRepository;
            _logger = logger;
        }

        public virtual async Task<T?> GetAsync(TFilter filter, CancellationToken cancellationToken = default)
        {
            var key = JsonSerializer.Serialize(filter);

            _logger.LogInformation("Got request: {key}", key);

            var cacheResult = await _customMemoryCache.GetAsync<T>(key, cancellationToken);

            if (cacheResult != null)
            {
                _logger.LogInformation("Request {key}. Found in cache", key);
                return cacheResult;
            }

            _logger.LogInformation("Request {key}. Requesting the database", key);

            var repResult = await _filtrableRepository.GetAsync(filter, cancellationToken);

            if (repResult != null)
            {
                await SetCacheAsync(key, repResult, cancellationToken);
            }

            return repResult;
        }

        public virtual async Task<IReadOnlyCollection<T>> GetPageContentAsync(TFilter filter, int? page = default, int? pageSize = default, CancellationToken cancellationToken = default)
        {
            var key = JsonSerializer.Serialize(filter) + $"page: {page}, pageSize: {pageSize}";

            _logger.LogInformation("Got request: {key}", key);

            var result = await _customMemoryCache.GetAsync<IReadOnlyCollection<T>>(key, cancellationToken);

            if (result != null)
            {
                _logger.LogInformation("Request {key}. Found in cache", key);
                return result;
            }

            _logger.LogInformation("Request {key}. Requesting the database", key);
            result = await _filtrableRepository.GetPageContentAsync(filter, page, pageSize, cancellationToken);

            var tasks = result.Select(item => SetCacheAsync(
                string.Format(CacheKeyFormatToAccessSingleViaId, item.Id, cancellationToken),
                item
                )
            ).Append(SetCacheAsync(key, result, cancellationToken));

            await Task.WhenAll(tasks);

            return result;
        }
    }
}
