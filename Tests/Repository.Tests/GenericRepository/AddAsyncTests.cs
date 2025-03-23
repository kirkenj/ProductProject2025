using Repository.Models;
using Repository.Tests.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericRepository
{
    [TestFixture(typeof(GenericRepository<,>))]
    [TestFixture(typeof(GenericCachingRepository<,>))]
    [TestFixture(typeof(GenericFiltrableRepository<,,>))]
    [TestFixture(typeof(GenericFiltrableCachingRepository<,,>))]
    public class AddAsyncTests : GenericRepositoryTest
    {
        public AddAsyncTests(Type repType) : base(repType) { }

        [Test]
        public void AddAsync_UserIsNull_ThrowsArgumentNullException()
        {
            var func = async () => await _repository.AddAsync(null);

            Assert.That(func, Throws.ArgumentNullException);
        }

        [Test]
        public void AddAsyncTests_UserAlreadyInDbSet_ThrowsArgumentException()
        {
            var func = async () => await _repository.AddAsync(Users.First());

            Assert.That(func, Throws.ArgumentException);
        }

        [Test]
        public void AddAsyncTests_IDIsTaken_ThrowsInvalidOperationException()
        {
            var user = new User
            {
                Address = "Zone 51",
                Name = "Natan",
                Email = "Natan228@ya.ru",
                Login = "BBCReporter",
                Id = Users.First().Id
            };

            var func = async () => await _repository.AddAsync(user);

            Assert.That(func, Throws.InvalidOperationException);
        }

        [Test]
        public async Task AddAsyncTests_ValueIsValid_AddsValue()
        {
            var user = new User
            {
                Address = "Zone 51",
                Name = "Natan",
                Email = "Natan228@uya.ru",
                Login = "BBCReportner",
                Id = Guid.NewGuid()
            };

            await _repository.AddAsync(user);

            var userFromContext = await _repository.GetAsync(user.Id);

            Assert.That(userFromContext, Is.EqualTo(user));
        }
    }
}