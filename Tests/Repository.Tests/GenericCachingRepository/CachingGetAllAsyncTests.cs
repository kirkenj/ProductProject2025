using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericCachingRepository
{
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class CachingGetAllAsyncTests : CachingRepositoryTest
    {
        public CachingGetAllAsyncTests(Type type) : base(type) { }


        [SetUp]
        public void Setup()
        {
            _customMemoryCache.ClearDb();
            _customMemoryCache.DropEvents();
        }

        [Test]
        public async Task GetAllAsync_ReturnsValuesAddsValueToCache()
        {
            //arrange
            var cachedUsers = new List<User>();

            ICollection<User>? cachedRange = null;

            _customMemoryCache.OnSet += (key, value, span) =>
            {
                if (value is User uVal)
                {
                    cachedUsers.Add(uVal);
                }
                else if (value is ICollection<User> idVal)
                {
                    cachedRange = idVal;
                }
            };

            //act
            var users = await _repository.GetAllAsync();

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(users, Is.EquivalentTo(_testDbContext.Users.ToArray()));

                Assert.That(users, Is.EquivalentTo(cachedRange));

                Assert.That(users, Is.EquivalentTo(cachedUsers));
            });
        }

        [Test]
        public async Task GetAllAsync_GetTwiseInARowWithDelay_SecondValueGotFromContext()
        {
            //arrange

            ICollection<User>? firstPreCachedResult = null;

            ICollection<User>? secondPreCachedResult = null;

            int cacheGetCounter = 0;

            _customMemoryCache.OnGet += (key, result) =>
            {
                cacheGetCounter++;

                if (result is not ICollection<User> uResult)
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

            var firstGottenResult = await _repository.GetAllAsync();

            await Task.Delay(_repository.ÑacheTimeoutMiliseconds);

            var secondGottenResult = await _repository.GetAllAsync();

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(firstPreCachedResult, Is.Null, "First precached value must be null");
                Assert.That(secondPreCachedResult, Is.Null, "Second precached value must be null");
                Assert.That(firstGottenResult, Is.EquivalentTo(secondGottenResult), $"{nameof(firstGottenResult)} must be same as {nameof(secondGottenResult)}");
            });
        }

        [Test]
        public async Task GetAllAsync_GetTwiseInARow_SecondValueGotFromCache()
        {
            //arrange

            ICollection<User>? firstPreCachedResult = null;

            ICollection<User>? secondPreCachedResult = null;

            int cacheGetCounter = 0;

            _customMemoryCache.OnGet += (key, result) =>
            {
                cacheGetCounter++;

                if (result is not ICollection<User> uResult)
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

            var firstGottenResult = await _repository.GetAllAsync();

            var secondGottenResult = await _repository.GetAllAsync();

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(firstPreCachedResult, Is.Null, "First precached value must be null");
                Assert.That(secondPreCachedResult, Is.Not.Null, "Second precached value must be not null");
                Assert.That(firstGottenResult, Is.EquivalentTo(secondGottenResult), $"{nameof(firstGottenResult)} must be same as {nameof(secondGottenResult)}");
                Assert.That(firstPreCachedResult, Is.Null, "First precached result has to be null");
                Assert.That(secondPreCachedResult, Is.EquivalentTo(secondGottenResult), "Second precached result has to be same as context value");
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _testDbContext.Dispose();
        }
    }
}