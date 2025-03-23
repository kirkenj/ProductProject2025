using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericCachingRepository
{
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class CachingGetPageContentTests : CachingRepositoryTest
    {
        public CachingGetPageContentTests(Type repType) : base(repType) { }

        [SetUp]
        public void SetUp()
        {
            _customMemoryCache.ClearDb();
            _customMemoryCache.DropEvents();
        }

        [Test]
        public async Task GetPageContentTests_NullPageNullSize_ReturnsAllValuesCachesAllValues()
        {
            List<User> cachedUsers = new();

            ICollection<User>? cachedUsersAsRange = null;

            _customMemoryCache.OnSet += (key, value, offset) =>
            {
                if (value is User uVal)
                {
                    cachedUsers.Add(uVal);
                }
                else if (value is ICollection<User> uCollection)
                {
                    cachedUsersAsRange = uCollection;
                }
            };

            var resultUsers = await _repository.GetPageContent(null, null);

            Assert.That(resultUsers, Is.EquivalentTo(Users));
            Assert.That(resultUsers, Is.EquivalentTo(cachedUsersAsRange));
            Assert.That(resultUsers, Is.EquivalentTo(cachedUsers));
        }

        [Test]
        public async Task GetPageContentTests_NullPageNotNullSize_ReturnsAllValues()
        {
            List<User> cachedUsers = new();

            ICollection<User>? cachedUsersAsRange = null;

            _customMemoryCache.OnSet += (key, value, offset) =>
            {
                if (value is User uVal)
                {
                    cachedUsers.Add(uVal);
                }
                else if (value is ICollection<User> uCollection)
                {
                    cachedUsersAsRange = uCollection;
                }
            };

            var resultUsers = await _repository.GetPageContent(null, 1);

            Assert.That(resultUsers, Is.EquivalentTo(Users));
            Assert.That(resultUsers, Is.EquivalentTo(cachedUsersAsRange));
            Assert.That(resultUsers, Is.EquivalentTo(cachedUsers));
        }

        [Test]
        public async Task GetPageContentTests_NotNullPageNullSize_ReturnsAllValues()
        {
            List<User> cachedUsers = new();

            ICollection<User>? cachedUsersAsRange = null;

            _customMemoryCache.OnSet += (key, value, offset) =>
            {
                if (value is User uVal)
                {
                    cachedUsers.Add(uVal);
                }
                else if (value is ICollection<User> uCollection)
                {
                    cachedUsersAsRange = uCollection;
                }
            };

            var resultUsers = await _repository.GetPageContent(1, null);

            Assert.That(resultUsers, Is.EquivalentTo(Users));
            Assert.That(resultUsers, Is.EquivalentTo(cachedUsersAsRange));
            Assert.That(resultUsers, Is.EquivalentTo(cachedUsers));
        }

        [TestCase(1, 1, 0, 1)]
        [TestCase(2, 1, 1, 1)]
        [TestCase(2, 2, 2, 2)]
        [TestCase(1, 3, 0, 3)]
        [TestCase(2, 3, 3, 3)]
        [TestCase(3, 2, 4, 2)]
        public async Task GetPageContentTests_NotNullPageNullSize_ReturnsValues(int page, int pageSize, int expectedResultIndex, int expectedResultCount)
        {
            List<User> cachedUsers = new();

            ICollection<User>? cachedUsersAsRange = null;

            _customMemoryCache.OnSet += (key, value, offset) =>
            {
                if (value is User uVal)
                {
                    cachedUsers.Add(uVal);
                }
                else if (value is ICollection<User> uCollection)
                {
                    cachedUsersAsRange = uCollection;
                }
            };

            var resultUsers = await _repository.GetPageContent(page, pageSize);

            var expectedResult = Users.GetRange(expectedResultIndex, expectedResultCount);

            Assert.That(resultUsers, Is.EquivalentTo(expectedResult));
            Assert.That(resultUsers, Is.EquivalentTo(cachedUsersAsRange));
            Assert.That(resultUsers, Is.EquivalentTo(cachedUsers));
        }


        [Test]
        public async Task GetPageContent_GetTwiseInARowWithDelay_SecondValueGotFromContext()
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

            var firstGottenResult = await _repository.GetPageContent();

            await Task.Delay(_repository.ÑacheTimeoutMiliseconds);

            var secondGottenResult = await _repository.GetPageContent();

            //assert

            Assert.Multiple(() =>
            {
                Assert.That(firstPreCachedResult, Is.Null, "First precached value must be null");
                Assert.That(secondPreCachedResult, Is.Null, "Second precached value must be null");
                Assert.That(firstGottenResult, Is.EquivalentTo(secondGottenResult), $"{nameof(firstGottenResult)} must be same as {nameof(secondGottenResult)}");
            });
        }

        [Test]
        public async Task GetPageContent_GetTwiseInARow_SecondValueGotFromCache()
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

            var firstGottenResult = await _repository.GetPageContent();

            var secondGottenResult = await _repository.GetPageContent();

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