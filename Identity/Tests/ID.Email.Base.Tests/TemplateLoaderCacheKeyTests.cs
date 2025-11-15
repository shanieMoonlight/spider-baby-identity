using System.Text.RegularExpressions;

namespace ID.Email.Base.Tests;

public class TemplateLoaderCacheKeyTests
{
    [Fact]
    public async Task LoadAsync_CachesValue_And_KeyContains16HexHash()
    {
        // Arrange
        var templatePath = "Assets/html-templates/EmailConfirmation/IdEmailConfirmationCustomer.html";
        var templateContent = "template-content";

        var originalLoaderMock = new Mock<ITemplateLoader>();
        originalLoaderMock.Setup(l => l.LoadAsync(templatePath)).ReturnsAsync(GenResult<string>.Success(templateContent));

        var memory = new TestMemoryCache();
        var opts = Options.Create(new TemplateCacheOptions { SlidingExpirationMins = 60 });
        var invalidator = new TemplateCacheInvalidator();
        var envMock = new Mock<IHostEnvironment>();
        envMock.Setup(e => e.ContentRootPath).Returns(AppContext.BaseDirectory);

        var sut = new TemplateLoaderCache_InMemory(originalLoaderMock.Object, memory, opts, invalidator, NullLogger<TemplateLoaderCache_InMemory>.Instance, envMock.Object);

        // Act
        var r1 = await sut.LoadAsync(templatePath);
        var r2 = await sut.LoadAsync(templatePath);

        // Assert
        r1.Succeeded.ShouldBeTrue();
        r2.Succeeded.ShouldBeTrue();
        r1.Value.ShouldBe(templateContent);
        r2.Value.ShouldBe(templateContent);

        // original loader called only once due to caching
        originalLoaderMock.Verify(l => l.LoadAsync(templatePath), Times.Once);

        // Inspect test memory cache entries to find the stored key/value
        memory.Entries.Count.ShouldBeGreaterThan(0);
        var pair = memory.Entries.FirstOrDefault(kv => kv.Value as string == templateContent);
        pair.Equals(default(KeyValuePair<object, object?>)).ShouldBeFalse();

        var key = pair.Key as string;
        key.ShouldNotBeNullOrWhiteSpace();

        // extract short hash (last segment after final '.')
        var lastDot = key.LastIndexOf('.');
        lastDot.ShouldBeGreaterThan(0);
        var hash = key.Substring(lastDot + 1);

        hash.Length.ShouldBe(16);
        Regex.IsMatch(hash, "^[0-9A-F]{16}$").ShouldBeTrue();
    }

    //##############################################################//

    // Simple test double for IMemoryCache that stores entries in a dictionary when disposed
    private class TestMemoryCache : IMemoryCache
    {
        public readonly Dictionary<object, object?> Entries = new();

        public ICacheEntry CreateEntry(object key) => new TestCacheEntry(this, key);

        public void Dispose() { }

        public void Remove(object key) => Entries.Remove(key);

        public bool TryGetValue(object key, out object? value) => Entries.TryGetValue(key, out value);

        private sealed class TestCacheEntry : ICacheEntry
        {
            private readonly TestMemoryCache _cache;
            public TestCacheEntry(TestMemoryCache cache, object key) { _cache = cache; Key = key; }

            public object Key { get; }
            public object? Value { get; set; }
            public DateTimeOffset? AbsoluteExpiration { get; set; }
            public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
            public TimeSpan? SlidingExpiration { get; set; }
            public IList<Microsoft.Extensions.Primitives.IChangeToken> ExpirationTokens { get; } = new List<Microsoft.Extensions.Primitives.IChangeToken>();
            public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; } = new List<PostEvictionCallbackRegistration>();
            public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;
            public long? Size { get; set; }

            public void Dispose()
            {
                // when the entry is disposed the memory-cache extension will have set Value
                _cache.Entries[Key] = Value;
            }
        }
    }

}//Cls
