using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericFiltrableCachingRepository
{
    public class CachingGetAsyncTests : CachingFiltrableRepositoryTest
    {
        public CachingGetAsyncTests() : base() { }


        [SetUp]
        public void SetUp()
        {
            _customMemoryCache.DropEvents();
            _customMemoryCache.ClearDb();
        }


        [Test]
        public async Task GetAsyncFilter_GetTwiseInARowWithDelay_SecondValueGotFromContext()
        {
            //arrange

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

            var firstGottenResult = await _repository.GetAsync(null);

            await Task.Delay(_repository.ÑacheTimeoutMiliseconds);

            var secondGottenResult = await _repository.GetAsync(null);

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(firstPreCachedResult, Is.Null, "First precached value must be null");
                Assert.That(secondPreCachedResult, Is.Null, "Second precached value must be null");
                Assert.That(firstGottenResult, Is.EqualTo(secondGottenResult), $"{nameof(firstGottenResult)} must be same as {nameof(secondGottenResult)}");
            });
        }

        [Test]
        public async Task GetAsyncFilter_GetTwiseInARow_SecondValueGotFromCache()
        {
            //arrange

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

            var firstGottenResult = await _repository.GetAsync(null);

            var secondGottenResult = await _repository.GetAsync(null);

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