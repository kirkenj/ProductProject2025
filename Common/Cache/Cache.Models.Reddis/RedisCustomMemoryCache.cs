using System.Text.Json;
using Cache.Contracts;
using Microsoft.Extensions.Caching.Distributed;

namespace Cache.Models.Reddis
{
    public class RedisCustomMemoryCache : ICustomMemoryCache
    {
        protected readonly IDistributedCache _implementation;

        public RedisCustomMemoryCache(IDistributedCache database)
        {
            ArgumentNullException.ThrowIfNull(database);
            _implementation = database;
        }

        public virtual async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var result = await _implementation.GetStringAsync(key.Trim(), cancellationToken);

            return string.IsNullOrWhiteSpace(result) ? default : JsonSerializer.Deserialize<T>(result.ToString());
        }

        public virtual async Task<bool> RefreshKeyAsync(string key, double millisecondsToExpire, CancellationToken cancellationToken = default)
        {
            var value = await _implementation.GetAsync(key, cancellationToken);

            if (value == null) 
            {
                return false;
            }

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMilliseconds(millisecondsToExpire));

            await _implementation.SetAsync(key, value, options, cancellationToken);

            return true;
        } 

        public virtual Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _implementation.RemoveAsync(key.Trim(), cancellationToken);
        }

        public virtual async Task SetAsync<T>(string key, T value, TimeSpan expirity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            ArgumentNullException.ThrowIfNull(value, nameof(value));

            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(expirity);

            await _implementation.SetStringAsync(key, JsonSerializer.Serialize(value), options, cancellationToken);
        }
    }
}