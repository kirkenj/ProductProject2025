using Microsoft.Extensions.Logging;
using Moq;
using Repository.Models;
using Repository.Tests.Common;

namespace Repository.Tests.Models.TestBases
{
    public class CachingRepositoryTest
    {
        protected List<User> Users => _testDbContext.Users.ToList();
        protected GenericCachingRepository<User, Guid> _repository = null!;
        protected RedisCustomMemoryCacheWithEvents _customMemoryCache;
        protected readonly TestDbContext _testDbContext = null!;

        public CachingRepositoryTest(Type repType)
        {
            var mockLogger = Mock.Of<ILogger<GenericCachingRepository<User, Guid>>>();
            var contextTask = TestConstants.GetDbContextAsync();
            _customMemoryCache = TestConstants.GetEmptyReddis();
            contextTask.Wait();
            _testDbContext = contextTask.Result;

            if (repType == typeof(GenericCachingRepository<,>))
            {
                _repository = new GenericCachingRepository<User, Guid>(_testDbContext, _customMemoryCache, mockLogger);
            }
            else if (repType == typeof(GenericFiltrableCachingRepository<,,>))
            {
                _repository = new GenericFiltrableCachingRepository<User, Guid, UserFilter>(_testDbContext, _customMemoryCache, mockLogger, UserFilter.GetFilteredSet);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
