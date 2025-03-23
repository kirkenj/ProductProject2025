using Cache.Models;
using Cache.Tests.Models;

#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.

namespace Cache.Tests
{

    [TestFixture(typeof(RedisCustomMemoryCache))]
    [TestFixture(typeof(CustomMemoryCache))]
    public class RemoveAsyncTests : TestBase
    {
        public RemoveAsyncTests(Type type) : base(type) { }

        [Test]
        public void RemoveAsync_KeyNull_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.RemoveAsync(null);

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void RemoveAsync_KeyEmpty_ThrowsArgumentNullException()
        {
            var key = string.Empty;

            //act

            //assert
            Assert.Throws(typeof(ArgumentNullException), () => _cache.RemoveAsync(key));
        }

        [Test]
        public void RemoveAsync_KeyNotNullCacheNotSet_Pass()
        {
            //arrange
            var key = Guid.NewGuid().ToString();

            //act

            //assert
            Assert.DoesNotThrowAsync(async () => await _cache.RemoveAsync(key));
        }

        [Test]
        public async Task RemoveAsync_KeyNotNullCacheSet_GetAsyncReturnsNull()
        {
            //arrange
            var key = Guid.NewGuid().ToString();
            var value = new Human() { Name = "Hello", Surname = "World" };

            //act
            await _cache.SetAsync(key, value, TimeSpan.FromSeconds(5));

            await _cache.RemoveAsync(key);

            var cacheResult = await _cache.GetAsync<Human>(key);

            //assert
            Assert.That(cacheResult, Is.Null);
        }

        [Test]
        public async Task RemoveAsync_MultipleCacheSet_RemovesOnlySelectedEntry()
        {
            //arrange

            List<KeyValuePair<string, Human>> humansBeforeCache = new();

            for (int i = 0; i < 10; i++)
            {
                humansBeforeCache.Add
                (new KeyValuePair<string, Human>
                    (
                        Guid.NewGuid().ToString(),
                        new Human { Name = Guid.NewGuid().ToString(), Surname = Guid.NewGuid().ToString() }
                    )
                );
            }

            KeyValuePair<string, Human> selectedKeyValuePair = new(Guid.NewGuid().ToString(), new Human() { Name = "Hello", Surname = "World" });

            //act

            foreach (var l in humansBeforeCache)
            {
                await _cache.SetAsync(l.Key, l.Value, TimeSpan.FromSeconds(5));
            }

            await _cache.SetAsync(selectedKeyValuePair.Key, selectedKeyValuePair.Value, TimeSpan.FromSeconds(5));

            await _cache.RemoveAsync(selectedKeyValuePair.Key);

            var selectedCacheResult = await _cache.GetAsync<Human>(selectedKeyValuePair.Key);

            List<KeyValuePair<string, Human?>> humansAfterCache = new();

            foreach (var h in humansBeforeCache)
            {
                humansAfterCache.Add(
                    new KeyValuePair<string, Human?>
                    (
                        h.Key,
                        await _cache.GetAsync<Human>(h.Key)
                    )
                );
            }

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(humansAfterCache, Is.EqualTo(humansBeforeCache));
                Assert.That(selectedCacheResult, Is.Null);
            });
        }
    }
}

#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.