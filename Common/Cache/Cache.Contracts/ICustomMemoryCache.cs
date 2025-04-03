namespace Cache.Contracts
{
    public interface ICustomMemoryCache
    {
        public Task SetAsync<T>(string key, T value, TimeSpan offset, CancellationToken cancellationToken = default);

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    }
}
