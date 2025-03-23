using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericRepository
{
    [TestFixture(typeof(GenericRepository<,>))]
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableRepository<,,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class UpdateAsyncTests : GenericRepositoryTest
    {
        public UpdateAsyncTests(Type repType) : base(repType) { }


        [Test]
        public async Task UpdateAsync_UpdatingContainedUser_ValueUpdated()
        {
            var userToUpdate = Users[Random.Shared.Next(Users.Count)];

            var userBeforeUpdate = new User
            {
                Id = userToUpdate.Id,
                Name = userToUpdate.Name,
                Email = userToUpdate.Email,
                Address = userToUpdate.Address,
                Login = userToUpdate.Login
            };

            var newValue = $"Updated value {Guid.NewGuid()}";

            userToUpdate.Name = newValue;

            await _repository.UpdateAsync(userToUpdate);

            var userAfterUpdate = await _repository.GetAsync(userToUpdate.Id) ?? throw new ArgumentException();

            Assert.Multiple(() =>
            {
                Assert.That(userToUpdate, Is.EqualTo(userAfterUpdate));
                Assert.That(userAfterUpdate, Is.Not.EqualTo(userBeforeUpdate));
            });
        }

        [Test]
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


            var func = async () => await _repository.UpdateAsync(userToUpdate);

            Assert.That(func, Throws.Exception);
        }


        [Test]
        public void UpdateAsync_UpdatingNull_ThrowsException()
        {
            var func = async () => await _repository.UpdateAsync(null);

            Assert.That(func, Throws.Exception);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _testDbContext.Dispose();
        }
    }
}