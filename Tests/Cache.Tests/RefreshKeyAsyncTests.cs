using Cache.Models;
using Cache.Tests.Models;

#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.

namespace Cache.Tests
{

    [TestFixture(typeof(RedisCustomMemoryCache))]
    [TestFixture(typeof(CustomMemoryCache))]
    public class RefreshKeyAsyncTests : TestBase
    {

        public RefreshKeyAsyncTests(Type type) : base(type) { }

        [Test]
        public async Task RefreshKeyAsync_KeyNotSet_ReturnsFalse()
        {
            var key = "hello world";

            var result = await _cache.RefreshKeyAsync(key, 300);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RefreshKeyAsync_KeyIsEmptyString_ReturnsFalse()
        {
            var key = string.Empty;

            var result = await _cache.RefreshKeyAsync(key, 300);

            Assert.That(result, Is.False);
        }

        [Test]
        public void RefreshKeyAsync_KeyIsNull_ReturnsFalse()
        {

            var func = async () => await _cache.RefreshKeyAsync(null, 300);

            Assert.That(func, Throws.ArgumentException);
        }

        [Test]
        public async Task RefreshKeyAsync_KeyValueSetKeyRefreshed_ReturnsValue()
        {
            var value = Guid.NewGuid();

            var key = $"Key for {value}";

            await _cache.SetAsync(key, value, TimeSpan.FromMilliseconds(300));

            Thread.Sleep(200);

            var refreshResult = await _cache.RefreshKeyAsync(key, 300);

            Thread.Sleep(200);

            var valueFromCache = await _cache.GetAsync<Guid>(key);

            Assert.Multiple(() =>
            {
                Assert.That(refreshResult, Is.True);

                Assert.That(valueFromCache, Is.EqualTo(value));
            });
        }

        [Test]
        public async Task RefreshKeyAsync_KeyValueSetKeyRefreshedWhenTimeIsUp_RefreshReturnsFalseGetValueReturnsNull()
        {
            var value = Guid.NewGuid();

            var key = $"Key for {value}";

            await _cache.SetAsync(key, value, TimeSpan.FromMilliseconds(300));

            Thread.Sleep(300);

            var refreshResult = await _cache.RefreshKeyAsync(key, 300);

            var valueFromCache = await _cache.GetAsync<Guid>(key);

            Assert.Multiple(() =>
            {
                Assert.That(refreshResult, Is.False);

                Assert.That(valueFromCache, Is.EqualTo(Guid.Empty));
            });
        }
    }
}

#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.