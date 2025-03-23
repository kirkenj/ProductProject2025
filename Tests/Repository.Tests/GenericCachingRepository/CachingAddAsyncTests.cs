using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericCachingRepository
{
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class CachingAddAsyncTests : CachingRepositoryTest
    {
        public CachingAddAsyncTests(Type type) : base(type) { }

        [Test]
        public async Task AddAsyncTests_ValueIsValid_AddsValueToContextAndCache()
        {
            var user223 = new User
            {
                Address = "Zone 51",
                Name = "Natan",
                Email = "Natan228@ya.rru",
                Login = "BBCReporter3",
            };

            User? userFromCache = null;

            _customMemoryCache.OnSet += (key, value, span) =>
            {
                if (value is User uVal && uVal.Equals(user223))
                {
                    userFromCache = uVal;
                }
            };

            await _repository.AddAsync(user223);

            var userFromContext = await _repository.GetAsync(user223.Id);

            Assert.That(userFromContext, Is.EqualTo(user223));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _testDbContext.Dispose();
        }
    }
}