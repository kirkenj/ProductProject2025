using Repository.Models;
using Repository.Tests.Models.TestBases;

namespace Repository.Tests.GenericFiltrableCachingRepository
{
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


        [OneTimeTearDown]
        public void TearDown()
        {
            _testDbContext.Dispose();
        }
    }
}