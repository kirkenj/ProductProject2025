using Microsoft.Extensions.Logging;
using Moq;
using Repository.Contracts;
using Repository.Models;
using Repository.Tests.Common;

namespace Repository.Tests.Models.TestBases
{
    public class FiltrableRepositoryTest
    {
        protected List<User> Users => _testDbContext.Users.ToList();
        protected IGenericFiltrableRepository<User, Guid, UserFilter> _repository = null!;
        protected TestDbContext _testDbContext { get; set; } = null!;
        protected readonly Func<IQueryable<User>, UserFilter, IQueryable<User>> filterDelegate;

        public FiltrableRepositoryTest(Type repType)
        {
            var contextTask = TestConstants.GetDbContextAsync();
            contextTask.Wait();
            _testDbContext = contextTask.Result;
            filterDelegate = UserFilter.GetFilteredSet;

            if (repType == typeof(GenericFiltrableRepository<,,>))
            {
                _repository = new GenericFiltrableRepository<User, Guid, UserFilter>(_testDbContext, filterDelegate);
            }
            else if (repType == typeof(GenericFiltrableCachingRepository<,,>))
            {
                var mockLogger = Mock.Of<ILogger<GenericFiltrableCachingRepository<User, Guid, UserFilter>>>();

                _repository = new GenericFiltrableCachingRepository<User, Guid, UserFilter>(_testDbContext, TestConstants.GetEmptyReddis(), mockLogger, filterDelegate);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
