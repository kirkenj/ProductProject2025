using Repository.Models;
using Repository.Tests.Common;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;
using System.Text.Json;

namespace Repository.Tests.GenericFiltrableRepository
{
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    [TestFixture(typeof(GenericFiltrableRepository<,,>))]
    public class GetAsyncTests : FiltrableRepositoryTest
    {
        public GetAsyncTests(Type type) : base(type) { }

        [SetUp]
        public async Task SetUp()
        {
            _testDbContext = await TestConstants.GetDbContextAsync();
        }

        [Test]
        public async Task GetAsyncFilter_FilterIsNull_ReturnsValueOrNull()
        {
            var result = await _repository.GetAsync(null);

            Assert.That(Users, Does.Contain(result).Or.Null);
        }

        [Test]
        public async Task GetAsyncFilter_FilterEmpty_ReturnsValues()
        {
            var filter = new UserFilter();

            var userResult = await _repository.GetAsync(filter);

            var possibleUsers = UserFilter.GetFilteredSet(Users.AsQueryable(), filter);

            Assert.That(possibleUsers, Does.Contain(userResult));
        }

        [Test]
        public async Task GetAsyncFilter_FilterSetSomeId_ReturnsValue()
        {
            var filter = new UserFilter();

            List<Guid> ids = new();

            for (int i = 0; i < 3; i++)
            {
                ids.Add(Users[Random.Shared.Next(Users.Count)].Id);
            }

            filter.Ids = ids;

            var userResult = await _repository.GetAsync(filter);

            var possibleUsers = UserFilter.GetFilteredSet(Users.AsQueryable(), filter);

            Assert.That(possibleUsers, Does.Contain(userResult));
        }

        [TestCase("{\"Ids\":null,\"LoginPart\":null,\"EmailPart\":null,\"NamePart\":\"i\",\"AddressPart\":null}")]
        [TestCase("{\"Ids\":null,\"LoginPart\":null,\"EmailPart\":\"e\",\"NamePart\":null,\"AddressPart\":\"sk\"}")]
        public async Task GetPageContentTests_FilterSetNullPageNullSize_ReturnsValues(string filterJson)
        {
            var filter = JsonSerializer.Deserialize<UserFilter>(filterJson) ?? throw new Exception();

            var userResult = await _repository.GetAsync(filter);

            var possibleUsers = UserFilter.GetFilteredSet(Users.AsQueryable(), filter).ToArray();

            Assert.That(possibleUsers, Does.Contain(userResult));
        }

        [TearDown]
        public void TearDown()
        {
            _testDbContext.Dispose();
        }
    }
}