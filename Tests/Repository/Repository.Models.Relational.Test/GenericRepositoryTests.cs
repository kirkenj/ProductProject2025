using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Repository.Models.Relational.Test.Models;
using Repository.Models.Relational.Test.TestBases;

namespace Repository.Models.Relational.Test
{
    public class GenericRepositoryTests : TestWithDbContext
    {
        protected virtual GenericRepository<User, Guid> Repository { get; set; }

        public GenericRepositoryTests()
        {
            Repository = new GenericRepository<User, Guid>(_dbContext);
        }

        [Fact]
        public void AddAsync_UserIsNull_ThrowsArgumentNullException()
        {
            // Arrange, Act and Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await Repository.AddAsync(null));
        }

        [Fact]
        public async Task AddAsync_ValueIsValid_AddsValue()
        {
            // Arragnge
            var user = new User
            {
                Address = "Zone 51",
                Name = "Natan",
                Email = "Natan228@uya.ru",
                Login = "BBCReportner",
                Id = Guid.NewGuid()
            };


            // Act
            await Repository.AddAsync(user);

            // Assert
            _dbContext.ChangeTracker.Clear();

            var userFromContext = await _dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            Assert.Equivalent(userFromContext, user);
        }


        [Fact]
        public async Task DeleteAsync_IdNotContained_ShoulNotThrowException()
        {
            // Arrange, Act and Assert
            await Repository.DeleteAsync(Guid.NewGuid());
        }

        [Fact]
        public async Task DeleteAsync_UserInDbSet_RemovesValue()
        {
            // Arrange
            var user = Users.First();

            // Act
            await Repository.DeleteAsync(user.Id);

            // Assert
            var entry = _dbContext.Entry(user);

            Assert.Equivalent(entry.State, EntityState.Detached);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsValues()
        {
            // Arrange and Act
            var users = await Repository.GetAllAsync();

            // Assert
            Assert.Equivalent(users, Users);
        }

        [Fact]
        public async Task GetAsync_IDNotContained_ReturnsNull()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var user = await Repository.GetAsync(id);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task GetAsync_IDContained_ReturnsTheUser()
        {
            // Arrange
            var user = Users.First();

            // Act
            var users = await Repository.GetAsync(user.Id);

            // Assert
            Assert.Equivalent(users, user);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, 1)]
        [InlineData(1, null)]
        public async Task GetPageContentTests_NullPageOrNullSize_ReturnsAllValues(int? page, int? pageSize)
        {
            // Arrange and Act
            var users = await Repository.GetPageContent(page, pageSize);

            // Assert
            Assert.Equivalent(users, Users);
        }

        [Theory]
        [InlineData(1, 1, 0, 1)]
        [InlineData(2, 1, 1, 1)]
        [InlineData(2, 2, 2, 2)]
        [InlineData(1, 3, 0, 3)]
        [InlineData(2, 3, 3, 3)]
        [InlineData(3, 2, 4, 2)]
        public async Task GetPageContentTests_ValidPageAndPagSize_ReturnsValues(int page, int pageSize, int expectedResultIndex, int expectedResultCount)
        {
            // Arrange
            var expectedResult = Users.ToList().GetRange(expectedResultIndex, expectedResultCount);

            // Act
            var users = await Repository.GetPageContent(page, pageSize);

            // Assert
            Assert.Equivalent(users, expectedResult);
        }


        [Fact]
        public async Task UpdateAsync_UpdatingContainedUser_ValueUpdated()
        {
            var userToUpdate = Users.First();

            var userBeforeUpdate = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(userToUpdate));

            var newValue = $"Updated value {Guid.NewGuid()}";

            userToUpdate.Name = newValue;

            await Repository.UpdateAsync(userToUpdate);

            var userAfterUpdate = await Repository.GetAsync(userToUpdate.Id) ?? throw new ArgumentException();

            Assert.Equivalent(userToUpdate, userAfterUpdate);
        }

        [Fact]
        public void UpdateAsync_UpdatingNotContainedUser_ThrowsException()
        {
            var userToUpdate = new User
            {
                Id = Guid.NewGuid(),
                Name = "userToUpdate.Name",
                Email = "userToUpdate.Email",
                Address = "userToUpdate.Address",
                Login = "userToUpdate.Login"
            };

            var func = async () => await Repository.UpdateAsync(userToUpdate);

            Assert.ThrowsAsync<InvalidOperationException>(func);
        }

        [Fact]
        public void UpdateAsync_UpdatingNull_ThrowsException()
        {
            var func = async () => await Repository.UpdateAsync(null);

            Assert.ThrowsAsync<InvalidOperationException>(func);
        }
    }
}