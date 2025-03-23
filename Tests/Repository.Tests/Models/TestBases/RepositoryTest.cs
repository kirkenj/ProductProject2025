using Microsoft.Extensions.Logging;
using Moq;
using Repository.Contracts;
using Repository.Models;
using Repository.Tests.Common;

namespace Repository.Tests.Models.TestBases
{
    public class GenericRepositoryTest
    {
        protected List<User> Users => _testDbContext.Users.ToList();
        protected IGenericRepository<User, Guid> _repository = null!;
        protected readonly TestDbContext _testDbContext = null!;

        public GenericRepositoryTest(Type repType)
        {
            var contextTask = TestConstants.GetDbContextAsync();
            contextTask.Wait();
            _testDbContext = contextTask.Result;

            if (repType == typeof(GenericRepository<,>))
            {
                _repository = new GenericRepository<User, Guid>(_testDbContext);
            }
            else if (repType == typeof(GenericCachingRepository<,>))
            {
                var mockLogger = Mock.Of<ILogger<GenericCachingRepository<User, Guid>>>();

                _repository = new GenericCachingRepository<User, Guid>(_testDbContext, TestConstants.GetEmptyReddis(), mockLogger);
            }
            else if (repType == typeof(GenericFiltrableRepository<,,>))
            {
                _repository = new GenericFiltrableRepository<User, Guid, UserFilter>(_testDbContext, UserFilter.GetFilteredSet);
            }
            else if (repType == typeof(GenericFiltrableCachingRepository<,,>))
            {
                var mockLogger = Mock.Of<ILogger<GenericFiltrableCachingRepository<User, Guid, UserFilter>>>();

                _repository = new GenericFiltrableCachingRepository<User, Guid, UserFilter>(_testDbContext, TestConstants.GetEmptyReddis(), mockLogger, UserFilter.GetFilteredSet);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
