using Repository.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericRepository
{
    [TestFixture(typeof(GenericRepository<,>))]
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableRepository<,,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class GetAllAsyncTests : GenericRepositoryTest
    {
        public GetAllAsyncTests(Type reptype) : base(reptype) { }

        [Test]
        public async Task GetAllAsync_ReturnsValues()
        {
            var users = await _repository.GetAllAsync();

            Assert.That(users, Is.EquivalentTo(Users));
        }

        [TearDown]
        public void TearDown()
        {
            _testDbContext.Dispose();
        }
    }
}