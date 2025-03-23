using Cache.Models;
using StackExchange.Redis;

namespace Cache.Tests
{
    public class ConstructorTests
    {

        [Test]
        public void Constructor_NullUriNullContainerName_ThrowsArgumentNullException()
        {
            //arrange
            string? redisUrl = null;

            //act
            var instanseInitDelegate = () => new RedisCustomMemoryCache(redisUrl);

            //assert
            Assert.That(instanseInitDelegate, Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_IncorrectUri_ThrowsRedisConnectionException()
        {
            //arrange
            string? redisUrl = "null";

            //act
            var instanseInitDelegate = () => new RedisCustomMemoryCache(redisUrl);

            //assert
            Assert.That(instanseInitDelegate, Throws.TypeOf<RedisConnectionException>());
        }

        [Test]
        public void Constructor_CorrectUri_ReturnsValue()
        {
            //arrange
            string? redisUrl = "localhost:6379";

            //act
            var instanseInitDelegate = new RedisCustomMemoryCache(redisUrl);

            //assert
            Assert.Pass();
        }
    }
}