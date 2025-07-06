using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Moq;
using PrerenderedSpa.PrerenderedDefaultFiles;
using Shouldly;

namespace PrerenderedSpa.Tests;

public class SpaDefaultFilesMiddlewareTests
{

    [Fact]
    public async Task Invoke_ServesDefaultFile_WhenExists()
    {
        // Arrange
        var fileProvider = new Mock<IFileProvider>();
        var fileInfo = new Mock<IFileInfo>();
        fileInfo.Setup(f => f.Exists).Returns(true);
        fileProvider.Setup(f => f.GetFileInfo(It.IsAny<string>())).Returns(fileInfo.Object);

        var options = Options.Create(GetOptions(fileProvider.Object));
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/foo/";
        var called = false;
        Task next(HttpContext ctx) { called = true; return Task.CompletedTask; }
        var middleware = new SpaDefaultFilesMiddleware(next, Mock.Of<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>(), options);

        // Act
        await middleware.Invoke(context);

        // Assert
        called.ShouldBeTrue();
        context.Request.Path.Value.ShouldEndWith("/foo/index.html");
    }

    [Fact]
    public async Task Invoke_FallsBack_WhenDefaultFileDoesNotExist()
    {
        // Arrange
        var fileProvider = new Mock<IFileProvider>();
        var fileInfo = new Mock<IFileInfo>();
        fileInfo.Setup(f => f.Exists).Returns(false);
        fileProvider.Setup(f => f.GetFileInfo(It.IsAny<string>())).Returns(fileInfo.Object);

        var options = Options.Create(GetOptions(fileProvider.Object, fallbackDirectoryPath: "/fallback"));
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/foo";
        var called = false;
        Task next(HttpContext ctx) { called = true; return Task.CompletedTask; }
        var middleware = new SpaDefaultFilesMiddleware(next, Mock.Of<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>(), options);

        // Act
        await middleware.Invoke(context);

        // Assert
        called.ShouldBeTrue();
        context.Request.Path.Value?.ShouldContain("/fallback/index.html");
    }

    [Fact]
    public async Task Invoke_SkipsFileRequests()
    {
        // Arrange
        var fileProvider = new Mock<IFileProvider>();
        var options = Options.Create(GetOptions(fileProvider.Object));
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/foo/bar.js";
        var called = false;
        Task next(HttpContext ctx) { called = true; return Task.CompletedTask; }
        var middleware = new SpaDefaultFilesMiddleware(next, Mock.Of<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>(), options);

        // Act
        await middleware.Invoke(context);

        // Assert
        called.ShouldBeTrue();
        context.Request.Path.Value.ShouldBe("/foo/bar.js");
    }

    [Fact]
    public async Task Invoke_RedirectsToPathWithSlash_WhenMissingSlash()
    {
        // Arrange
        var fileProvider = new Mock<IFileProvider>();
        var fileInfo = new Mock<IFileInfo>();
        fileInfo.Setup(f => f.Exists).Returns(true);
        fileProvider.Setup(f => f.GetFileInfo(It.IsAny<string>())).Returns(fileInfo.Object);

        var options = Options.Create(GetOptions(fileProvider.Object));
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/foo";
        var called = false;
        Task next(HttpContext ctx) { called = true; return Task.CompletedTask; }
        var middleware = new SpaDefaultFilesMiddleware(next, Mock.Of<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>(), options);

        // Patch PrerenderedHelpers.PathEndsInSlash to return false and RedirectToPathWithSlash to set a header
        // (Would require refactoring for full testability)
        // For now, just check that next is not called and status is 301

        // Act
        await middleware.Invoke(context);

        // Assert
        called.ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(301);
    }

    [Fact]
    public async Task Invoke_PathWithoutTrailingSlash_IsRedirectedWithSlash()
    {
        // Arrange
        var fileProvider = new Mock<IFileProvider>();
        var fileInfo = new Mock<IFileInfo>();
        fileInfo.Setup(f => f.Exists).Returns(true);
        fileProvider.Setup(f => f.GetFileInfo(It.IsAny<string>())).Returns(fileInfo.Object);

        var options = Options.Create(GetOptions(fileProvider.Object));
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/about";
        var called = false;
        Task next(HttpContext ctx) { called = true; return Task.CompletedTask; }
        var middleware = new SpaDefaultFilesMiddleware(next, Mock.Of<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>(), options);

        // Act
        await middleware.Invoke(context);

        // Assert
        called.ShouldBeFalse();
        context.Response.StatusCode.ShouldBe(StatusCodes.Status301MovedPermanently);
        context.Response.Headers["Location"].ToString().ShouldContain("/about/");
    }

    //----------------------------//


    private static SpaDefaultFilesOptions GetOptions(IFileProvider fileProvider, string? defaultFileName = null, string? fallbackFileName = null, string? fallbackDirectoryPath = null, bool fallbackToNearestParent = true)
    {
        return new SpaDefaultFilesOptions
        {
            FileProvider = fileProvider,
            DefaultFileName = defaultFileName ?? "index.html",
            FallbackFileName = fallbackFileName ?? "index.html",
            FallbackDirectoryPath = fallbackDirectoryPath,
            FallbackToNearestParentDirectory = fallbackToNearestParent
        };
    }



}//Cls
