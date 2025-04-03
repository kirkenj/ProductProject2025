using Cache.Contracts;
using Microsoft.Extensions.Caching.Memory;


namespace Cache.Models.InMemory
{
    public class InMemoryCache : ICustomMemoryCache
    {
        private readonly IMemoryCache _implementation;

        public InMemoryCache(IMemoryCache memoryCache)
        {
            _implementation = memoryCache;
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var objRes = _implementation.Get(key.Trim());
            return Task.FromResult(objRes == null ? default : (T)objRes);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            _implementation.Remove(key.Trim());
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value, TimeSpan offset, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            ArgumentNullException.ThrowIfNull(value);

            _implementation.Set(key.Trim(), value, offset);
            return Task.CompletedTask;
        }
    }
}
