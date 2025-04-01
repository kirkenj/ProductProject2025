using Cache.Contracts;
using Microsoft.Extensions.Caching.Memory;


namespace Cache.Models
{
    public class CustomMemoryCache : ICustomMemoryCache
    {
        private readonly IMemoryCache _implementation;

        public CustomMemoryCache(IMemoryCache memoryCache)
        {
            _implementation = memoryCache;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var objRes = _implementation.Get(key.Trim());
            return Task.FromResult(objRes == null ? default : (T)objRes);
        }

        public Task<bool> RefreshKeyAsync(string key, double millisecondsToExpire)
        {
            if (key == null)
            {
                throw new ArgumentException($"{nameof(key)} is null", nameof(key));
            }

            var objRes = _implementation.Get(key.Trim());
            if (objRes == null)
            {
                return Task.FromResult(false);
            }

            try
            {
                _implementation.Set(key, objRes, DateTimeOffset.UtcNow.AddMilliseconds(millisecondsToExpire));
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task RemoveAsync(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            _implementation.Remove(key.Trim());
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value, TimeSpan offset)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            ArgumentNullException.ThrowIfNull(value, nameof(value));

            var offsetDiff = offset.TotalMilliseconds;

            if (offsetDiff < 100)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), $"Offset has to be at least 100 ms (given offset is {offsetDiff})");
            }

            _implementation.Set(key.Trim(), value, offset);
            return Task.CompletedTask;
        }
    }
}
