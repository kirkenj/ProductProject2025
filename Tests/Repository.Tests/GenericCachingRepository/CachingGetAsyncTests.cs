using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericCachingRepository
{
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class CachingGetAsyncTests : CachingRepositoryTest
    {
        public CachingGetAsyncTests(Type type) : base(type) { }


        [SetUp]
        public void SetUp()
        {
            _customMemoryCache.DropEvents();
            _customMemoryCache.ClearDb();
        }


        [Test]
        public async Task GetAsync_ValueExcists_NotFoundInCacheReturnsValueAddsValueToCache()
        {
            //arrange

            Guid userId = Users[Random.Shared.Next(0, Users.Count)].Id;

            User? preCachedResult = null;

            User? valueAddedToCache = null;

            _customMemoryCache.OnGet += (key, result) =>
            {
                if (result is User uResult)
                {
                    preCachedResult = uResult;
                }
            };


            _customMemoryCache.OnSet += (key, value, span) =>
            {
                if (value is User uResult)
                {
                    valueAddedToCache = uResult;
                }
            };

            //act

            var user = await _repository.GetAsync(userId);

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(preCachedResult, Is.Null, "Precached Result Check");
                Assert.That(valueAddedToCache, Is.EqualTo(user), "After act cached result");
            });
        }


        [Test]
        public async Task GetAsync_GetTwiseInARowWithDelay_SecondValueGotFromContext()
        {
            //arrange

            Guid userId = Users[Random.Shared.Next(0, Users.Count)].Id;

            User? firstPreCachedResult = null;

            User? secondPreCachedResult = null;

            int cacheGetCounter = 0;

            _customMemoryCache.OnGet += (key, result) =>
            {
                cacheGetCounter++;

                if (result is not User uResult)
                {
                    return;
                }

                if (cacheGetCounter == 1)
                {
                    firstPreCachedResult = uResult;
                }
                else if (cacheGetCounter == 2)
                {
                    secondPreCachedResult = uResult;
                }
            };

            //act

            var firstGottenResult = await _repository.GetAsync(userId);

            await Task.Delay(_repository.ÑacheTimeoutMiliseconds);

            var secondGottenResult = await _repository.GetAsync(userId);

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(firstPreCachedResult, Is.Null, "First precached value must be null");
                Assert.That(secondPreCachedResult, Is.Null, "Second precached value must be null");
                Assert.That(firstGottenResult, Is.EqualTo(secondGottenResult), $"{nameof(firstGottenResult)} must be same as {nameof(secondGottenResult)}");
            });
        }

        [Test]
        public async Task GetAsync_GetTwiseInARow_SecondValueGotFromCache()
        {
            //arrange

            Guid userId = Users[Random.Shared.Next(0, Users.Count)].Id;

            User? firstPreCachedResult = null;

            User? secondPreCachedResult = null;

            int cacheGetCounter = 0;

            _customMemoryCache.OnGet += (key, result) =>
            {
                cacheGetCounter++;

                if (result is not User uResult)
                {
                    return;
                }

                if (cacheGetCounter == 1)
                {
                    firstPreCachedResult = uResult;
                }
                else if (cacheGetCounter == 2)
                {
                    secondPreCachedResult = uResult;
                }
            };

            //act

            var firstGottenResult = await _repository.GetAsync(userId);

            var secondGottenResult = await _repository.GetAsync(userId);

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(firstPreCachedResult, Is.Null, "First precached value must be null");
                Assert.That(secondPreCachedResult, Is.Not.Null, "Second precached value must be not null");
                Assert.That(firstGottenResult, Is.EqualTo(secondGottenResult), $"{nameof(firstGottenResult)} must be same as {nameof(secondGottenResult)}");
                Assert.That(firstPreCachedResult, Is.Null, "First precached result has to be null");
                Assert.That(secondPreCachedResult, Is.EqualTo(secondGottenResult), "Second precached result has to be same as context value");
            });
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _testDbContext.Dispose();
        }
    }
}