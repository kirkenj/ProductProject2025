using Microsoft.Extensions.Logging;
using Moq;
using Repository.Models;
using Repository.Tests.Common;

namespace Repository.Tests.Models.TestBases
{
    public class CachingFiltrableRepositoryTest
    {
        protected List<User> Users => _testDbContext.Users.ToList();
        protected GenericFiltrableCachingRepository<User, Guid, UserFilter> _repository = null!;
        protected RedisCustomMemoryCacheWithEvents _customMemoryCache;
        protected readonly TestDbContext _testDbContext = null!;

        public CachingFiltrableRepositoryTest()
        {
            var mockLogger = Mock.Of<ILogger<GenericCachingRepository<User, Guid>>>();
            var contextTask = TestConstants.GetDbContextAsync();
            _customMemoryCache = TestConstants.GetEmptyReddis();
            contextTask.Wait();
            _testDbContext = contextTask.Result;

            _repository = new GenericFiltrableCachingRepository<User, Guid, UserFilter>(_testDbContext, _customMemoryCache, mockLogger, UserFilter.GetFilteredSet);
        }
    }
}