using Cache.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Contracts;

namespace Repository.Models
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


        public async Task AddAsync(T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            await Repository.AddAsync(obj);

            await SetCacheAsync(string.Format(CacheKeyFormatToAccessSingleViaId, obj.Id), obj);
        }

        public async Task DeleteAsync(TIdType id)
        {
            await Repository.DeleteAsync(id);

            var cacheKey = string.Format(CacheKeyFormatToAccessSingleViaId, id);

            await CustomMemoryCache.RemoveAsync(cacheKey);

            Logger.Log(LogLevel.Information, $"Removed the key {cacheKey}");
        }

        public async Task<IReadOnlyCollection<T>> GetPageContent(int? page = default, int? pageSize = default)
        {
            var key = CacheKeyPrefix + $"(page:{page}, pagesize:{pageSize}";

            var cacheResult = await CustomMemoryCache.GetAsync<IReadOnlyCollection<T>>(key);

            if (cacheResult != null)
            {
                return cacheResult;
            }

            var result = await Repository.GetPageContent(page, pageSize);

            var tasks = result.Select(r => SetCacheAsync(string.Format(CacheKeyFormatToAccessSingleViaId, r.Id), r))
                .Append(CustomMemoryCache.SetAsync(key, result, TimeSpan.FromMilliseconds(СacheTimeoutMiliseconds)));

            await Task.WhenAll(tasks);

            return result;
        }

        public async Task<T?> GetAsync(TIdType id)
        {
            Logger.Log(LogLevel.Information, $"Got request for {typeof(T).Name} with id = '{id}'. ");
            var cacheKey = string.Format(CacheKeyFormatToAccessSingleViaId, id);
            var result = await CustomMemoryCache.GetAsync<T>(cacheKey);

            if (result != null)
            {
                Logger.Log(LogLevel.Information, $"Found it in cache.");
                return result;
            }

            Logger.Log(LogLevel.Information, "Sending request to database.");

            result = await Repository.GetAsync(id);

            if (result != null)
            {
                await SetCacheAsync(cacheKey, result);
            }

            return result;
        }

        public async Task UpdateAsync(T obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            await Task.WhenAll
                (
                    Repository.UpdateAsync(obj),
                    SetCacheAsync(CacheKeyPrefix + obj.Id, obj)
                );
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            var key = CacheKeyPrefix + "All";

            var cacheResult = await CustomMemoryCache.GetAsync<IReadOnlyCollection<T>>(key);

            if (cacheResult != null)
            {
                Logger.Log(LogLevel.Information, $"Key found in cache.");
                return cacheResult;
            }

            var result = await Repository.GetAllAsync();

            var tasks = result.Select(r => SetCacheAsync(string.Format(CacheKeyFormatToAccessSingleViaId, r.Id), r))
                .Append(SetCacheAsync(key, result));

            await Task.WhenAll(tasks);

            return result;
        }

        protected virtual async Task SetCacheAsync(string key, object value)
        {
            ArgumentNullException.ThrowIfNull(key);

            ArgumentNullException.ThrowIfNull(value);

            await CustomMemoryCache.SetAsync(key, value, TimeSpan.FromMilliseconds(СacheTimeoutMiliseconds));

            Logger.Log(LogLevel.Information, $"Set cache with key '{key}'");
        }
    }
}
