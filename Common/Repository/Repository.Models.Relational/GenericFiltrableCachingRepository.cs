using System.Text.Json;
using Cache.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repository.Contracts;

namespace Repository.Models.Relational
{
    public class GenericFiltrableCachingRepository<T, TIdType, TFilter> :
        GenericCachingRepository<T, TIdType>,
        IGenericFiltrableRepository<T, TIdType, TFilter>
        where T : class, IIdObject<TIdType> where TIdType : struct
    {

        public GenericFiltrableCachingRepository(DbContext dbContext, ICustomMemoryCache customMemoryCache, ILogger<GenericCachingRepository<T, TIdType>> logger, Func<IQueryable<T>, TFilter, IQueryable<T>> getFilteredSetDelegate)
        {
            FiltrableRepository = new(dbContext, getFilteredSetDelegate);
            CustomMemoryCache = customMemoryCache;
            Logger = logger;
        }

        protected GenericFiltrableRepository<T, TIdType, TFilter> FiltrableRepository { get; set; }

        protected override GenericRepository<T, TIdType> Repository => FiltrableRepository;

        public virtual async Task<T?> GetAsync(TFilter filter, CancellationToken cancellationToken = default)
        {
            var key = JsonSerializer.Serialize(filter) + "First";

            Logger.LogInformation($"Got request: {key}");

            var cacheResult = await CustomMemoryCache.GetAsync<T>(key);

            if (cacheResult != null)
            {
                Logger.LogInformation($"Request {key}. Found in cache");
                return cacheResult;
            }

            Logger.LogInformation($"Request {key}. Requesting the database");

            var repResult = await FiltrableRepository.GetAsync(filter, cancellationToken);

            if (repResult != null)
            {
                await SetCacheAsync(key, repResult);
            }

            return repResult;
        }


        public virtual async Task<IReadOnlyCollection<T>> GetPageContent(TFilter filter, int? page = default, int? pageSize = default, CancellationToken cancellationToken = default)
        {
            var key = JsonSerializer.Serialize(filter) + $"page: {page}, pageSize: {pageSize}";

            Logger.LogInformation($"Got request: {key}");

            var result = await CustomMemoryCache.GetAsync<IReadOnlyCollection<T>>(key, cancellationToken);

            if (result != null)
            {
                Logger.LogInformation($"Request {key}. Found in cache");
                return result;
            }

            Logger.LogInformation($"Request {key}. Requesting the database");
            result = await FiltrableRepository.GetPageContent(filter, page, pageSize);

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
