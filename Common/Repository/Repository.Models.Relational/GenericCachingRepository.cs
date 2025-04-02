using Cache.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Contracts;

namespace Repository.Models.Relational
{
    public class GenericCachingRepository<T, TIdType> : IGenericRepository<T, TIdType> where T : class, IIdObject<TIdType> where TIdType : struct
    {
        protected readonly Guid _repId = Guid.NewGuid();

        private ILogger<GenericCachingRepository<T, TIdType>>? _logger;
        private GenericRepository<T, TIdType>? _repository;
        private ICustomMemoryCache? _customMemoryCache;

        protected GenericCachingRepository()
        {
        }

        public GenericCachingRepository(DbContext dbContext, ICustomMemoryCache customMemoryCache, ILogger<GenericCachingRepository<T, TIdType>> logger)
        {
            CustomMemoryCache = customMemoryCache;
            Repository = new GenericRepository<T, TIdType>(dbContext);
            Logger = logger;
        }

        protected ILogger<GenericCachingRepository<T, TIdType>> Logger
        {
            get => _logger ?? throw new ArgumentNullException(nameof(Logger));
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _logger = value;
            }
        }

        protected virtual GenericRepository<T, TIdType> Repository
        {
            get => _repository ?? throw new ArgumentNullException(nameof(Repository));
            private set
            {
                ArgumentNullException.ThrowIfNull(value);
                _repository = value;
            }
        }

        protected ICustomMemoryCache CustomMemoryCache
        {
            get => _customMemoryCache ?? throw new ArgumentNullException(nameof(CustomMemoryCache));
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _customMemoryCache = value;
            }
        }

        public int СacheTimeoutMiliseconds { get; set; } = 2_000;
        protected string CacheKeyPrefix => _repId + " ";
        protected string CacheKeyFormatToAccessSingleViaId => CacheKeyPrefix + "{0}";


        public async Task AddAsync(T obj, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(obj);

            await Repository.AddAsync(obj, cancellationToken);

            await SetCacheAsync(string.Format(CacheKeyFormatToAccessSingleViaId, obj.Id), obj);
        }

        public async Task DeleteAsync(TIdType id, CancellationToken cancellationToken = default)
        {
            await Repository.DeleteAsync(id, cancellationToken);

            var cacheKey = string.Format(CacheKeyFormatToAccessSingleViaId, id);

            await CustomMemoryCache.RemoveAsync(cacheKey, cancellationToken);

            Logger.Log(LogLevel.Information, $"Removed the key {cacheKey}");
        }

        public async Task<IReadOnlyCollection<T>> GetPageContent(int? page = default, int? pageSize = default, CancellationToken cancellationToken = default)
        {
            var key = CacheKeyPrefix + $"(page:{page}, pagesize:{pageSize}";

            var cacheResult = await CustomMemoryCache.GetAsync<IReadOnlyCollection<T>>(key, cancellationToken);

            if (cacheResult != null)
            {
                return cacheResult;
            }

            var result = await Repository.GetPageContent(page, pageSize, cancellationToken);

            var tasks = result.Select(r => SetCacheAsync(string.Format(CacheKeyFormatToAccessSingleViaId, r.Id), r, cancellationToken))
                .Append(CustomMemoryCache.SetAsync(key, result, TimeSpan.FromMilliseconds(СacheTimeoutMiliseconds), cancellationToken));

            await Task.WhenAll(tasks);

            return result;
        }

        public async Task<T?> GetAsync(TIdType id, CancellationToken cancellationToken = default)
        {
            Logger.Log(LogLevel.Information, $"Got request for {typeof(T).Name} with id = '{id}'. ");
            var cacheKey = string.Format(CacheKeyFormatToAccessSingleViaId, id);
            var result = await CustomMemoryCache.GetAsync<T>(cacheKey, cancellationToken);

            if (result != null)
            {
                Logger.Log(LogLevel.Information, $"Found it in cache.");
                return result;
            }

            Logger.Log(LogLevel.Information, "Sending request to database.");

            result = await Repository.GetAsync(id, cancellationToken);

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
                    Repository.UpdateAsync(obj, cancellationToken),
                    SetCacheAsync(CacheKeyPrefix + obj.Id, obj, cancellationToken)
                );
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var key = CacheKeyPrefix + "All";

            var cacheResult = await CustomMemoryCache.GetAsync<IReadOnlyCollection<T>>(key);

            if (cacheResult != null)
            {
                Logger.Log(LogLevel.Information, $"Key found in cache.");
                return cacheResult;
            }

            var result = await Repository.GetAllAsync(cancellationToken);

            var tasks = result.Select(r => SetCacheAsync(string.Format(CacheKeyFormatToAccessSingleViaId, r.Id), r, cancellationToken))
                .Append(SetCacheAsync(key, result, cancellationToken));

            await Task.WhenAll(tasks);

            return result;
        }

        protected virtual async Task SetCacheAsync(string key, object value, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(key);

            ArgumentNullException.ThrowIfNull(value);

            await CustomMemoryCache.SetAsync(key, value, TimeSpan.FromMilliseconds(СacheTimeoutMiliseconds), cancellationToken);

            Logger.Log(LogLevel.Information, $"Set cache with key '{key}'");
        }
    }
}
