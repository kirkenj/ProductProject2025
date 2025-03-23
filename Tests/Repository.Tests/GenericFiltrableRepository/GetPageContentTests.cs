using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;
using System.Text.Json;

namespace Repository.Tests.GenericFiltrableRepository
{
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    [TestFixture(typeof(GenericFiltrableRepository<,,>))]
    public class GetPageContentTests : FiltrableRepositoryTest
    {
        public GetPageContentTests(Type type) : base(type) { }


        [Test]
        public async Task GetPageContentTests_FilterNullNullPageNullSize_ReturnsAllValues()
        {
            var users = await _repository.GetPageContent(null, null, null);

            Assert.That(users, Is.EqualTo(Users));
        }

        [Test]
        public async Task GetPageContentTests_FilterEmptyNullPageNullSize_ReturnsAllValues()
        {
            var filter = new UserFilter();

            var users = await _repository.GetPageContent(filter, null, null);

            Assert.That(users, Is.EqualTo(Users));
        }

        [Test]
        public async Task GetPageContentTests_FilterSetSomeIdNullPageNullSize_ReturnsValues()
        {
            var filter = new UserFilter();

            List<Guid> ids = new();

            for (int i = 0; i < 3; i++)
            {
                ids.Add(Users[Random.Shared.Next(Users.Count)].Id);
            }

            filter.Ids = ids;

            var users = await _repository.GetPageContent(filter, null, null);

            Assert.That(users, Is.SubsetOf(Users));
        }

        [TestCase("{\"Ids\":null,\"LoginPart\":null,\"EmailPart\":null,\"NamePart\":\"i\",\"AddressPart\":null}")]
        [TestCase("{\"Ids\":null,\"LoginPart\":null,\"EmailPart\":\"e\",\"NamePart\":null,\"AddressPart\":\"sk\"}")]
        public async Task GetPageContentTests_FilterSetNullPageNullSize_ReturnsValues(string filterJson)
        {
            var filter = JsonSerializer.Deserialize<UserFilter>(filterJson) ?? throw new Exception();
            var usersResult = await _repository.GetPageContent(filter, null, null);
            var expectedUsers = UserFilter.GetFilteredSet(Users.AsQueryable(), filter);

            Assert.That(usersResult, Is.SubsetOf(Users));
            Assert.That(usersResult, Is.EquivalentTo(expectedUsers));
        }


        [OneTimeTearDown]
        public void TearDown()
        {
            _testDbContext.Dispose();
        }
    }
}