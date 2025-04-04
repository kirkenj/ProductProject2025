using Microsoft.EntityFrameworkCore;
using Repository.Models.Relational.Test.Models;

namespace Repository.Models.Relational.Test.TestBases
{
    public abstract class TestWithDbContext : IDisposable
    {
        protected readonly TestDbContext _dbContext = null!;
        protected IEnumerable<User> Users { get; set; } = null!;

        protected TestWithDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString(), null);
            _dbContext = new(optionsBuilder.Options);

            Users = TestConstants.DefaultUsers;

            _dbContext.Users.AddRange(Users);

            _dbContext.SaveChanges();

            _dbContext.ChangeTracker.Clear();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
