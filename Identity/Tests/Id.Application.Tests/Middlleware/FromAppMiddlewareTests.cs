using ID.Application.AppAbs.FromApp;
using ID.Application.Middleware;
using ID.Application.Setup;
using ID.Application.Tests.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace ID.Application.Tests.Middlleware;

[Collection(TestingConstants.NonParallelCollection)]
public class FromAppMiddlewareTests
{
    private const string _headerKey = "X-Mobile-App";
    private const string _headerValue = "abcdef123456";
    private readonly RequestDelegate _nextDelegate = context => Task.CompletedTask;


    //---------------------------//

    [Fact]
    public async Task InvokeAsync_SetsIsFromApp_WhenHeaderIsPresent()
    {
        // Arrange
        var mockFromAppService = new Mock<IIsFromMobileApp>();
        var context = new DefaultHttpContext();
        context.Request.Headers[_headerKey] = _headerValue;

        var options = CreateOptions();
        var middleware = new FromAppMiddleware(_nextDelegate, mockFromAppService.Object, options);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        mockFromAppService.VerifySet(service => service.IsFromApp = true, Times.Once);
    }

    //---------------------------//

    [Fact]
    public async Task InvokeAsync_DoesNotSetIsFromApp_WhenHeaderIsNotPresent()
    {
        // Arrange
        var mockFromAppService = new Mock<IIsFromMobileApp>();
        var context = new DefaultHttpContext();
        var options = CreateOptions();

        var middleware = new FromAppMiddleware(_nextDelegate, mockFromAppService.Object, options);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        mockFromAppService.VerifySet(service => service.IsFromApp = false, Times.Once);
    }

    //---------------------------//

    [Fact]
    public async Task InvokeAsync_DoesNotSetIsFromApp_WhenHeaderValueDoesNotMatch()
    {
        // Arrange
        var mockFromAppService = new Mock<IIsFromMobileApp>();
        var context = new DefaultHttpContext();
        context.Request.Headers[_headerKey] = "wrong-value";

        var options = CreateOptions();
        var middleware = new FromAppMiddleware(_nextDelegate, mockFromAppService.Object, options);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        mockFromAppService.VerifySet(service => service.IsFromApp = false, Times.Once);
    }


    //---------------------------//


    private static IOptions<IdApplicationOptions> CreateOptions(string? headerKey = null, string? headerValue = null)
    {
        return Options.Create(new IdApplicationOptions
        {
            FromAppHeaderKey = headerKey ?? _headerKey,
            FromAppHeaderValue = headerValue ?? _headerValue
        });
    }


}//Cls