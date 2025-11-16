using ID.Email.Base.LocalImps;

namespace ID.Email.Base.Tests;

public class TemplateLoaderTests
{
    [Fact]
    public async Task LoadAsync_ReturnsFileFromContentRoot()
    {
        // Arrange
        var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(temp);
        try
        {
            var rel = Path.Combine("Assets", "html-templates", "test.html");
            var full = Path.Combine(temp, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(full)!);
            await File.WriteAllTextAsync(full, "hello-content-root");

            var envMock = new Mock<IHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(temp);

            var loader = new TemplateLoader(envMock.Object);

            // Act
            var res = await loader.LoadAsync(rel);

            // Assert
            res.Succeeded.ShouldBeTrue();
            res.Value.ShouldBe("hello-content-root");
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }

    //--------------------------//

    [Fact]
    public async Task LoadAsync_ReturnsFileFromBinFallback()
    {
        // Arrange
        var tempContent = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempContent);
        var asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var rel = Path.Combine("Assets", "html-templates", "binfallback.html");
        var full = Path.Combine(asmDir, rel);
        Directory.CreateDirectory(Path.GetDirectoryName(full)!);
        try
        {
            await File.WriteAllTextAsync(full, "hello-bin");

            var envMock = new Mock<IHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(tempContent);

            var loader = new TemplateLoader(envMock.Object);

            // Act
            var res = await loader.LoadAsync(rel);

            // Assert
            res.Succeeded.ShouldBeTrue();
            res.Value.ShouldBe("hello-bin");
        }
        finally
        {
            if (File.Exists(full)) File.Delete(full);
            Directory.Delete(tempContent, true);
        }
    }

    //--------------------------//

    [Fact]
    public async Task LoadAsync_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var temp = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(temp);
        try
        {
            var envMock = new Mock<IHostEnvironment>();
            envMock.Setup(e => e.ContentRootPath).Returns(temp);

            var loader = new TemplateLoader(envMock.Object);

            // Act
            var res = await loader.LoadAsync("nonexistent.html");

            // Assert
            res.Succeeded.ShouldBeFalse();
            res.Status.ShouldBe(MyResults.BasicResult.ResultStatus.NotFound);
        }
        finally
        {
            Directory.Delete(temp, true);
        }
    }
}
