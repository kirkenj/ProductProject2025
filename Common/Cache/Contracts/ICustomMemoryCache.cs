namespace Cache.Contracts
{
    public interface ICustomMemoryCache
    {
        public Task SetAsync<T>(string key, T value, TimeSpan offset);

        public Task RemoveAsync(string key);

        public Task<T?> GetAsync<T>(string key);

        public Task<bool> RefreshKeyAsync(string key, double millisecondsToExpire);
    }
}
