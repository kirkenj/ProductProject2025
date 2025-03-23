using Repository.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericRepository
{
    [TestFixture(typeof(GenericRepository<,>))]
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableRepository<,,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class GetAsyncTests : GenericRepositoryTest
    {
        public GetAsyncTests(Type repType) : base(repType) { }

        [Test]
        public async Task GetAsync_IDdefault_ReturnsNull()
        {
            var users = await _repository.GetAsync(default);

            Assert.That(users, Is.Null);
        }

        [Test]
        public async Task GetAsync_IDNotContained_ReturnsNull()
        {
            var user = await _repository.GetAsync(Guid.NewGuid());

            Assert.That(user, Is.Null);
        }

        [Test]
        public async Task GetAsync_IDContained_ReturnsTheUser()
        {
            var user = Users.First();

            var users = await _repository.GetAsync(user.Id);

            Assert.That(users, Is.EqualTo(user));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _testDbContext.Dispose();
        }
    }
}