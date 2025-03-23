using Cache.Models;
using Cache.Tests.Models;

#pragma warning disable CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.

namespace Cache.Tests
{
    [TestFixture(typeof(RedisCustomMemoryCache))]
    [TestFixture(typeof(CustomMemoryCache))]
    public class SetAsyncTests : TestBase
    {
        private const int _inaccuracyMs = 20;

        public SetAsyncTests(Type type) : base(type) { }


        [Test]
        public void SetAsync_KeyNullValueNullOffset99ms_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>(null, null, TimeSpan.FromMilliseconds(99));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }


        [Test]
        public void SetAsync_KeyNullValueNullOffset100msPlusInaccuracy_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>(null, null, TimeSpan.FromMilliseconds(100 + _inaccuracyMs));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAsync_KeyNullValueNotNullOffset99ms_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>(null, "value", TimeSpan.FromMilliseconds(99));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAsync_KeyNullValueNotNullOffset100msPlusInaccuracy_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>(null, "value", TimeSpan.FromMilliseconds(100 + _inaccuracyMs));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAsync_KeyNotNullValueNullOffset99ms_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>("key", null, TimeSpan.FromMilliseconds(99));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAsync_KeyNotNullValueNullOffset100msPlusInaccuracy_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>("key", null, TimeSpan.FromMilliseconds(100 + _inaccuracyMs));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAsync_KeyNotNullValueNotNullOffset99ms_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>("key", "value", TimeSpan.FromMilliseconds(99));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void SetAsync_KeyNotNullValueNotNullOffset100msPlusInaccuracy_Pass()
        {
            //act
            var func = async () => await _cache.SetAsync<object>("Key", "value", TimeSpan.FromMilliseconds(100 + _inaccuracyMs));

            //assert
            Assert.That(func, Throws.Nothing);
        }

        [Test]
        public void SetAsync_KeyEmptyValueNotNullOffset100msPlusInaccuracy_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>(string.Empty, "value", TimeSpan.FromMilliseconds(100 + _inaccuracyMs));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void SetAsync_KeyWhiteSpaceValueNotNullOffset100msPlusInaccuracy_ThrowsArgumentNullException()
        {
            //act
            var func = async () => await _cache.SetAsync<object>("   ", "value", TimeSpan.FromMilliseconds(100 + _inaccuracyMs));

            //assert
            Assert.That(func, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public async Task SetAsync_KeyValidValueNotNullOffset102ms_PassOrThrowsArgumentOutOfRangeException()
        {
            //arrange
            bool argumentOutOfRangeExceptionThrown = false;
            bool noExceptionsThrown = false;

            string messageStart = "Offset has to be at least 100 ms ahead from now";

            //act
            try
            {
                await _cache.SetAsync<object>("key", "value", TimeSpan.FromMilliseconds(100 + 2));
                noExceptionsThrown = true;
            }
            catch (AggregateException agrEx)
            {
                var exc = agrEx.InnerExceptions.First();

                argumentOutOfRangeExceptionThrown = exc is ArgumentOutOfRangeException outOfRangeException && outOfRangeException.Message.StartsWith(messageStart);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                argumentOutOfRangeExceptionThrown = ex.Message.StartsWith(messageStart);
            }

            //assert

            Assert.That(noExceptionsThrown || argumentOutOfRangeExceptionThrown);
        }

        [Test]
        public async Task SetAsync_MultipleSetsOnOneKey_ReturnsLastSetValue()
        {
            //arrange

            string key = Guid.NewGuid().ToString();
            List<Human> humansBeforeCache = new();

            for (int i = 0; i < 3; i++)
            {
                humansBeforeCache.Add
                (new Human()
                {
                    Name = Guid.NewGuid().ToString(),
                    Surname = Guid.NewGuid().ToString()
                }
                );
            }

            //act

            foreach (var l in humansBeforeCache)
            {
                await _cache.SetAsync(key, l, TimeSpan.FromSeconds(5));
            }

            var cacheValue = await _cache.GetAsync<Human>(key);

            //assert

            Assert.That(cacheValue, Is.EqualTo(humansBeforeCache.Last()));
        }

        [Test]
        public async Task SetAsync_MultipleSets_ReturnsValuesBeforeCaching()
        {
            //arrange

            List<KeyValuePair<string, Human>> humansBeforeCache = new List<KeyValuePair<string, Human>>();

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

            //act

            foreach (var l in humansBeforeCache)
            {
                await _cache.SetAsync(l.Key, l.Value, TimeSpan.FromSeconds(5));
            }

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

            Assert.That(humansAfterCache, Is.EqualTo(humansBeforeCache));
        }
    }
}

#pragma warning restore CS8625 // Литерал, равный NULL, не может быть преобразован в ссылочный тип, не допускающий значение NULL.