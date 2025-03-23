using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericCachingRepository
{
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class CachingUpdateAsyncTests : CachingRepositoryTest
    {
        public CachingUpdateAsyncTests(Type repType) : base(repType) { }

        [SetUp]
        public void SetUp()
        {
            _customMemoryCache.ClearDb();
            _customMemoryCache.DropEvents();
        }

        [Test]
        public async Task UpdateAsync_UpdatingContainedUser_ValueUpdatedAndAddedToCache()
        {
            var userToUpdate = Users[Random.Shared.Next(Users.Count)];

            User? userAddedToCache = null;

            _customMemoryCache.OnSet += (key, value, offset) =>
            {
                if (value is User uVal && uVal.Id == userToUpdate.Id)
                {
                    userAddedToCache = uVal;
                }
            };

            var userBeforeUpdate = new User
            {
                Id = userToUpdate.Id,
                Name = userToUpdate.Name,
                Email = userToUpdate.Email,
                Address = userToUpdate.Address,
                Login = userToUpdate.Login
            };

            userToUpdate.Name = Guid.NewGuid().ToString();

            await _repository.UpdateAsync(userToUpdate);

            var userAfterUpdate = await _repository.GetAsync(userToUpdate.Id) ?? throw new ArgumentException();

            Assert.Multiple(() =>
            {
                Assert.That(userAfterUpdate, Is.Not.EqualTo(userBeforeUpdate), $"Is equal: {userAfterUpdate.Equals(userBeforeUpdate)}");
                Assert.That(userAfterUpdate, Is.EqualTo(userToUpdate));
                Assert.That(userAfterUpdate, Is.EqualTo(userAddedToCache));
            });
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _testDbContext.Dispose();
        }
    }
}