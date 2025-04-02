﻿using System.Text.Json;
using Cache.Contracts;
using StackExchange.Redis;

namespace Cache.Models
{
    public class RedisCustomMemoryCache : ICustomMemoryCache
    {
        protected readonly IDatabase _implementation;
        private readonly ConnectionMultiplexer connection;


        public RedisCustomMemoryCache(string redisConnectionString)
        {
            ArgumentNullException.ThrowIfNull(redisConnectionString);

            connection = ConnectionMultiplexer.Connect(redisConnectionString);

            _implementation = connection.GetDatabase();

            var keyToCheckConnection = "hello";
            _implementation.StringSet(keyToCheckConnection, "World");
            _implementation.KeyDelete(keyToCheckConnection);
        }

        public virtual async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var result = await _implementation.StringGetAsync(key.Trim());

            if (result == default || result.HasValue == false || result.IsNullOrEmpty == true)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(result.ToString());
        }

        public virtual Task<bool> RefreshKeyAsync(string key, double millisecondsToExpire, CancellationToken cancellationToken = default) => _implementation.KeyExpireAsync(key, TimeSpan.FromMilliseconds(millisecondsToExpire));

        public virtual Task RemoveAsync(string key, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _implementation.KeyDeleteAsync(key.Trim());
        }

        public virtual async Task SetAsync<T>(string key, T value, TimeSpan expirity, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            ArgumentNullException.ThrowIfNull(value, nameof(value));

            var offsetDiff = expirity.TotalMilliseconds;

            if (offsetDiff < 100)
            {
                throw new ArgumentOutOfRangeException(nameof(expirity), $"Offset has to be at least 100 ms (given offset is {offsetDiff})");
            }

            await _implementation.StringSetAsync(key.Trim(), JsonSerializer.Serialize(value), expirity);
        }
    }
}
