using Cache.Contracts;
using Cache.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Cache.Tests.Models
{
    public class TestBase
    {
        protected readonly ICustomMemoryCache _cache = null!;

        public TestBase(Type type)
        {
            if (type == typeof(RedisCustomMemoryCache))
            {
                _cache = new RedisCustomMemoryCache("localhost:3300");
            }
            else if (type == typeof(CustomMemoryCache))
            {
                _cache = new CustomMemoryCache(new MemoryCache(new MemoryCacheOptions()));
            }
        }
    }
}
