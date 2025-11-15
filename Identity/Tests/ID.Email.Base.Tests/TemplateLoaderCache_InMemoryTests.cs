namespace ID.Email.Base.Tests;

public class TemplateLoaderCache_InMemoryTests
{
    [Fact]
    public async Task LoadAsync_CachesLoadedTemplate_WhenFilePresent()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        var rel = Path.Combine("Assets", "html-templates", "cached.html");
        var full = Path.Combine(tempDir, rel);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        await File.WriteAllTextAsync(full, "cached-content");

        var envMock = new Mock<IHostEnvironment>();
        envMock.Setup(e => e.ContentRootPath).Returns(tempDir);

        var originalLoaderMock = new Mock<ITemplateLoader>();
        originalLoaderMock.Setup(l => l.LoadAsync(rel)).ReturnsAsync(GenResult<string>.Success("cached-content"));

        var memory = new MemoryCache(new MemoryCacheOptions());
        var opts = Options.Create(new TemplateCacheOptions { SlidingExpirationMins = 60 });
        var invalidator = new TemplateCacheInvalidator();
        var logger = new NullLogger<TemplateLoaderCache_InMemory>();

        var cache = new TemplateLoaderCache_InMemory(originalLoaderMock.Object, memory, opts, invalidator, NullLogger<TemplateLoaderCache_InMemory>.Instance, envMock.Object);

        // Act
        var r1 = await cache.LoadAsync(rel);
        var r2 = await cache.LoadAsync(rel);

        // Assert
        r1.Succeeded.ShouldBeTrue();
        r1.Value.ShouldBe("cached-content");
        r2.Succeeded.ShouldBeTrue();
        r2.Value.ShouldBe("cached-content");

        // Original loader should have been called once due to caching
        originalLoaderMock.Verify(l => l.LoadAsync(rel), Times.Once);

        Directory.Delete(tempDir, true);
    }

    //--------------------------//

    [Fact]
    public async Task LoadAsync_UsesAssemblyStamp_WhenNoFile()
    {
        // Arrange
        var rel = Path.Combine("Assets", "html-templates", "embedded-fallback.html");
        var originalLoaderMock = new Mock<ITemplateLoader>();
        originalLoaderMock.Setup(l => l.LoadAsync(rel)).ReturnsAsync(GenResult<string>.Success("embedded-content"));

        var memory = new MemoryCache(new MemoryCacheOptions());
        var opts = Options.Create(new TemplateCacheOptions { SlidingExpirationMins = 60 });
        var invalidator = new TemplateCacheInvalidator();
        var envMock = new Mock<IHostEnvironment>();
        envMock.Setup(e => e.ContentRootPath).Returns(Path.GetTempPath());

        var cache = new TemplateLoaderCache_InMemory(originalLoaderMock.Object, memory, opts, invalidator, NullLogger<TemplateLoaderCache_InMemory>.Instance, envMock.Object);

        // Act
        var r = await cache.LoadAsync(rel);

        // Assert
        r.Succeeded.ShouldBeTrue();
        r.Value.ShouldBe("embedded-content");
    }
}
