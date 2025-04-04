using Cache.Contracts;
using Microsoft.Extensions.Logging;
using Repository.Contracts;

namespace Repository.Caching
{
    public class CachingGenericRepository<T, TIdType> : IGenericRepository<T, TIdType> where T : class, IIdObject<TIdType> where TIdType : struct
    {
        private readonly IGenericRepository<T, TIdType> _repository;
        private readonly ILogger<CachingGenericRepository<T, TIdType>> _logger;
        protected readonly ICustomMemoryCache _customMemoryCache;

        public CachingGenericRepository(IGenericRepository<T, TIdType> genericRepository, ICustomMemoryCache customMemoryCache, ILogger<CachingGenericRepository<T, TIdType>> logger)
        {
            _customMemoryCache = customMemoryCache;
            _repository = genericRepository;
            _logger = logger;
        }

        public int СacheTimeoutMiliseconds { get; set; } = 2_000;
        protected string CacheKeyPrefix => nameof(CachingGenericRepository<T, TIdType>);
        protected string CacheKeyFormatToAccessSingleViaId => CacheKeyPrefix + "{0}";

        public async Task AddAsync(T obj, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(obj);

            await _repository.AddAsync(obj, cancellationToken);

            await SetCacheAsync(string.Format(CacheKeyFormatToAccessSingleViaId, obj.Id), obj, cancellationToken);
        }

        public async Task DeleteAsync(TIdType id, CancellationToken cancellationToken = default)
        {
            await _repository.DeleteAsync(id, cancellationToken);

            var cacheKey = string.Format(CacheKeyFormatToAccessSingleViaId, id);

            await _customMemoryCache.RemoveAsync(cacheKey, cancellationToken);

            _logger.Log(LogLevel.Information, "Removed the key {cacheKey}", cacheKey);
        }

        public async Task<IReadOnlyCollection<T>> GetPageContent(int? page = default, int? pageSize = default, CancellationToken cancellationToken = default)
        {
            var key = CacheKeyPrefix + $"(page:{page}, pagesize:{pageSize}";

            var cacheResult = await _customMemoryCache.GetAsync<IReadOnlyCollection<T>>(key, cancellationToken);

            if (cacheResult != null)
            {
                return cacheResult;
            }

            var result = await _repository.GetPageContent(page, pageSize, cancellationToken);

            var tasks = result.Select(r => SetCacheAsync(string.Format(CacheKeyFormatToAccessSingleViaId, r.Id), r, cancellationToken))
                .Append(_customMemoryCache.SetAsync(key, result, TimeSpan.FromMilliseconds(СacheTimeoutMiliseconds), cancellationToken));

            await Task.WhenAll(tasks);

            return result;
        }

        public async Task<T?> GetAsync(TIdType id, CancellationToken cancellationToken = default)
        {
            _logger.Log(LogLevel.Information, "Got request for {objType} with id = '{objId}'. ", typeof(T).Name, id);
            var cacheKey = string.Format(CacheKeyFormatToAccessSingleViaId, id);
            var result = await _customMemoryCache.GetAsync<T>(cacheKey, cancellationToken);

            if (result != null)
            {
                _logger.LogInformation($"Found it in cache.");
                return result;
            }

            _logger.LogInformation("Sending request to database.");

            result = await _repository.GetAsync(id, cancellationToken);

            if (result != null)
            {
                await SetCacheAsync(cacheKey, result, cancellationToken);
            }

            return result;
        }

        public async Task UpdateAsync(T obj, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(obj);

            await Task.WhenAll
                (
                    _repository.UpdateAsync(obj, cancellationToken),
                    SetCacheAsync(CacheKeyPrefix + obj.Id, obj, cancellationToken)
                );
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var cacheKeyAll = CacheKeyPrefix + "All";

            var cacheResult = await _customMemoryCache.GetAsync<IReadOnlyCollection<T>>(cacheKeyAll, cancellationToken);

            if (cacheResult != null)
            {
                _logger.LogInformation($"Key found in cache.");
                return cacheResult;
            }

            var result = await _repository.GetAllAsync(cancellationToken);

            var tasks = result.Select(r =>
            {
                var singleKey = string.Format(CacheKeyFormatToAccessSingleViaId, r.Id);
                return SetCacheAsync(singleKey, r, cancellationToken);
            })
                .Append(SetCacheAsync(cacheKeyAll, result, cancellationToken));

            await Task.WhenAll(tasks);

            return result;
        }

        protected virtual async Task SetCacheAsync(string key, object value, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(key);

            ArgumentNullException.ThrowIfNull(value);

            await _customMemoryCache.SetAsync(key, value, TimeSpan.FromMilliseconds(СacheTimeoutMiliseconds), cancellationToken);

            _logger.LogInformation("Set cache with key '{key}'", key);
        }
    }
}