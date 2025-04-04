using System.Text.Json;
using Repository.Models.Relational.Test.Models;
using static Repository.Models.Relational.Test.Models.TestConstants;

namespace Repository.Models.Relational.Test
{
    public class GenericFiltrableRepositoryTests : GenericRepositoryTests
    {
        private readonly GenericFiltrableRepository<User, Guid, UserFilter> _repository;

        protected override GenericRepository<User, Guid> Repository => _repository;

        public GenericFiltrableRepositoryTests()
        {
            _repository = new GenericFiltrableRepository<User, Guid, UserFilter>(_dbContext, UserFilter.GetFilteredSet);
        }

        [Theory]
        [ClassData(typeof(EmptyUserFilterData))]
        [InlineData(null)]
        public async Task GetAsyncFilter_FilterIsNullOrEmpty_ReturnsValueFromContext(UserFilter userFilter)
        {
            // Arrange and Act
            var result = await _repository.GetAsync(userFilter);

            // Assert
            Assert.True(_dbContext.Set<User>().Contains(result));
        }

        [Fact]
        public async Task GetAsyncFilter_FilterSetSomeId_ReturnsValue()
        {
            // Arrange
            var filter = new UserFilter()
            {
                Ids = [.. Users.Take(3).Select(u => u.Id)]
            };

            var possibleUsers = UserFilter.GetFilteredSet(Users.AsQueryable(), filter);

            // Act
            var userResult = await _repository.GetAsync(filter);

            // Assert
            Assert.Equivalent(possibleUsers, userResult);
        }

        [Theory]
        [InlineData("{\"Ids\":null,\"LoginPart\":null,\"EmailPart\":null,\"NamePart\":\"i\",\"AddressPart\":null}")]
        [InlineData("{\"Ids\":null,\"LoginPart\":null,\"EmailPart\":\"e\",\"NamePart\":null,\"AddressPart\":\"sk\"}")]
        public async Task GetPageContentTests_FilterSetNullPageNullSizeWithoutPages_ReturnsValues(string filterJson)
        {
            // Arrange
            var filter = JsonSerializer.Deserialize<UserFilter>(filterJson) ?? throw new Exception();

            var possibleUsers = UserFilter.GetFilteredSet(Users.AsQueryable(), filter).ToArray().First();

            // Act
            var userResult = await _repository.GetAsync(filter);

            // Assert
            Assert.Equivalent(possibleUsers, userResult);
        }

        [Theory]
        [ClassData(typeof(EmptyUserFilterData))]
        [InlineData(null)]
        public async Task GetPageContentTests_FilterEmptyNullPageNullSize_ReturnsAllValues(UserFilter userFilter)
        {
            // Arrange and Act
            var users = await _repository.GetPageContentAsync(userFilter, null, null);

            // Assert
            Assert.Equivalent(users, Users);
        }

        [Fact]
        public async Task GetPageContentTests_FilterSetSomeIdNullPageNullSize_ReturnsValues()
        {
            // Arrange
            var filter = new UserFilter()
            {
                Ids = [.. Users.Take(3).Select(u => u.Id)]
            };

            // Act
            var users = await _repository.GetPageContentAsync(filter, null, null);

            // Assert
            Assert.Equivalent(users, Users);
        }

        [Theory]
        [InlineData("{\"Ids\":null,\"LoginPart\":null,\"EmailPart\":null,\"NamePart\":\"i\",\"AddressPart\":null}")]
        [InlineData("{\"Ids\":null,\"LoginPart\":null,\"EmailPart\":\"e\",\"NamePart\":null,\"AddressPart\":\"sk\"}")]
        public async Task GetPageContentTests_FilterSetNullPageNullSize_ReturnsValues(string filterJson)
        {
            var filter = JsonSerializer.Deserialize<UserFilter>(filterJson) ?? throw new Exception();
            var usersResult = await _repository.GetPageContentAsync(filter, null, null);
            var expectedUsers = UserFilter.GetFilteredSet(Users.AsQueryable(), filter);

            Assert.Equivalent(usersResult, expectedUsers);
        }
    }
}
