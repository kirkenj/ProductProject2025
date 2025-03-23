using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericCachingRepository
{
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class CachingDeleteAsyncTests : CachingRepositoryTest
    {
        public CachingDeleteAsyncTests(Type type) : base(type) { }

        [SetUp]
        public void Setup()
        {
            _customMemoryCache.DropEvents();
            _customMemoryCache.ClearDb();
        }

        [Test]
        public async Task DeleteAsync_UserInDbSetGetAndDelete_RemovesFromDbAndCache()
        {
            //arrange

            var user = Users[Random.Shared.Next(Users.Count)];

            string? keyToGetCachedUser = null;

            _customMemoryCache.OnSet += (key, value, span) =>
            {
                if (user.Equals(value))
                {
                    keyToGetCachedUser = key;
                }
            };

            //act

            var userFromContext = await _repository.GetAsync(user.Id);

            if (keyToGetCachedUser == null) { throw new Exception(); }

            var userFromCache = await _customMemoryCache.GetAsync<User>(keyToGetCachedUser);

            await _repository.DeleteAsync(userFromContext?.Id ?? throw new Exception());

            var attemptToGetuserFromContext = await _repository.GetAsync(user.Id);

            var attemptToGetuserFromCache = await _customMemoryCache.GetAsync<User>(keyToGetCachedUser);

            //asssert

            Assert.Multiple(() =>
            {
                Assert.That(userFromCache, Is.Not.Null);

                Assert.That(attemptToGetuserFromContext, Is.Null);

                Assert.That(attemptToGetuserFromCache, Is.Null);
            });
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _testDbContext.Dispose();
        }
    }
}